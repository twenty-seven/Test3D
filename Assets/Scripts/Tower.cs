using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BlobWars {
	public class Tower : HealthObject {
		// Soldier Prefabs that are used to spawn soldiers
		public GameObject Fighter, Ranged, Artillery, Selector;
		private GameObject selector;
		[SyncVar]
		public string uid;
		// Maximum Number of Soldiers
		public int maxSoldiers = 2;
		// Current Number of Soldiers
		[SyncVar]
		public int numSoldiers = 0;
		// Variable for position
		[SyncVar]
		public Vector3 syncPos;
		// Variable for rotation
		[SyncVar] 
		public Quaternion syncRot;
		// REMOVE TODO
		private float spawnDelay;

		//Audio
		public AudioClip openDoorAudio;
		
		void Start () {
			base.Start ();
			currentHealth = maxHealth;
			// Setup SyncPosition 
			syncPos = transform.position;
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
			spawnDelay = Time.time + 5f;
			selector = (GameObject) Instantiate(Selector, transform.position, Quaternion.identity);
			selector.transform.parent = GameObject.Find("ImageTarget").transform;
			selector.GetComponent<Selector> ().towerUID = transform.name;

			Button selectBtn = GameObject.Find("selBtn").GetComponent<Button>();
			selectBtn.interactable = true;
			
			//add selection trigger function to the button.
			selectBtn.onClick.AddListener (()=>{
				selector.GetComponent<Selector>().TriggerSelect();
			});
		}
		
		//TODO: remove this var when done testing
		private int soldierTypeTester = 0;
		// Update is called once per frame
		void FixedUpdate () {
			//TODO: change to actual spawning behaviour ... this is for testing.
			//spawn soldier when there is space ... go through types :)
			soldierTypeTester++;
			if (soldierTypeTester > 2) {
				soldierTypeTester = 0;
			}
			if (Time.time > spawnDelay) {
				spawnSoldier(soldierTypeTester);
				spawnDelay = Time.time + 5f;
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

		// Client call to spawn a soldier on the server
		[Client]
		void spawnSoldier(int type) {
			if (numSoldiers < maxSoldiers) {
				GetComponent<TowerAnim>().doorsOpen = true;
				// Get unique name for blob
				string blobName = uid + "." + 0;
				for (int i = 0; i < maxSoldiers*5; i++) {
					blobName = uid + "." + i;
					if (GameObject.Find (blobName) == null) {
						break;
					}
				}
				// Server call with towername, blob id and spawn position
				GetComponent<TowerAnim> ().openDoors();
				CmdSpawnSoldier (type, uid, blobName, transform.position);

			}
		}

		// Spawns a soldier on the server
		[Command]
		void CmdSpawnSoldier(int type, string towerName, string blobName, Vector3 location) {
			// Create, name and spawn the object on the server
			GameObject prefab = Fighter;
			switch (type) {
			case 1:
				prefab = Ranged;
				break;
			case 2: 
				prefab = Artillery;
				break;
			}
			GameObject blob = (GameObject)Instantiate (prefab, location, Quaternion.identity);
			blob.GetComponent<Blob> ().towerName = towerName;
			blob.transform.SetParent (GameObject.Find ("ImageTarget").transform);
			numSoldiers++;
			Debug.Log ("Spawning " + blobName + " of tower " + towerName + " at " + location);

			NetworkServer.Spawn (blob);
			
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
