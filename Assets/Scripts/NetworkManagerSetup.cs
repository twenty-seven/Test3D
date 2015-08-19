using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace BlobWars {
	public class NetworkManagerSetup : NetworkManager {
		private GameObject[] spawnPoints;
		// Use this for initialization
		void Start () {
			spawnPoints = GameObject.FindGameObjectsWithTag ("Respawn");

			//show IP on panel

			Text ipText = GameObject.Find ("ip-text").GetComponent<Text> ();
			ipText.text += " " + Network.player.ipAddress;
		}
		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{	GameObject player;
			// Ensure that client always spawns on client side, host always spawns on server side
			if (numPlayers >= 1) {
						player = (GameObject)GameObject.Instantiate(playerPrefab, spawnPoints[1].transform.position, Quaternion.identity);
						player.transform.SetParent(GameObject.Find("ImageTarget").transform);
			} else {
						player = (GameObject)GameObject.Instantiate(playerPrefab, spawnPoints[0].transform.position, Quaternion.identity);
						player.transform.SetParent(GameObject.Find("ImageTarget").transform);
				}

			NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
		}
		// Update is called once per frame
		void Update () {
		
		}

		//helper to start host via custom UI
		public void CreateHost(){
			StartHost ();
		}

		//helper to connect to server via custom UI
		public void CreateClient() {
			//set adress to what is typed in the field.

			string address = GameObject.Find ("GUI").transform.Find("Panel").FindChild("clientField").GetComponent<InputField>().text;
			//Transform clientField = transform.parent.FindChild ("clientField");
			if (address == null || address == "") {
				address = "localhost";
			}

			Debug.Log ("Address is: " + address);
			networkAddress = address;
			StartClient ();
		}
		
	}
}