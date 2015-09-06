using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using UnityEngine.Networking.NetworkSystem;


namespace BlobWars {
	public class NetworkManagerScript : NetworkManager {

		// The image target of the QR Camera thing
		private Transform imageTarget;
		// The actual NetworkManager spawned with the gameobject
		private NetworkManager manager;

		void Start () {
			manager = GetComponent<NetworkManager> ();
		}

		void Update() {
		}

		// Starts the host of a new network game
		// and makes sure it's working.
		public override NetworkClient StartHost() {
			if (Debug.isDebugBuild) {
				Debug.Log ("Starting Host");
			}
			NetworkClient test;
			try {
				 test = base.StartHost();
				if (null == test) throw new NullReferenceException();
			} catch (NullReferenceException e){
				Debug.Log ("Host Startup failed");
				Application.LoadLevel("MenuScene");
				return null;
			}
			return test;
		}

		// Make sure the client disconnects if the server loses connection
		public override void OnServerDisconnect (NetworkConnection conn) {
			Debug.Log ("Server Disconnect");
			DisconnectAll ();
		}

		// Close all connections, e.g. when exiting or aborting a game
		public void DisconnectAll () {

			if (manager == null) 
				manager = GameObject.Find ("NetworkManager").GetComponent<NetworkManager> ();
			if (manager.isNetworkActive) {
				if (manager.client != null)
					manager.StopClient ();
				manager.StopHost ();
				Network.Disconnect ();
			}
		} 

		// called when a client connects 
		public override void OnServerConnect(NetworkConnection conn) {
			Debug.Log ("Client connected");
			base.OnServerConnect (conn);
		}

		public override void OnClientConnect(NetworkConnection conn) {
			Debug.Log ("Server connected");
			base.OnClientConnect (conn);
		}

		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{	GameObject player;
			Debug.Log ("Adding Player");
			// Ensure that client always spawns on client side, host always spawns on server side
			player = (GameObject)GameObject.Instantiate(playerPrefab, manager.GetStartPosition().transform.position, Quaternion.identity);
			player.transform.SetParent(imageTarget);

			//GameObject.Find ("Notification").GetComponent<Text> ().text = "A new Player joined the game.";
			//GameObject.Find ("NPanel").SetActive (true);
			//StartCoroutine (HideNotification ());
			NetworkServer.AddPlayerForConnection (conn, player, playerControllerId);
		}

		IEnumerator HideNotification() {
			yield return new WaitForSeconds (3);
			GameObject.Find ("NPanel").SetActive (false);
		}


		// wrapper function that calls start host, call this one to use custom ip and port. 
		public void CreateHost(string ip = "127.0.0.1", int port = 777){
			if (Debug.isDebugBuild) {
				Debug.Log ("Creating Host");
			}
			try {
				NetworkManager.singleton.networkPort = port;
				NetworkManager.singleton.networkAddress = ip;
				base.StartHost();
			} catch (UnityException e){
				GameObject.Find ("MainMenu").GetComponent<MenuController>().DisplayError("Could not Start the Host");
				Debug.Log ("Log: " + e.Message);
			}
		}

		// wrapper function that calls start host, call this one to use custom ip and port. 
		public void CreateClient(string ip = "localhost", int port = 7777) {
			//set adress to what is typed in the field.
			try {
				Debug.Log ("Creating  Client");
				networkPort = port;
				networkAddress = ip;
				base.StartClient();
				Debug.Log (System.DateTime.Now);
			} catch (TimeoutException  e) {
				// Debug.Log ("Timeout");
				// Application.LoadLevel("MenuScene");
			}

		}


		
	}
}