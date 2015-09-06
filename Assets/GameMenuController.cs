using UnityEngine;
using System.Collections;

public class GameMenuController : MonoBehaviour {

	public Camera uiCamera;
	public Camera arCamera;
	private RectTransform disconnect;
	private RectTransform notification;
	private RectTransform selectButton;
	private RectTransform exitButton;
	public int offsetX;

	// Use this for initialization
	void Start () {
	
		/*disconnect = GameObject.Find ("Disconnect").GetComponent<RectTransform>();
		notification = GameObject.Find ("NPanel").GetComponent<RectTransform>();
		selectButton = GameObject.Find ("selBtn").GetComponent<RectTransform> ();
		exitButton = GameObject.Find ("exitBtn").GetComponent<RectTransform> ();
		Debug.Log ("Disconnect Button: " + disconnect);
		Debug.Log ("Motification: " + notification);
		Debug.Log ("Select Button: " + selectButton);
		Debug.Log ("Deselect Button: " + exitButton);
		offsetX = 50;
		//buttons = GameObject.Find ("ButtonPanel").transform;
*/
	}

	void Update() 
	{

		/*	int width = Screen.width;
			int height = Screen.height;
		int centerWidth =  width / 2;
		int centerHeight =  height / 2;
		Vector3 selectPosition = new Vector3 (centerWidth + offsetX, -150,0);

		Vector3 deselectPosition = new Vector3 (centerWidth + offsetX, -height,0);
		//Debug.Log (width + " x " + height);
		Debug.Log (selectPosition);
		Debug.Log (deselectPosition);
		Debug.Log (exitButton.anchoredPosition);
		exitButton.position = deselectPosition;
		disconnect.position = Vector3.zero;
			notification.position = Vector3.zero;
			//buttons.position = new Vector3(0,25,0);
*/


	}
}
