using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointObject : MonoBehaviour {

	GameManager gameManager = null;

	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindObjectOfType<GameManager> ();	
	}

	void EndPointHit() {
		gameManager.HitEndPoint ();
	}

	void OnCollisionEnter(Collision col) {
		Debug.Log ("EndPoint Hit");
		if (col.gameObject.name == "Ball") {
			EndPointHit ();
		} else {
			Debug.Log ("Not a ball");
		}
	}

}
