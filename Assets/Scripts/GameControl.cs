using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameControl : MonoBehaviour {

	GameObject networkUI;

	// Use this for initialization
	void Start () {
		networkUI = GameObject.Find ("Panel");
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void QuitGame() {
		NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		if (nm.isNetworkActive) {
			if(Network.isClient) {
				nm.StopClient();
			} else {
				networkUI.SetActive(true);
				nm.StopHost();
			}
		} else {
			Application.Quit ();
		}
	}
}
