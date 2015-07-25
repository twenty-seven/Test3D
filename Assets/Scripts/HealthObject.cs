using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace BlobWars {
	/**
	 * Handles general health features, also provides the NetworkBehaviour class
	 */
	public class HealthObject : NetworkBehaviour {

		// Should be set by prefab
		public int maxHealth = 150;
		// Should trigger animation		
		private bool alive = true;
		// The current health
		[SyncVar] public int currentHealth;

		// Calculates Damage on the Server and distributes it accordingly
		[Command]
		public void CmdDamageObject(int damage) {
			Debug.Log (currentHealth);
			if (currentHealth - damage > 0) {
				currentHealth -= damage;
				CmdTransmitHealth();
			} else {
				alive = false;
				Destroy(gameObject);
				// TODO: Trigger animation before destruction?
			}
		}
		// Send Health from Server to Object
		[Command]
		void CmdTransmitHealth () {
			TransmitHealth (currentHealth);
		}
		// Set Health on local object
		[ClientCallback]
		void TransmitHealth (int newHealth) {
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