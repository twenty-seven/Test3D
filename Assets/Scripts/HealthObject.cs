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

		// Calculates Damage on the Server and distributes it accordingly
		[Command]
		public void CmdDamageObject(int damage) {

			Debug.Log (currentHealth);
			if (currentHealth - damage > 0) {
				currentHealth -= damage;
				CmdTransmitHealth();
			} else {
				
				// TODO: Trigger animation before destruction?
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
				g.GetComponent<TowerAnim>().currentTowerHealth = 0;
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