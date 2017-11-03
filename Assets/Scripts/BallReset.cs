//============== James A. Feister  Udacity Rube Goldberg Project ==============
// 
// Controll the ball
// 
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallReset : MonoBehaviour {

	public Transform StartPoint = null;

	[SerializeField]
	private GameManager gameManager = null;

	int GameLayer = 0;

	Vector3 InitialPosition;
	Quaternion InitialRotation;

	bool inThrowZone = false;

	// Use this for initialization
	void Start () {
		if (gameManager == null) {
			gameManager = GameObject.FindObjectOfType<GameManager> ();
		}
		GameLayer = LayerMask.NameToLayer ("Game");
		if (StartPoint != null) {
			transform.position = StartPoint.position;
		} else {
			InitialPosition = transform.position;
			InitialRotation = transform.rotation;
		}
	}

	private void ResetBall() {
		gameObject.layer = GameLayer;
		Rigidbody rb = gameObject.GetComponent<Rigidbody> ();
		rb.angularVelocity = Vector3.zero;
		rb.velocity = Vector3.zero;
		if (StartPoint != null) {
			transform.position = StartPoint.position;
		} else {
			transform.position = InitialPosition;
			transform.rotation = InitialRotation;
		}
		gameManager.ResetGameplay ();
	}

	void OnTriggerEnter (Collider col) {
		if (col.gameObject.CompareTag ("Ground")) {
/*			Debug.Log ("Collision: HitGround");*/
			ResetBall ();
		}
		if (col.gameObject.CompareTag ("ThrowZone")) {
			Debug.Log ("Ball In ThrowZone");
			inThrowZone = true;
		}
			
	}

	void OnTriggerExit (Collider col) {
		if (col.gameObject.CompareTag("ThrowZone")) {
			inThrowZone = false;
		}
	}

	public void Thrown (ControllerInputManager cim) {
		Debug.Log ("I was Thrown");
		if (inThrowZone) {
			gameManager.ballInPlay = true;
		}
	}
}