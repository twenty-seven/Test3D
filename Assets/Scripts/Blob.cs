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
		//private Quaternion offset = new Quaternion ();
		// The minimal distance between the current position and destination, to trigger movement.
		public double minDistance;
		// Number of seconds to wait with door closing
		public int doorOpenSeconds;
		// Distance the Blob walks after he is spawned (walking out of the tower)
		public Vector3 stepOut = Vector3.zero;
		// Differentiate between a selected Blob and an unselected Blob (navigation)
		public bool isSelected = false;
		// Damage the blob does to other blobs
		public int damage;
		// Range over which the blob can attack other blobs
		public int range;
		// Speed with which it attacks in seconds
		public float attackSpeed;
		// Private placeholder for speed treshold
		private float nextTime;
		// Synchronization smoothing
		private int lerpRate = 5;
		// Blob speed
		public int speed;

		public SlimeAnim slAnim;
		// Unique Blob Id
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

		//Audio
		public AudioClip atkAudio;
		public AudioClip moveAudio;
		void Awake () {

		}
		// Use this for initialization
		void Start () {
			// Set up unique ID, current Healthpoints and the attack speed
			currentHealth = maxHealth;
			nextTime = Time.time + attackSpeed;
			uid = towerName + "." + GetComponent<NetworkIdentity> ().netId.ToString ();
			transform.name = uid;
			//offset.y = .785f;
			slAnim = GetComponent<SlimeAnim> ();
			// Keep them from jumping to 0,0,0
			 syncDestination = transform.position;
			// My commanding tower
			tower = GameObject.Find (towerName);
			tower.GetComponent<TowerAnim> ().openDoors();
			// The Object steps out of the tower
			if (Vector3.Equals(stepOut,Vector3.zero)) {
				if (tower.transform.position.z < 0) {

					stepOut = Vector3.forward * 100;
				} else {
					stepOut = Vector3.back * 100;
				}
			}
			base.Start ();

			syncPos = transform.position;
			syncRot = transform.rotation;

			// Make the object move out of the tower
			MoveTo (transform.position + stepOut);
			StartCoroutine(WaitWithDoorAnimation ());

		}
		IEnumerator WaitWithDoorAnimation() {
			yield return new WaitForSeconds(doorOpenSeconds);
			tower.GetComponent<TowerAnim> ().closeDoors ();
		}
		// Maybe check for obstacle here too or create similar command function
		[Command]
		void CmdCheckForEnemies () {

			GameObject[] Blobs = GameObject.FindGameObjectsWithTag (tag);

			GameObject enemyTower;
			// For each Blob on the field
			if (nextTime < Time.time) {
				nextTime = Time.time + attackSpeed;
				// Only attack one target, once each shot
				for (var d = 0; d  < Blobs.Length; d++) {
					Blob blob = Blobs [d].GetComponent<Blob> ();
					// Skip erroneous blobs. 
					// Sometimes the algorithm checks not yet fully instantiated blobs, that do not yet have a towername
					if (blob == null || blob.towerName == null) {
						continue;
					}
					// Skip allied blobs
					if (blob.towerName == towerName) {
						continue;
					} else {

						//Debug.Log ("Checking enemy position from " + uid + " to " + blob.uid + " " + Vector3.Distance (Blobs [d].transform.position, transform.position));
						// Get the enemy tower if we don't know it already
						enemyTower = blob.tower;
						// If the blob is in range, attack blob.
						if (Vector3.Distance (Blobs [d].transform.position, transform.position) <= range) {

							transform.LookAt (Blobs [d].transform.position);
							slAnim.doAttack = true;	

							// First attempt at ranged attacks:
							//GameObject aBall = gameObject.transform.FindChild("attackball").gameObject;
							//if (aBall != null) {
								//GameObject b= Instantiate((GameObject) aBall,aBall.transform.position,aBall.transform.rotation);
							//}


							// Damage is done here, including animation
							// this would be the place to ranged attacks

							aSource.PlayOneShot(atkAudio);
							blob.CmdDamageObject (damage);
							break; // Break out of for loop after a Blob has been attacked
						} // If no blobs are in range
					else {
							// Check if he's in range
							if (Vector3.Distance (enemyTower.transform.position, transform.position) <= range) {

								transform.LookAt (enemyTower.transform.position);

								slAnim.doAttack = true;
								aSource.PlayOneShot(atkAudio);
								// TODO: Ranged attacks
								// Damage is done here, including animation 
								// this would be the place to ranged attacks
								enemyTower.GetComponent<Tower> ().CmdDamageObject (damage);
								break; // Break out of for loop after the tower has been attacked, so it doesn't try to attack the remaining blobs in the for loop.

							}


						}

					} 

				}
			}
		}



		// Update is called once per frame
		void Update () {
			// If we're on the server, calculate movement and send it through network
			if (isServer) {
				StepMove ();
				CmdCheckForEnemies ();
			// TODO: Maybe CheckForObstacles(); ?
			} else {
			// Else simulate movement of remote objects
				lerpPosition ();
			}

		}

		// Checks whether it's the movmenet out of the tower and handles the animation.
		// Turns on Walking ANimation and sets new destination, which triggers movement from the update function.
		public void MoveTo(Vector3 location) {
			syncDestination = location;
		}

		// Make a single step towards the target location 
		private void StepMove () {
			Vector3 currentPos;
			Animator anim = GetComponent<Animator>();
			// Use rootPosition of the animator to get a position unbound to the animation
			if (anim != null ) {
				currentPos = GetComponent<Animator> ().rootPosition;
				
			} else {
				currentPos = transform.position;
			}

			// If the distance is bigger than range, or I just spawned and have to leave the tower.
			if (Vector3.Distance (transform.position, syncDestination) > minDistance) {
				slAnim.isWalking = true;
				// TODO: PATHFINDING 
				// Do your own pathfinding here
				Vector3 movement = syncDestination - currentPos;
				// Normalise the movement vector and make it proportional to the speed per second.
				movement = movement.normalized * speed * Time.deltaTime;

				// Move the player to it's current position plus the movement.
				transform.position = currentPos + movement;


				TurnTo (syncDestination);
			} else {			
				// Turn off animation after you finished walking


				slAnim.isWalking = false;
				transform.position = syncDestination;			//anim.rootPosition = syncDestination;
			}
			// Synchronize Movement
			syncPos = transform.position;
			syncRot = transform.rotation;


			
		}
		void TurnTo(Vector3 point) {
			var targetRotation = Quaternion.LookRotation (point - transform.position, Vector3.up);
			//targetRotation.y = offset.y;
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
			Animator anim = GetComponent<Animator>();
			Vector3 currentPos;
			if (anim != null ) {
				currentPos = anim.rootPosition;
				
			} else {
				currentPos = transform.position;
			}
			if (Vector3.Distance (currentPos, syncPos) > minDistance) {

				//Debug.Log ("Moving towards " + synPos);
				transform.position = Vector3.Lerp (transform.position, syncPos, Time.deltaTime * lerpRate);
				transform.rotation = Quaternion.Lerp (transform.rotation, syncRot, Time.deltaTime * lerpRate);
			} else {
				transform.position = syncPos;
				//slAnim.isWalking = false;
			}
		}

		//trigger this via animation event
		void PlayMoveAudio() {
			//play audio for movement once
			aSource.PlayOneShot (moveAudio);
		}
	}
}