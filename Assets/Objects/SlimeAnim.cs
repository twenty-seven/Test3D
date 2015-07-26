using UnityEngine;
using System.Collections;

public class SlimeAnim : MonoBehaviour {

	Animator anim;

	public GameObject[] deactivateOnDeathObjects;
	public ParticleSystem deathParticleSystem;

	public bool isWalking = false;
	public bool doAttack = false;
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
