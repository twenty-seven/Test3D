using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

namespace BlobWars {
	// This class controls the Menu set up in the MenuScene, it deals with 
	// the creation of new games and quitting the game.
	public class MenuController : MonoBehaviour {
		// The actual menu panel as transform
		private Transform menu;
		// The Input Field of the Server Address
		private InputField serverIP;
		// The Input Field of the Server Port
		private InputField serverPort;
		// The panel to display errors, contains a Text element
		private GameObject errorField;
		// The Text element to display my current IP 
		private Text myIP;
		// The Address field that is displayed when the client/host tries to connect.
		public GameObject clientWaitingAddress;
		public GameObject hostWaitingAddress;
		// The Network Manager Prefab
		public GameObject networkManagerPrefab;


		// The actual Network Manager Obejct that was spawned
		private NetworkManager networkManager;



		// Use this for initialization
		void Start () {
			// Gather Information
			menu = transform;
			Transform buttons = menu.FindChild("Interaction").FindChild ("Buttons").transform;
			serverIP = buttons.FindChild ("ClientField").GetComponent<InputField>();
			serverPort = buttons.FindChild ("PortField").GetComponent<InputField>();
			myIP = menu.FindChild("Interaction").FindChild ("Labels").transform.FindChild ("IPAddress").GetComponent<Text> ();
			errorField = menu.FindChild ("ErrorField").gameObject;

			// Use GameObject since it's not a child of menu
			GameObject nm = GameObject.Find ("NetworkManager");

			// Avoid running multiple managers when switching scenes.
			if (null == nm) {
				if (Debug.isDebugBuild) {
					Debug.Log ("Creating NetworkManager Singleton");
				}
				nm = (GameObject)Instantiate (networkManagerPrefab, Vector3.zero, Quaternion.identity);

			}
			networkManager = nm.GetComponent<NetworkManager>();
			networkManager.name = "NetworkManager";
			SetIPLabel(Network.player.ipAddress);
		}
		// Gets the input from the server ip and parses it via regex for a valid IP address
		private string GetIP() {
			string result = serverIP.text;
			if (result == "") {
				result = serverIP.transform.FindChild ("Placeholder").GetComponent<Text> ().text;
			}
			if (result == "localhost") {
				return result;
			} else {
				if (Regex.IsMatch(result,"^\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$")) {
					Debug.Log ("Regex Passed");
					return result;
				} else {
					DisplayError("'" + result + "' is not a valid address!");
					return null;
				}

			}
		}
		// Gets the port from the port input aFnd parses it to an int
		private int GetPort() {
			string value = serverPort.transform.FindChild ("Placeholder").GetComponent<Text> ().text;
			int result = -1;
			if (serverPort.text != "") {
				value = serverPort.text;
			}
			try {
				int.TryParse (value, out result);
			} catch (FormatException e) {
				DisplayError ("'" + value + "' is not a valid port number!");
			}
			return result;
		}

		// sets ip label
		public void SetIPLabel(string addr) {
			myIP.text = addr;
		}


		// Wrapper function that sets the label and loads appropriate menu, 
		// before calling network managers createHost, which calls StartHost
		public void StartHost() {
			string ip = GetIP ();
			int port = GetPort ();
			if ( hostWaitingAddress == null) {
				hostWaitingAddress = GameObject.Find ("ClientWaiting");
			}
			if (ip == null || ip == "" || port == -1) {
				if (Debug.isDebugBuild) {
					Debug.Log ("Creating failed due to uncommon port ("+port+")or ip ("+ip+") ");
				}
				return;
			}
			hostWaitingAddress.GetComponent<Text> ().text = ip + ":" + port;
			networkManager.GetComponent<NetworkManagerScript> ().CreateHost (ip,port);
		}

		// Wrapper function that sets the label and loads appropriate menu, 
		// before calling network managers createClient, which calls StartClient
		public void StartClient() {
			if (clientWaitingAddress == null) {
				clientWaitingAddress = GameObject.Find ("ClientWaiting");
			}

			string ip = GetIP ();
			int port = GetPort ();

			if (ip == null || ip == "" || port == -1) {
				if (Debug.isDebugBuild) {
					Debug.Log ("Creating failed due to uncommon port ("+port+")or ip ("+ip+") ");
				}
				return;
			}
			clientWaitingAddress.GetComponent<Text> ().text = ip + ":" + port;

			networkManager.GetComponent<NetworkManagerScript> ().CreateClient (ip, port);

		}

		// Ends Game after disconnecting from network
		public void QuitGame() {
			networkManager.GetComponent<NetworkManagerScript> ().DisconnectAll ();
			Application.Quit ();
		}


		// Shows the main menu with an additional field, which display information about 
		// an error that occured.
		public void DisplayError(string message) {

			gameObject.SetActive (true);
			GameObject.Find ("ClientWaiting").SetActive (false);
			errorField.SetActive (true);
			errorField.transform.FindChild ("ErrorText").GetComponent<Text> ().text = message;


		}
	}
}