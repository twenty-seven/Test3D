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
		private GameObject tower;

		// Use this for initialization
		void Start () {
			// Set up unique ID, current Healthpoints and the attack speed
			currentHealth = maxHealth;
			nextTime = Time.time + attackSpeed;
			uid = towerName + "." + GetComponent<NetworkIdentity> ().netId.ToString ();
			transform.name = uid;
			// Make sure we don't jump to (0,0,0)
			syncPos = transform.position;
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

		[Command]
		void CmdCheckForEnemies () {
			GameObject[] Blobs = GameObject.FindGameObjectsWithTag (tag);
			GameObject enemyTower;
			// For each Blob on the field
			for (var d = 0; d  < Blobs.Length; d++) {
				Blob blob = Blobs [d].GetComponent<Blob> ();
				// Skip my own blobs
				if (blob.towerName == towerName) {
					continue;
				} else {
					// Get the enemy tower if we don't know it already

					enemyTower = blob.tower;
					 // If the tower is in range, attack tower.
					if (Vector3.Distance (enemyTower.transform.position, transform.position) <= range) {
						Debug.Log ("Attacking Tower!");
						if (nextTime < Time.time) {

							enemyTower.GetComponent<Tower>().CmdDamageObject (damage);
							nextTime = Time.time + attackSpeed;
						}
					} // If tower is not in range and current blob belongs to enemy
					else {
						// Check if he's in range
						if (Vector3.Distance (Blobs [d].transform.position, transform.position) <= range) {
							if (nextTime < Time.time) {
								blob.CmdDamageObject (damage);
								nextTime = Time.time + attackSpeed;
								Debug.Log ("Found Enemy in range: " + Blobs [d].transform.name);
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
				StepMove ();
				CmdTransmitPosition ();
				CmdCheckForEnemies();
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
							NetworkIdentity ni = b.tower.GetComponent<NetworkIdentity> ();
							if (ni.isLocalPlayer) {
								// If so you can select the blob
								b.isSelected = true;

							}
						} else if (isSelected && isClient) {
							// A selected blob can be send to a destination
							syncDestination = GetDestination ();
							// Sends new destination to all client objects through tower
							tower.GetComponent<Tower> ().TransmitDestination (name, syncDestination);
							isSelected = false;
						} 
					}
				}
			}
		}

		// Sets destination, the rest is triggered from Update
		public void MoveTo(Vector3 location) {
			syncDestination = location;
		}
		// Make a single step towards the target location 
		private void StepMove () {
			Vector3 movement = syncDestination - transform.position;
			// Normalise the movement vector and make it proportional to the speed per second.
			movement = movement.normalized * speed * Time.deltaTime;
			// Move the player to it's current position plus the movement, using the rigidbody movement.
			transform.position = transform.position + movement;
			
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