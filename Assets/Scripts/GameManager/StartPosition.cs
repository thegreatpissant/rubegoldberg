using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPosition : MonoBehaviour {

	[SerializeField]
	private GameManager gameManager = null;

	[SerializeField]
	private GameObject throwHint = null;

	void Start () {
		if (gameManager == null) {
			gameManager = GameObject.FindObjectOfType<GameManager> ();
		}
		if (gameManager.tutorial) {
			throwHint.SetActive (false);
		}
	}

	public void ShowThrowHint(GameManager gm) {
		Debug.Log ("StartPosition - ThrowHint");
		throwHint.SetActive (true);
	}

	void OnTriggerEnter(Collider col) {
		gameManager.SendMessage ("StartGamePlay");
	}
}
