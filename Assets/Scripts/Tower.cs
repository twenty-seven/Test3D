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
			GetComponent<HealthObject>().currentHealth = GetComponent<HealthObject>().maxHealth;
			// Create 'unique' name for tower
			uid = "Player" + GetComponent<NetworkIdentity>().netId;
			gameObject.transform.name = uid;
			//Camera serverCam = GameObject.Find ("Server Camera").GetComponent<Camera>();
			//Camera clientCam = GameObject.Find ("Client Camera").GetComponent<Camera>();
			// Handle Cameras for host
			/*if (isServer && isClient) {
				if (serverCam != null) {
					serverCam.enabled = true;
				}
				if (clientCam != null) {
					clientCam.enabled = false;
				}
			} else { // for client
				if (serverCam != null) {
					serverCam.enabled = false;
				}
				if (clientCam != null) {
					clientCam.enabled = true;
				}
			}*/
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
			} else {
				Debug.Log ("Remote Tower Spawned.");
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

						GetComponent<Animator>().SetBool("DoorsOpen",true);
					spawnDelay = Time.time + spawnDelayValue;
				}

			}

			// If you click on your tower, a new unit spawns
			// No Position synchronization necessary
			/*if (Input.GetKeyDown("w")) {
				//if(hitMe()) {
				if (isLocalPlayer && Time.time > spawnDelay){
					
					spawnSoldier(0);
					spawnDelay = Time.time + 2f;
				}
				//}
			}
			if (Input.GetKeyDown("a")) {
				//if(hitMe()) {
				if (isLocalPlayer && Time.time > spawnDelay){
					
					spawnSoldier(1);
					spawnDelay = Time.time + 2f;
				}
			}
			if (Input.GetKeyDown("s")) {
				//if(hitMe()) {
				if (isLocalPlayer && Time.time > spawnDelay){
					
					spawnSoldier(2);
					spawnDelay = Time.time + 2f;
				}
				//}
			}*/
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

				NetworkServer.Spawn (blob);
			}
			
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
		// Did that raycast hit me?
		bool hitMe () {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				if (hit.transform.CompareTag("Player")) {
					if (hit.transform.GetComponent<NetworkIdentity>().isLocalPlayer) {
						return true;
					}
				}
			}
			return false;
			
		}
	}
}
