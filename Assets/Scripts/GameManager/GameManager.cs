using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	//  Game State Vars
	private int objectsCollectedCount = 0;
	private int objectsAvailableCount = 0;
	private CollectableObject[] collectableObjects = null;
	public bool playerWon = false; 
	public int score = 0;
	public bool running = false;
	public bool ballInPlay = false;

	//  Game Objects
	[SerializeField]
	GameObject gameBall = null;

	[SerializeField]
	StartPosition startPosition = null;

	[SerializeField]
	string nextScene = "StartScreen";

	int PurgatoryLayer = 0;

	//  Tutorial States
	public bool tutorial = false;
	private bool gameTutorialRunning = false;
	private bool createdObject = false;
	private Coroutine gameTutorialCoroutine = null;

	// Use this for initialization
	void Start () {
		playerWon = false;
		gameTutorialRunning = false;
		PurgatoryLayer = LayerMask.NameToLayer ("Purgatory");
		gameBall = GameObject.Find ("Ball");
		startPosition = GameObject.FindObjectOfType<StartPosition>();
		collectableObjects = GameObject.FindObjectsOfType<CollectableObject>();
		objectsAvailableCount = collectableObjects.Length;
	}

	public void AddScore(int value) {
		Debug.Assert(running, "Scoring while not running, Something is wrong!");
		if (!running) {
			return;
		}
		objectsCollectedCount += 1;
		score += value;
		Debug.Log ("Player Score: " + score.ToString ());
	}

	public void ResetGameplay() {
		Debug.Log ("ResetGameplay()");
		Debug.Log ("GameTutorial: " + gameTutorialRunning.ToString () + " running: " + running.ToString () + " !createdObject: " + (!createdObject).ToString ());
		//  Tutorial 
		if (tutorial) {
			if (gameTutorialRunning && running && !createdObject) {
				Debug.Log ("Tutorial - Create an object");
				//  They are trying to get the game to the goal without 
				ControllerInputManager.rightController.SendMessage ("InvokeObjectHint");
			}
		}
		//  Gameplay
		running = false;
		ballInPlay = false;

		for (int i = 0; i < collectableObjects.Length; i++) {
			collectableObjects[i].gameObject.SetActive (true);
		}
		objectsCollectedCount = score = 0;
	}
	public void ThrowingBall() {
		Debug.Log ("Throwing Ball");
	}
	public void StartGamePlay() {
		if (!ballInPlay) {
			return;
		}
		ballInPlay = false;
		ResetGameplay ();
		running = true;
		if (tutorial) {
			StopGameTutorial ();
		}
	}

	public void HitEndPoint () {
		if (objectsCollectedCount == objectsAvailableCount && running) {
			Debug.Log ("Collected All objects and hit endpoint");
			playerWon = true;
			Rigidbody rb = gameBall.GetComponent<Rigidbody>();
			rb.angularVelocity = rb.velocity = Vector3.zero;
			Invoke ("LoadNextLevel", 4f);
		} else if (!running) {
			PlayerCheating ();
		}
	}
	void Update () {
		if (playerWon) {
			Rigidbody rb = gameBall.GetComponent<Rigidbody>();
			rb.angularVelocity = rb.velocity = Vector3.zero;
		}
	}

	public void LoadNextLevel() {
		Debug.Log ("Loading next Level");
		SteamVR_LoadLevel.Begin (nextScene);
	}
	public void PlayerCheating() {
		Debug.Log ("Player is cheating");
		gameBall.layer = PurgatoryLayer;
		ResetGameplay ();
	}
	public void PlayerGrabbedBall( ControllerInputManager cim ) {
		if (running) {
			PlayerCheating ();
		}
	}

	// --------------------------
	//  Tutorial 
	// --------------------------
	#region "Tutorial"
	public void StopGameTutorial() {
		if (gameTutorialCoroutine != null) {
			StopCoroutine (gameTutorialCoroutine);
			gameTutorialCoroutine = null;
		}
		CancelInvoke ("StartGameTutorial");
	}
	private IEnumerator StartGameTutorialCoroutine () {
		
		while (true) {
			Debug.Log("Tutorial - GameManager - Show Throw here sign");
			startPosition.SendMessage("ShowThrowHint", this);
			yield return new WaitForSeconds (3.0f);
		}
	}
	public void StartGameTutorial(ControllerInputManager cim) {
		//  Initiate Tutorial
		Debug.Log("Tutorial - StartGameTutorial");
		gameTutorialRunning = true;
		//  Show Throw here
		StopGameTutorial();
		gameTutorialCoroutine = StartCoroutine (StartGameTutorialCoroutine ());
	}

	public void CreatedObject () {
		Debug.Log ("gameTutorialRunning: " + gameTutorialRunning.ToString ());
		if (gameTutorialRunning) {
			createdObject = true;
			Debug.Log ("Tutorial - GameManager - Created an object");
		}
	}
	#endregion
}
