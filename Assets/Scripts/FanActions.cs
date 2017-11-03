using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanActions : MonoBehaviour {

	[SerializeField]
	private GameObject windDirection;

	private void OnGrabbed (ControllerInputManager cip) {
		Debug.Log ("Fan Grabbed");
		windDirection.SetActive (true);
	}

	private void UnGrabbed (ControllerInputManager cip) {
		windDirection.SetActive (false);
	}
}
