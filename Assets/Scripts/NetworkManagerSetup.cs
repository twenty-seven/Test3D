using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
namespace BlobWars {
	public class NetworkManagerSetup : NetworkManager {
		private GameObject[] spawnPoints;
		// Use this for initialization
		void Start () {
			spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
		}

		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{	GameObject player;
			// Ensure that client always spawns on client side, host always spawns on server side
			if (numPlayers >= 1) {
						player = (GameObject)GameObject.Instantiate(playerPrefab, spawnPoints[1].transform.position, Quaternion.identity);
			} else {
						player = (GameObject)GameObject.Instantiate(playerPrefab, spawnPoints[0].transform.position, Quaternion.identity);
				}

			NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
		}
		// Update is called once per frame
		void Update () {
		
		}
	}
}