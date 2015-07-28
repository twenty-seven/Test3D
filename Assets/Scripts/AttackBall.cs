//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.18444
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace BlobWars
{
	public class AttackBall : MonoBehaviour
	{

		// Speed of the projectile per Time.deltaTime
		float exitSpeed = 10;
		// Exit angle of the projectile, gets calculated automatically.
		double exitAngle;
		// Time duration of the travel
		float travelTime;
		// Target position
		Vector3 targetPos;
		// Projectile Prefab
		public GameObject projectile;
		private AttackBall bullet;
		 Vector3 syncPos;
		float angle = 75f;


		Vector2 vecSpeed = Vector2.zero;
		float speed = 5;
		private float minDistance = .5f;
		private float timeDiff;
		public void Start() {
			targetPos = transform.position;
		}

		public void shootAt(Vector3 location) {
			if (bullet != null)
				return;
			Debug.Log ("Transform: " + transform.position + " target: " + targetPos);

			AttackBall ball = ((GameObject) Instantiate(projectile,transform.position, GetComponentInParent<Blob>().transform.rotation)).GetComponent<AttackBall>();
			bullet = ball;
			travelTime = GetComponentInParent<Blob> ().attackSpeed;
			targetPos =  GetComponentInParent<Blob>().transform.position;
			ball.travelTime = travelTime;
			ball.targetPos = targetPos;
			Debug.Log ("Ball: " + ball.transform.position);
			ball.moveTo (location);




			//float rad = Mathf.Deg2Rad * ((distance * Physics.gravity.magnitude) / exitSpeed);

			//angle = Mathf.Acos(rad);
		}

		public void moveTo(Vector3 location) {
			targetPos = location;
			float distance = Vector3.Distance (transform.position, location);
			Debug.Log ("angle rad: " + (Mathf.Deg2Rad * angle) + " cos " + Mathf.Cos (Mathf.Deg2Rad * angle));
			speed = distance / (travelTime * Mathf.Cos (Mathf.Deg2Rad * angle));
			Debug.Log (speed);
			vecSpeed = new Vector2 (
				speed * Mathf.Cos (angle),
				speed * Mathf.Sin (angle)
				).normalized;
			Debug.Log ("Vector Speed: " + vecSpeed);

		}

		private void StepMove () {
			// If the distance is bigger than range, or I just spawned and have to leave the tower.
			if (Vector3.Distance (transform.position, targetPos) > minDistance) {
				Debug.Log (transform.position + " dest: " + targetPos);
				float speedX = speed * Mathf.Cos (angle);
				float speedY = speed * Mathf.Sin (angle) * Time.deltaTime;
				Debug.Log ("SpeedX: " + speedX + ", speedY: " + speedY + " dist: " + Vector3.Distance (transform.position,targetPos));
				// Do your own pathfinding here
				Vector3 movement = (targetPos - transform.position).normalized;
				Debug.Log ("Movement without speed: " + movement);
				movement.y = 1;
				movement = Vector3.Scale (movement,new Vector3(speedX,speedY,speedX));

				Debug.Log ("Time: " + timeDiff);
				// Normalise the movement vector and make it proportional to the speed per second.
				movement = movement *timeDiff;
				// Move the player to it's current position plus the movement.
				timeDiff += Time.deltaTime;
				transform.position = transform.position + movement;

			
			} else {
				transform.position = GetComponentInParent<Blob>().transform	.position;
				targetPos = transform.position;
			}
			
			
			
		}
		public void Update () {
			if (targetPos != transform.position) {
				//Debug.Log ("Me: " + transform.position + " Mob: " + targetPos);

			}
		}

	}
}

