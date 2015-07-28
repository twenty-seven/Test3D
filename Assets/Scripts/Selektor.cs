using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace BlobWars {
	public class Selektor : MonoBehaviour {

		//to allow selection of object in editor
		public GameObject selectObject;

		private GameObject selector;
		private Blob selectedUnit;
		private Blob possibleSelection;
		private Vector3 lastRayHitPoint;

		// Use this for initialization
		void Start () {
			selector = (GameObject)Instantiate (selectObject, transform.position, Quaternion.identity);
		}
		
		// Update is called once per frame
		void Update () {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			//get mouse position on plane and hit
			if (Physics.Raycast (ray, out hit)) {
				Blob b = hit.transform.GetComponent<Blob> ();
				if (b != null && b.tower != null) {
					selector.transform.position = new Vector3(b.transform.position.x ,20 , b.transform.position.z);

					NetworkIdentity ni = b.tower.GetComponent<NetworkIdentity> ();
					if (ni.isLocalPlayer) {
						possibleSelection = b;
					}

				} else {
					possibleSelection = null;
					selector.transform.position = new Vector3(hit.point.x, 1, hit.point.z);
				}
				lastRayHitPoint = hit.point;
			}

			//trigger slecetion on click!
			if(Input.GetKey ("mouse 0")) {
				TriggerSelect();
			}

		}

		void TriggerSelect() {
			if (possibleSelection != null) {
				if(selectedUnit != null) {
					selectedUnit.isSelected = false;
				}
				// If so you can select the blob
				selectedUnit = possibleSelection;
				possibleSelection.isSelected = true;

			} else if (selectedUnit != null) {
				//trigger movement
				selectedUnit.MoveTo(lastRayHitPoint);
			}

		}
	}
}
