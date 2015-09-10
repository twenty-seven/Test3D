using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BlobWars {
	public class Tower : HealthObject {
		// Soldier Prefabs that are used to spawn soldiers
		public GameObject Fighter, Ranged, Artillery, Selector;
		private GameObject selector;
		public string uid;
		// Maximum Number of Soldiers
		public int maxSoldiers = 1;
		// Current Number of Soldiers
		[SyncVar]
		public int numSoldiers = 0;
		// REMOVE TODO
		private float spawnDelay;
		private float spawnDelayValue = 5f;

		//Audio
		public AudioClip openDoorAudio;
		void Start () {
			currentHealth = maxHealth;
			GetComponent<HealthObject>().currentHealth = GetComponent<HealthObject>().maxHealth;
			// Create 'unique' name for tower
			uid = "Player" + GetComponent<NetworkIdentity>().netId;
			gameObject.transform.name = uid;
			spawnDelay = Time.time + spawnDelayValue;
		 if (isLocalPlayer) {
			Debug.Log ("Local Tower Spawned." + transform.name);
			selector = (GameObject)Instantiate (Selector, transform.position, Quaternion.identity);
			selector.GetComponent<Selector> ().towerUID = transform.name;

			GameObject imgTarget = GameObject.Find ("ImageTarget");
			if (imgTarget != null) {
				selector.transform.parent = imgTarget.transform;
			}

			
			GameObject selectBtn = GameObject.Find ("selBtn");
			if (selectBtn != null) {
				selectBtn.GetComponent<Button> ().interactable = true;
				// add selection trigger function to the button.
				selectBtn.GetComponent<Button> ().onClick.AddListener (() => {
					selector.GetComponent<Selector> ().TriggerSelect ();
				});
			}
		}
			base.Start ();
		}
		
		//TODO: remove this var when done testing
		private int soldierTypeTester = 0;
		// Update is called once per frame
		void FixedUpdate () {
			//TODO: change to actual spawning behaviour ... this is for testing.
			//spawn soldier when there is space ... go through types :)
			if (isServer) {
				soldierTypeTester++;
				if (soldierTypeTester > 2) {
					soldierTypeTester = 0;
				}
				if (Time.time > spawnDelay) {
					CmdSpawnSoldier (soldierTypeTester, transform.position);

					GetComponent<TowerAnim>().openDoors();
					spawnDelay = Time.time + spawnDelayValue;
				}

			}
		}

		// Spawns a soldier on the server
		[Command]
		void CmdSpawnSoldier(int type, Vector3 location) {
			if (numSoldiers < maxSoldiers) {
				string blobName = uid + "." + 0;
				for (int i = 0; i < maxSoldiers*5; i++) {
					blobName = uid + "." + i;
					if (GameObject.Find (blobName) == null) {
						break;
					}
				}
				// Create, name and spawn the object on the server
				GameObject prefab = Fighter;
				Debug.Log ("Spawning Soldier on Server");
				switch (type) {
				case 1:
					prefab = Ranged;
					break;
				case 2: 
					prefab = Artillery;
					break;
				}
				GameObject blob = (GameObject)Instantiate (prefab, location, Quaternion.identity);
				Debug.Log ("Spawning " + blobName + " of tower " + uid + " at " + location);
				blob.GetComponent<Blob> ().towerName = uid;
				GameObject imgTarget = GameObject.Find ("ImageTarget");
				if (imgTarget != null) {
					blob.transform.SetParent (imgTarget.transform);
				}
				numSoldiers++;
				GetComponent<TowerAnim>().doorsOpen = true;
				NetworkServer.Spawn (blob);
			}
			
		}
		void triggerDoor() {
			//GetComponent<Animator> ().SetBool("DoorsOpen",true);
			GetComponent<TowerAnim> ().openDoors ();
		}
		// In case a blob changes it's destination, the tower is used to 
		// inform the Server about the changes
		[Client]
		public void TransmitDestination (string blobName, Vector3 destination) {
			CmdTransmitDestination (blobName, destination);
		}
		// Server gets changed object an set different location
		[Command]
		void CmdTransmitDestination(string blobName, Vector3 destination) {
			GameObject.Find (blobName).GetComponent<Blob> ().MoveTo (destination);
		}
	}
}
