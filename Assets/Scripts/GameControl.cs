using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameControl : MonoBehaviour {

	GameObject winUI;

	// Use this for initialization
	void Start () {
		winUI = GameObject.Find ("WinScreen");
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void WinGame(bool win){
		GUIText wintxt = GameObject.Find ("WinText").GetComponent<GUIText> ();

		if (win) {
			wintxt.text = "VICTORY!";
			wintxt.color = Color.green;
		} else {
			wintxt.text = "DEFEAT!";
			wintxt.color = Color.red;
		}

		winUI.SetActive (true);
		GameObject.Find ("Controls").SetActive (false);
	}

	public void QuitGame() {
		NetworkManager nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		if (nm.isNetworkActive) {
			if(Network.isClient) {
				nm.StopClient();
			} else {
				nm.StopHost();
			}
		} else {
			Application.Quit ();
		}
	}
}
