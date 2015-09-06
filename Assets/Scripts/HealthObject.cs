using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace BlobWars {
	/**
	 * Handles general health features, also provides the NetworkBehaviour class
	 */
	public class HealthObject : NetworkBehaviour {

		// Should be set by prefab
		public float maxHealth = 150;
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

		}

		// Calculates Damage on the Server and distributes it accordingly
		[Command]
		public void CmdDamageObject(int damage) {

			Debug.Log (currentHealth);
			if (currentHealth - damage > 0) {
				aSource.PlayOneShot(getHitAudio);
				currentHealth -= damage;
				//CmdTransmitHealth();
			} else {
				aSource.PlayOneShot(dieAudio);

				// TODO: Trigger animation before destruction? || Gets triggerd in CmdKillObject
				killObject(name);
			}
		}
		[Client]
		void killObject(string uid) {
			CmdKillObject (uid);

		}
		[Command]
		void CmdKillObject(string uid) {
			GameObject g = GameObject.Find (uid);
			if (g != null && g.GetComponent<Blob> () != null) {

				g.GetComponent<SlimeAnim>().die = true;

				Tower t = g.GetComponent<Blob> ().tower.GetComponent<Tower>();
				t.numSoldiers = t.numSoldiers - 1;
				Destroy (g);	
			} else if (g.GetComponent<Tower> () != null) {
				Destroy (g);
				// End Game
			}

			
			
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