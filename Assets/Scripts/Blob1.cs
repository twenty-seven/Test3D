using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace BlobWars {
	/**
	 * This class contains the Blob and 
	 * their specific functionalities. 
	 * Most synchronizing and selection, as well as 
	 * combat is defined here.
	 *
	 */
	public class Bloba : HealthObject {
		// TODO: Should be remove, just used to rotate the blob correctly
		//private Quaternion offset = new Quaternion ();
		// The minimal distance between the current position and destination, to trigger movement.
		public double minDistance = .1;
		// Distance the Blob walks after he is spawned (walking out of the tower)
		public Vector3 stepOut = Vector3.zero;
		// Differentiate between a selected Blob and an unselected Blob (navigation)
		public bool isSelected = false;
		// Damage the blob does to other blobs
		public int damage = 5;
		// Range over which the blob can attack other blobs
		public int range = 10;
		// Speed with which it attacks in seconds
		public float attackSpeed = 2f;
		// Private placeholder for speed treshold
		private float nextTime;
		// Synchronization smoothing
		private int lerpRate = 1;
		// Blob speed
		public int speed = 15;
		private Animator anim;
		private SlimeAnim slimeAnim;
		[SyncVar]
		Vector3 syncPos;
		[SyncVar]
		Quaternion syncRot;
		// Unique Blob Id
		public string uid;
		// The Location the Blob is currently traveling towards
		[SyncVar]
		public Vector3 syncDestination;
		// Only sync the name, the object can be fetched later
		[SyncVar]
		public string towerName;
		public GameObject tower;
		public override void PreStartClient() {

		}
		// Use this for initialization
		void Start () {
			// Get tower, id and position/rotation
			tower = GameObject.Find (towerName);			
			uid = towerName + "." + GetComponent<NetworkIdentity> ().netId.ToString ();
			transform.name = uid;
			syncPos = transform.position;
			syncRot = transform.rotation;
			currentHealth = maxHealth;
			if (isClient) {
				
				GetComponent<NetworkAnimator> ().SetParameterAutoSend (0, true);
				GetComponent<NetworkAnimator> ().SetParameterAutoSend (1, true);
			}
			if (isServer) {
				Debug.Log ("Spawning " + name + " of tower " + towerName + " at " + transform.position);
				// Get Animation Component, not needed on clients
				anim = GetComponent<Animator>();
				slimeAnim = GetComponent<SlimeAnim>();
				GetComponent<NetworkAnimator> ().SetParameterAutoSend (0, true);
				GetComponent<NetworkAnimator> ().SetParameterAutoSend (1, true);
				// Setup Health
				nextTime = Time.time + attackSpeed;
				// Make them step out
				if (Vector3.Equals (stepOut, Vector3.zero)) {
					if (tower.transform.position.z < 0) {
						stepOut = Vector3.forward * 15;
					} else {
						stepOut = Vector3.back * 15;
					}
				}
			}
			// Send command to server through Local Player in Tower
		//	tower.GetComponent<Tower>().moveSoldier(name,transform.position + stepOut);

		}

		// Maybe check for obstacle here too or create similar command function
		[Command]
		void CmdCheckForEnemies () {

			GameObject[] Blobs = GameObject.FindGameObjectsWithTag (tag);

		
			// For each Blob on the field
			if (nextTime < Time.time) { 
				for (var d = 0; d  < Blobs.Length; d++) {
					Blob blob = Blobs [d].GetComponent<Blob> ();
					// Skip my own blobs
					if (null == blob || blob.towerName == towerName) {
						continue;
					} else {
						// Get the enemy tower if we don't know it already

						//if (null == enemyTower) enemyTower = blob.tower;
						float distance = Vector3.Distance (blob.transform.position, transform.position);

						// If a blob is in range, attack blob.
						if ( distance <= range) {
							CmdTurnTo(blob.transform.position);
							anim.SetBool("Attack",true);
							Transform ballTransform = gameObject.transform.FindChild("attackball");

							if (ballTransform != null) {
								AttackBall ballComponent = ballTransform.GetComponent<AttackBall>();
								if (ballComponent != null) {
								Debug.Log ("Found Ranged");
								ballComponent.moveTo(blob.transform.position);
								}
								//GameObject b= Instantiate((GameObject) aBall,aBall.transform.position,aBall.transform.rotation);
							}
							// TODO: Ranged attacks

							// Damage is done here, including animation
							// this would be the place to ranged attacks
							blob.CmdDamageObject(damage);

							nextTime = Time.time + attackSpeed;
							return;
						
						} 
					} 

				}

				GameObject enemyTower;
				// only two players
				GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
				for (int i = 0; i < players.Length; i++) {
					if (players[i].name != towerName) {
						enemyTower = players[i].GetComponent<Tower>().gameObject;
						if (null != enemyTower &&  Vector3.Distance (enemyTower.transform.position, transform.position) <= range) {
							CmdTurnTo (enemyTower.transform.position);
							anim.SetBool("Attack",true);
							// TODO: Ranged attacks
							// Damage is done here, including animation 
							// this would be the place to ranged attacks
							enemyTower.GetComponent<Tower>().CmdDamageObject(damage);
							
							
						}
						break;
					}
				}
				// if no blob was hit and tower is in range


			
			}
		}



		// Update is called once per frame
		void Update () {
			// If we're on the server, calculate movement and send it through network
			if (isServer) {
				CmdStepMove ();
				CmdCheckForEnemies ();
				// Setup Sync Vars
				syncPos = transform.position;
				syncRot = transform.rotation;
				// TODO: Maybe CheckForObstacles(); ?
			} else {
				// If we're a client, just move the object
				lerpPosition();
			}

		}

		[Command]
		// Make a single step towards the target location 
		private void CmdStepMove () {
			// If the distance is bigger than the minimal distance
			if (Vector3.Distance (anim.rootPosition, syncDestination) > minDistance) {
				
				anim.SetBool("IsWalking",true);
				// TODO: PATHFINDING 
				// Do your own pathfinding here
				Vector3 movement = syncDestination - anim.rootPosition;
				// Normalise the movement vector and make it proportional to the speed per second.
				movement = movement.normalized * speed * Time.deltaTime;
				// Move the player to it's current position plus the movement.
				Debug.Log ("pos: " + transform.position*100 + " anim: " + anim.rootPosition*100 + " mov: " + movement);
				transform.position = anim.rootPosition + movement;
				Debug.Log ("After: " + transform.position*100 + " anim: " + anim.rootPosition*100);

				CmdTurnTo(syncDestination);
			} else {
				transform.position = syncDestination;
				anim.SetBool("IsWalking",false);
			}
		}
		[Command]
		void CmdTurnTo(Vector3 point) {
			var targetRotation = Quaternion.LookRotation (point - transform.position, Vector3.up);
			//targetRotation.y = offset.y;
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);

		}
		[Client]
		// Returns the 3D Destination of the hit point of a ray through the current mouse position
		Vector3 GetDestination() {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast (ray, out hit)) {
				
				// Debug.Log ("Ray Hit " + objectHit + " at " + hit.point);	
				Vector3 destination = hit.point;
				destination.y = transform.position.y;
				return destination;
			} 
			return transform.position;
		}
		[Client]
		// Smooth out movement
		void lerpPosition() {
			if (isClient) {
				//Debug.Log ("Moving towards " + synPos);
				transform.position = Vector3.Lerp (transform.position, syncPos, Time.deltaTime * lerpRate);
				transform.rotation = Quaternion.Lerp (transform.rotation, syncRot, Time.deltaTime * lerpRate);
			}
		}
	}
}