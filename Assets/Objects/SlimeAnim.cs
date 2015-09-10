using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SlimeAnim : NetworkBehaviour {

	Animator anim;

	public GameObject[] deactivateOnDeathObjects;
	public ParticleSystem deathParticleSystem;
	[SyncVar]
	public bool isWalking = false;
	[SyncVar]
	public bool doAttack = false;
	private BlobWars.Blob script;
	void Start () {
		anim = GetComponent<Animator> ();
		script = GetComponentInParent<BlobWars.Blob> ();
	}

	void Update () {
		if (script == null || script.tower.GetComponent<NetworkIdentity>().isClient
		    || script.tower.GetComponent<NetworkIdentity>().isServer) {
			if (doAttack) {
				Debug.Log ("Attacking");
				anim.SetTrigger ("Attack");
				doAttack = false;
			}
			anim.SetBool ("IsWalking", isWalking);

			if (script != null && script.currentHealth <= 0) {
				for (int i = 0; i < deactivateOnDeathObjects.Length; i++) {
					deactivateOnDeathObjects [i].SetActive (false);
				}
				deathParticleSystem.Play ();
			}
		}
	}
}
