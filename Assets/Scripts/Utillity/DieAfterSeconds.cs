using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterSeconds : MonoBehaviour {

	[SerializeField]
	private float seconds = 1f;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, seconds);
	}
}
