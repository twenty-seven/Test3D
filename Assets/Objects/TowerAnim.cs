using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TowerAnim : NetworkBehaviour {

	Animator break1Anim;
	Animator break2Anim;
	Animator break3Anim;
	Animator break4Anim;
	Animator break5Anim;
	Animator break6Anim;
	Animator break7Anim;
	Animator break8Anim;
	Animator break9Anim;
	Animator break10Anim;
	Animator break11Anim;
	Animator break12Anim;
	Animator break13Anim;
	Animator break14Anim;
	Animator break15Anim;
	Animator break16Anim;
	Animator break17Anim;
	Animator break18Anim;

	// Gets set through the corresponding blob/tower value
	BlobWars.HealthObject h;
	private float maxTowerHealth;
	private float currentTowerHealth;
	
	bool broken1 = false;
	bool broken2 = false;
	bool broken3 = false;
	bool broken4 = false;
	bool broken5 = false;
	bool broken6 = false;
	bool broken7 = false;
	bool broken8 = false; 
	bool broken9 = false;
	bool broken10 = false;
	bool broken11 = false;
	bool broken12 = false;
	bool broken13 = false; 
	bool broken14 = false;
	bool broken15 = false;
	bool broken16 = false;
	bool broken17 = false;
	bool broken18 = false;


	public Animator doorsAnim;
	[SyncVar]
	public bool doorsOpen = false;

	void Awake () {
		// Use transform to ensure they only find the objects of this specific tower.
		break1Anim = transform.Find("upperstone1Anim").GetComponent<Animator> ();
		break2Anim = transform.Find("upperstone2Anim").GetComponent<Animator> ();
		break3Anim = transform.Find("upperstone3Anim").GetComponent<Animator> ();
		break4Anim = transform.Find("upperstone4Anim").GetComponent<Animator> ();
		break5Anim = transform.Find("upperstone5Anim").GetComponent<Animator> ();
		break6Anim = transform.Find("lower_back_left_Anim").GetComponent<Animator> ();
		break7Anim = transform.Find("lower_back_right_Anim").GetComponent<Animator> ();
		break8Anim = transform.Find("lower_left_Anim").GetComponent<Animator> ();
		break9Anim = transform.Find("lower_right_back_Anim").GetComponent<Animator> ();
		break10Anim = transform.Find("lower_right_front_Anim").GetComponent<Animator> ();
		break11Anim = transform.Find("middle_chip_1_Anim").GetComponent<Animator> ();
		break12Anim = transform.Find("middle_chip_2_Anim").GetComponent<Animator> ();
		break13Anim = transform.Find("middle_chip_3_Anim").GetComponent<Animator> ();
		break14Anim = transform.Find("middle_chip_4_Anim").GetComponent<Animator> ();
		break15Anim = transform.Find("middle_chip_5_Anim").GetComponent<Animator> ();
		break16Anim = transform.Find("middle_chip_6_Anim").GetComponent<Animator> ();
		break17Anim = transform.Find("middle_chip_7_Anim").GetComponent<Animator> ();
		break18Anim = transform.Find("log_Anim").GetComponent<Animator> ();

		//doorsAnim = transform.Find("doors_Anim").GetComponent<Animator> ();
		doorsAnim.SetBool("doorsOpen", false);
		h = GetComponent<BlobWars.HealthObject> ();
		maxTowerHealth = h.maxHealth;
		currentTowerHealth = h.maxHealth;


	}

	bool opened = false;
	public void openDoors() {

		if (!opened) {
			print ("open");
			doorsAnim.SetBool ("doorsOpen", true);
			opened = true;
		}
	}

	public void closeDoors() {
		if (opened) {
			print ("close");
			doorsAnim.SetBool ("doorsOpen", false);
			opened = false;
		}
	}


	void Update () {
			currentTowerHealth = h.currentHealth;
			float percentageHealth = currentTowerHealth / maxTowerHealth;
			if (!broken1 && percentageHealth < 0.95) {
				break1Anim.SetTrigger ("break");
				broken1 = true;
			}
			if (!broken2 && percentageHealth < 0.9) {
				break6Anim.SetTrigger ("break");
				broken2 = true;
			}
			if (!broken3 && percentageHealth < 0.85) {
				break12Anim.SetTrigger ("break");
				broken3 = true;
			}
			if (!broken4 && percentageHealth < 0.80) {
				break4Anim.SetTrigger ("break");
				broken4 = true;
			}
			if (!broken5 && percentageHealth < 0.75) {
				break15Anim.SetTrigger ("break");
				broken5 = true;
			}
			if (!broken6 && percentageHealth < 0.70) {
				break16Anim.SetTrigger ("break");
				broken6 = true;
			}
			if (!broken7 && percentageHealth < 0.65) {
				break10Anim.SetTrigger ("break");
				broken7 = true;
			}
			if (!broken8 && percentageHealth < 0.6) {
				break14Anim.SetTrigger ("break");
				broken8 = true;
			}
			if (!broken9 && percentageHealth < 0.55) {
				break2Anim.SetTrigger ("break");		
				broken9 = true;	
			}
			if (!broken10 && percentageHealth < 0.5) {
				break11Anim.SetTrigger ("break");
				broken10 = true;
			}
			if (!broken11 && percentageHealth < 0.45) {
				break8Anim.SetTrigger ("break");
				broken11 = true;
			}
			if (!broken12 && percentageHealth < 0.4) {
				break9Anim.SetTrigger ("break");
				broken12 = true;
			}
			if (!broken13 && percentageHealth < 0.35) {
				break13Anim.SetTrigger ("break");
				broken13 = true;
			}
			if (!broken14 && percentageHealth < 0.3) {
				break3Anim.SetTrigger ("break");
				broken14 = true;
			}
			if (!broken15 && percentageHealth < 0.25) {
				break7Anim.SetTrigger ("break");
				broken15 = true;
			}
			if (!broken16 && percentageHealth < 0.2) {
				break5Anim.SetTrigger ("break");
				broken16 = true;
			}
			if (!broken17 && percentageHealth < 0.15) {
				break17Anim.SetTrigger ("break");
				broken17 = true;
			}
			if (!broken18 && percentageHealth < 0.1) {
				break18Anim.SetTrigger ("break");
				broken18 = true;
			}

			/*
		break1Anim.SetTrigger("break");
		break2Anim.SetTrigger("break");
		break3Anim.SetTrigger("break");
		break4Anim.SetTrigger("break");
		break5Anim.SetTrigger("break");
		break6Anim.SetTrigger("break");
		break7Anim.SetTrigger("break");
		break8Anim.SetTrigger("break");
		break9Anim.SetTrigger("break");
		break10Anim.SetTrigger("break");
		break11Anim.SetTrigger("break");
		break12Anim.SetTrigger("break");
		break13Anim.SetTrigger("break");
		break14Anim.SetTrigger("break");
		break15Anim.SetTrigger("break");
		break16Anim.SetTrigger("break");
		break17Anim.SetTrigger("break");
		break18Anim.SetTrigger("break");
		*/
			/*if (doorsOpen) {

				openDoors ();
			} else {
				closeDoors ();
			}*/
	
	}
}
