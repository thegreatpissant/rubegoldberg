using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreBoard : MonoBehaviour {

	[SerializeField]
	private GameManager gameManager = null;

	[SerializeField]
	private GameObject gameScoreWord = null;
	[SerializeField]
	private TextMeshPro gameScore = null;
	private int score = 0;
	[SerializeField]
	private TextMeshPro gameWonPhrase = null;
	private string[] phrases = {"Woot!!\nYou Won!", "All The\nThings!!", "Score!!", "Great\nJob!", "Kicking\nBut!", "You Won!", "Totaly\nRubed!"};
	private bool anounced = false;
	// Use this for initialization
	void Start () {
		if (gameManager == null) {
			gameManager = GameObject.FindObjectOfType<GameManager> ();
		}
		score = gameManager.score;
	}

	// Update is called once per frame
	void Update () {
		if (score != gameManager.score) {
			score = gameManager.score;
			gameScore.text = score.ToString ();
		}
		if (gameManager.playerWon && !anounced) {
			anounced = true;
			gameScore.gameObject.SetActive (false);
			gameScoreWord.SetActive (false);
			gameWonPhrase.text = phrases [Random.Range (0, phrases.Length - 1)] + "\n\nLoading\nNext Level";
			gameWonPhrase.gameObject.SetActive (true);
		}
	}
}
