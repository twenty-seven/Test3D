using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace BlobWars {
	/**
	 * Handles general health features, also provides the NetworkBehaviour class
	 */
	public class HealthObject : NetworkBehaviour {

		// Should be set by prefab
		public float maxHealth;
		// Should trigger animation		
		private bool alive = true;
		private Animator anim;
		// The current health
		[SyncVar] public float currentHealth;

		//audio
		protected AudioSource aSource;
		public AudioClip getHitAudio;
		public AudioClip dieAudio;

		public void Start() {
			aSource = GetComponent<AudioSource> ();
			currentHealth = maxHealth;

		}

		// Calculates Damage on the Server and distributes it accordingly
		[Command]
		public void CmdDamageObject(int damage) {
			Debug.Log ("I'm being damaged " + damage + " points of " + currentHealth);
			if (currentHealth - damage > 0) {
				aSource.PlayOneShot(getHitAudio);
				currentHealth -= damage;
				//CmdTransmitHealth();
			} else {
				// Ignore objects that are already dead
				if (currentHealth > 0) {
					currentHealth -= damage;
					aSource.PlayOneShot(dieAudio);
					// Animation of dying object gets triggered in SlimeAnim, if the health of Healthobject drops below or equal to 0
					CmdKillObject(name);
				}
			}
		}

		[Command]
		void CmdKillObject(string uid) {
			GameObject g = GameObject.Find (uid);
			if (g != null && g.GetComponent<Blob> () != null) {
				Tower t = g.GetComponent<Blob> ().tower.GetComponent<Tower>();
				t.numSoldiers = t.numSoldiers - 1;
				StartCoroutine(WaitForDeathAnimation(g));	
			} else if (g.GetComponent<Tower> () != null) {
				StartCoroutine(WaitForDeathAnimation(g));
				// End Game here GameController.
			}

			
			
		}
		// Wait 2 Seconds before destroying the killed object
		// so the animation still is visible
		IEnumerator WaitForDeathAnimation(GameObject g) {
			yield return new WaitForSeconds(2);
			Destroy (g);
		}
		// Send Health from Server to Object
		[Command]
		void CmdTransmitHealth () {
			TransmitHealth (currentHealth);
		}
		// Set Health on local object
		[ClientCallback]
		void TransmitHealth (float newHealth) {
			if (isLocalPlayer) {
				currentHealth =  newHealth;
			}
		}

		// is still alive
		public bool isAlive() {
			return (alive = (currentHealth > 0));
		}
	}	
}