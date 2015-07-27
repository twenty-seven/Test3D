using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace BlobWars {
	public class Selektor : MonoBehaviour {

		//to allow selection of object in editor
		public GameObject selectObject;


		private GameObject selector;

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
				if (b != null && b.tower != null && b.tower.GetComponent<NetworkIdentity>().isLocalPlayer) {
					selector.transform.position = new Vector3(b.transform.position.x ,20 , b.transform.position.z);
				} else {
					selector.transform.position = new Vector3(hit.point.x, 2, hit.point.z);
				}
			}

		}
	}
}
