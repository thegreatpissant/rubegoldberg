using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CollectableObject : MonoBehaviour {

	[SerializeField]
	private int pointValue = 0;

	[SerializeField]
	private GameObject hitEffect = null;

	GameManager gameManager = null;


	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindObjectOfType<GameManager> ();
	}

	void ObjectCollected() {
		if (gameManager.running) {
			gameManager.AddScore (pointValue);
			Instantiate<GameObject> (hitEffect, transform.position, transform.rotation);
			gameObject.SetActive (false);
		}
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.name == "Ball") {
			ObjectCollected ();
		}
	}
}
