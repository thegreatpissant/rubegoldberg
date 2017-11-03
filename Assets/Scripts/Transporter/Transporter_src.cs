using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transporter_src : MonoBehaviour {

	Transporter transportManger;

	// Use this for initialization
	void Start () {
		transportManger = transform.parent.gameObject.GetComponent<Transporter> ();
	}
	
	void OnTriggerEnter(Collider col) {
/*		Debug.Log ("Transporter SRC Trigger with Gameobject");*/
		if(col.gameObject.CompareTag ("Throwable")) {
			transportManger.SendMessage ("TransporterReady", col.gameObject);
		}
			
	}	

	private void OnGrabbed (ControllerInputManager cip) {
		transportManger.gameObject.SendMessage ("OnGrabbed", cip);
	}
	private void UnGrabbed (ControllerInputManager cip) {
		transportManger.gameObject.SendMessage ("UnGrabbed", cip);
	}
	private void GrabbedUpdate (ControllerInputManager cip) {
		transportManger.gameObject.SendMessage ("GrabbedUpdate", cip);
	}
}
