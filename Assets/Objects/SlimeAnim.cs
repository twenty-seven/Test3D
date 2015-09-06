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
	[SyncVar]
	public bool die = false;

	void Start () {
		anim = GetComponent<Animator> ();
	}

	void Update () {
		if (doAttack) {
			anim.SetTrigger("Attack");
			doAttack = false;
		}
		anim.SetBool ("IsWalking", isWalking);

		if (die) {
			for(int i = 0; i < deactivateOnDeathObjects.Length; i++) {
				deactivateOnDeathObjects[i].SetActive(false);
			}
			deathParticleSystem.Play();
			die = false;
		}
	}
}
