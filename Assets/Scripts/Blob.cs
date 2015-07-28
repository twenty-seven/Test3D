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
	public class Blob : HealthObject {
		// TODO: Should be remove, just used to rotate the blob correctly
		private Quaternion offset = new Quaternion ();
		// The minimal distance between the current position and destination, to trigger movement.
		public double minDistance = .1;
		// Distance the Blob walks after he is spawned (walking out of the tower)
		public Vector3 stepOut = Vector3.zero;
		// Differentiate between a selected Blob and an unselected Blob (navigation)
		private bool isSelected = false;
		// Damage the blob does to other blobs
		public int damage = 5;
		// Range over which the blob can attack other blobs
		public int range = 10;
		// Speed with which it attacks in seconds
		public float attackSpeed = 2f;
		// Private placeholder for speed treshold
		private float nextTime;
		// Synchronization smoothing
		private int lerpRate = 15;
		// Blob speed
		public int speed = 15;

		public SlimeAnim slAnim;
		// Unique Blob Id
		[SyncVar]
		public string uid;
		// The rotation of the Object.
		[SyncVar]
		public Quaternion syncRot;
		// The Location the Blob is currently traveling towards
		[SyncVar]
		private Vector3 syncDestination;
		// The Location the Blob is currently at.
		[SyncVar]
		public Vector3 syncPos;
		// Only sync the name, the object can be fetched later
		[SyncVar]
		public string towerName;
		public GameObject tower;

		// Use this for initialization
		void Start () {
			// Set up unique ID, current Healthpoints and the attack speed
			currentHealth = maxHealth;
			nextTime = Time.time + attackSpeed;
			uid = towerName + "." + GetComponent<NetworkIdentity> ().netId.ToString ();
			transform.name = uid;
			offset.y = .785f;
			slAnim = GetComponent<SlimeAnim> ();
			// Make sure we don't jump to (0,0,0)
			syncPos = transform.position;
			syncRot = transform.rotation;
			// My commanding tower
			tower = GameObject.Find (towerName);
			// The Object steps out of the tower
			if (Vector3.Equals(stepOut,Vector3.zero)) {
				if (tower.transform.position.z < 0) {

					stepOut = Vector3.forward * 15;
				} else {
					stepOut = Vector3.back * 15;
				}
			}
			// Make the object move
			MoveTo(transform.position + stepOut);
		}

		// Maybe check for obstacle here too or create similar command function
		[Command]
		void CmdCheckForEnemies () {

			GameObject[] Blobs = GameObject.FindGameObjectsWithTag (tag);

			GameObject enemyTower;
			// For each Blob on the field
			if (nextTime < Time.time) { 
				for (var d = 0; d  < Blobs.Length; d++) {
					Blob blob = Blobs [d].GetComponent<Blob> ();
					// Skip my own blobs
					if (blob.towerName == towerName) {
						continue;
					} else {
						// Get the enemy tower if we don't know it already

						enemyTower = blob.tower;
						// If the tower is in range, attack tower.
						if (Vector3.Distance (blob.transform.position, transform.position) <= range) {
							// TODO: Move closer to enemy blob before attack
							transform.LookAt (enemyTower.transform.position);

							slAnim.doAttack = true;	
							GameObject aBall = gameObject.transform.FindChild("attackball").gameObject;
							if (aBall != null) {
								Debug.Log ("Found Ranged");
								//GameObject b= Instantiate((GameObject) aBall,aBall.transform.position,aBall.transform.rotation);
							}
							// TODO: Ranged attacks

							// Damage is done here, including animation
							// this would be the place to ranged attacks
							enemyTower.GetComponent<Tower> ().CmdDamageObject (damage);

							nextTime = Time.time + attackSpeed;
							
							break;
						
						} // If tower is not in range and current blob belongs to enemy
					else {
							// TODO: Move closer to enemy blob before attack
							// Check if he's in range
							if (Vector3.Distance (enemyTower.transform.position, transform.position) <= range) {
								transform.LookAt (Blobs [d].transform.position);
							slAnim.doAttack = true;
							// TODO: Ranged attacks
							// Damage is done here, including animation 
							// this would be the place to ranged attacks
							blob.CmdDamageObject (damage);

							break;
							}


						}
					} 

				}
			
			}
		}


		/**
		 * The following functions transmit the position and rotation data
		 * from the server side Blobs to the client side blobs.
		 */
		[Client]
		void TransmitPosition(Vector3 pos, Quaternion rot) {
			syncPos = pos;
			syncRot = rot;
		}

		[Command]
		void CmdTransmitPosition() {
			TransmitPosition (transform.position, transform.rotation);
		}

		// Update is called once per frame
		void Update () {
			// If we're on the server, calculate movement and send it through network
			if (isServer) {
				CmdTransmitPosition ();
				CmdCheckForEnemies();
				StepMove ();
				// TODO: Maybe CheckForObstacles(); ?
			} else {
				// Else simulate movement of remote objects
				lerpPosition ();
			}
			// If it's not the server 
			if (isClient) {
				// and someone clicks
				if (Input.GetKey ("mouse 0")) {
					// Check if the object that was hit belongs to us
					RaycastHit hit;
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					// If the ray actually hits something
					if (Physics.Raycast (ray, out hit)) {

						// Check if it is a blob and whether it belongs to the local Player
						Blob b = hit.transform.GetComponent<Blob> ();


						if (b != null && b.tower != null) {
							Tower t = b.tower.GetComponent<Tower>();
							NetworkIdentity ni = b.tower.GetComponent<NetworkIdentity> ();
							if (ni.isLocalPlayer && t.selectedBlob == "") {
								// If so you can select the blob
								b.isSelected = true;
								b.tower.GetComponent<Tower>().selectedBlob = name;

							}
						} else if (isSelected && isClient) {
							// A selected blob can be send to a destination
							syncDestination = GetDestination ();
							// it will use the stepMove function, do zour pathfinding there
							slAnim.isWalking = true;
							// Sends new destination to all client objects through tower
							tower.GetComponent<Tower> ().TransmitDestination (name, syncDestination);
							isSelected = false;
							tower.GetComponent<Tower>().selectedBlob = "";
						} 
					}
				}
				if (Input.GetKey("j")) {
					if (GetComponentInChildren<AttackBall>() != null)
						GetComponentInChildren<AttackBall>().shootAt(Vector3.zero);
				}
			}
		}

		// Sets destination, the rest is triggered from Update
		public void MoveTo(Vector3 location) {

			if (transform.position == tower.transform.position && location == (tower.transform.position + stepOut)) {
				tower.GetComponent<TowerAnim> ().doorsOpen = false;
			}
			syncDestination = location;

		}
		// Make a single step towards the target location 
		private void StepMove () {
			// If the distance is bigger than range, or I just spawned and have to leave the tower.
			if (Vector3.Distance (transform.position, syncDestination) > minDistance) {
				// TODO: PATHFINDING 
				// Do your own pathfinding here
				Vector3 movement = syncDestination - transform.position;
				// Normalise the movement vector and make it proportional to the speed per second.
				movement = movement.normalized * speed * Time.deltaTime;
				// Move the player to it's current position plus the movement.
				transform.position = transform.position + movement;
				TurnTo(syncDestination);
			} else {
				// Turn off animation after you finished walking
				slAnim.isWalking = false;
			}


			
		}
		void TurnTo(Vector3 point) {
			var targetRotation = Quaternion.LookRotation (point - transform.position, Vector3.up);
			targetRotation.y = offset.y;
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
		}
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
		// Smooth out movement
		void lerpPosition() {
			if (!tower.GetComponent<NetworkIdentity>().isLocalPlayer) {
				//Debug.Log ("Moving towards " + synPos);
				transform.position = Vector3.Lerp (transform.position, syncPos, Time.deltaTime * lerpRate);
				transform.rotation = Quaternion.Lerp (transform.rotation, syncRot, Time.deltaTime * lerpRate);
			}
		}
	}
}