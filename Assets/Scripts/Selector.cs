using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace BlobWars {
	public class Selector : MonoBehaviour {

		//to allow selection of object in editor
		public GameObject selectObject;
		// The unique ID of the tower the selector belongs to
		public string towerUID;
		// The actual Tower object, gets set automatically by UID
		private  Tower tower;
		// The NetworkIdentity of the tower, used to get the .isLocalPlayer property.
		private NetworkIdentity tNI;
		// The last Object that was selected.
		private GameObject lastSelection;

		// Gets spawned in the Tower.Start() methode.
		void Start () {
			transform.name = towerUID + ".select";
			tower = GameObject.Find (towerUID).GetComponent<Tower> ();
			tNI = tower.GetComponent<NetworkIdentity> ();
		}
		
		// Update is called once per frame
		void Update () {
			// If we're not on the server
			if (tNI.isLocalPlayer) {
				// Move the selector ball correspondigly 
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				//get mouse position on plane and hit
				if (Physics.Raycast (ray, out hit)) {
					Vector3 location = new Vector3 (hit.point.x, hit.point.y, hit.point.z);

					if (hit.transform.GetComponent<Blob>() != null) {
						location.y += 10;
					}
					transform.position = location;
				}
				//trigger slecetion on click!
				if (Input.GetKey ("mouse 0")) {
					TriggerSelect ();
				}
			}

		}
		// If someone clicked
		void TriggerSelect() {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// Throw a ray
			if (Physics.Raycast (ray, out hit)) {
				Blob b = hit.transform.GetComponent<Blob> ();
				// If we hit a Blob
				if (b != null && b.tower != null) {
					// mark it
					lastSelection = b.gameObject;
				} else {
					// If we didn't hit a blob and selected another blob already
					if (null != lastSelection) {
						// Send Blob on his way
						tower.TransmitDestination(lastSelection.name, new Vector3(hit.point.x,lastSelection.transform.position.y,hit.point.z));
					}
				}

			}
		}}
}
