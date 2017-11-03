using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transporter_dst : MonoBehaviour {

	Transporter transportManger;

	// Use this for initialization
	void Start () {
		transportManger = transform.parent.gameObject.GetComponent<Transporter> ();
	}

	private void OnGrabbed (ControllerInputManager cip) {
		Debug.Log ("Transporter Grabbed");
		transportManger.gameObject.SendMessage ("OnGrabbed", cip);
	}
	private void UnGrabbed (ControllerInputManager cip) {
		transportManger.gameObject.SendMessage ("UnGrabbed", cip);
	}
	private void GrabbedUpdate (ControllerInputManager cip) {
		transportManger.gameObject.SendMessage ("GrabbedUpdate", cip);
	}
}
