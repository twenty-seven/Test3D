using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

	// velocity
	float velocity = 50;
	Vector3 startPosition;
	// Sync Position
	[SyncVar]
	Vector3 syncPos;
	// Target Position
	Vector3 syncTargetPos = new Vector3 (10,0,10);
	float attackSpeed = 2;

	float angle = 75;
	// Use this for initialization
	void Start () {
		startPosition = transform.position;
		syncPos = transform.position;

		float distance = Vector3.Distance (syncPos, syncTargetPos);

		return;
		//float v2 = Mathf.Pow (velocity, 2);
		float g = Physics.gravity.magnitude;
		float x = distance;
		float y = transform.position.y;
		//float innerSqrt = Mathf.Pow (v2, 2) - g * (g * Mathf.Pow (x, 2) + 2 * y * Mathf.Pow (velocity, 2));
		//if (innerSqrt < 0) {
			Debug.LogWarning("The Velocity of the bullet is not high enough!");
			Destroy(gameObject);
		//}

		//Debug.Log (innerSqrt);
		//angle = Mathf.Atan((velocity + Mathf.Sqrt(Mathf.Pow (velocity,4) - Physics.gravity.magnitude * (Physics.gravity.magnitude * Mathf.Pow (distance,2) + 2 *transform.position.y * Mathf.Pow (velocity,2)))));
		Debug.Log ("Angle : " + angle);
		
		
	}
	
	// Update is called once per frame
	void Update () {
		if (startPosition != syncTargetPos) {
			//Debug.Log (transform.position + " target: " + syncTargetPos);
			float distance = Vector3.Distance (syncPos, syncTargetPos);
			Debug.Log (velocity);
			float x = velocity * Mathf.Cos(angle);
			float y = -velocity * Mathf.Sin(angle)  - Time.deltaTime * Physics.gravity.magnitude;
			Debug.Log((-velocity * Mathf.Sin(angle)) + " - " + (Mathf.Pow(Time.deltaTime * Time.frameCount,2) * Physics.gravity.magnitude) + " = " + y);
			//Debug.Log (x + " asd " + velocity * Mathf.Sin (angle));
			// Do your own pathfinding here
			Vector3 movement = (syncTargetPos - transform.position).normalized;
			// Normalise the movement vector and make it proportional to the speed per second.
			float movY = movement.y * y ;
			movement = movement * x;
			//movement.y = movY;

			// Move the player to it's current position plus the movement.
			transform.position = transform.position + movement;
		}
	}
}
