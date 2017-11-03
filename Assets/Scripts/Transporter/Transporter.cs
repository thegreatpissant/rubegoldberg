using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transporter : MonoBehaviour {

	public GameObject DST = null;
	public ParticleSystem TransporterSRCEffect = null;
	public ParticleSystem TransporterDSTEffect = null;

	private LineRenderer transporterBeam = null;

	private bool leftHandGrabbing = false;
	private bool rightHandGrabbing = false;

	void Start () {
		transporterBeam = gameObject.AddComponent<LineRenderer> ();
		transporterBeam.enabled = false;
		transporterBeam.startWidth = 0.02f;
		transporterBeam.endWidth = 0.02f;
	}
	public void TransporterReady (GameObject obj) {
		if (DST != null ) {
			if (TransporterSRCEffect != null) {
				TransporterSRCEffect.transform.position = obj.transform.position;
				TransporterSRCEffect.Play ();
			}
			obj.transform.position = DST.transform.position;
			obj.transform.rotation = DST.transform.rotation;
			if (TransporterDSTEffect != null) {
				TransporterDSTEffect.transform.position = obj.transform.position;
				TransporterDSTEffect.Play ();
			}
		}
	}

	void UpdateBeamPosition () {
		transporterBeam.SetPosition (0, TransporterSRCEffect.transform.parent.transform.position);
		transporterBeam.SetPosition (1, TransporterDSTEffect.transform.parent.transform.position);
	}

	void Update () {
	}

	private void OnGrabbed (ControllerInputManager cip) {
		Debug.Log ("OnGrabbed");
		if (cip.HandName == "left") {
			leftHandGrabbing = true;
		}
		else if (cip.HandName == "right") {
			rightHandGrabbing = true;
		}
		UpdateBeamPosition ();
		transporterBeam.enabled = true;
	}
	private void UnGrabbed (ControllerInputManager cip) {
		Debug.Log ("UnGrabbed");
		if (cip.HandName == "left") {
			leftHandGrabbing = false;
		} else if (cip.HandName == "right") {
			rightHandGrabbing = false;
		}
		transporterBeam.enabled = (leftHandGrabbing||rightHandGrabbing);
	}
	private void GrabbedUpdate (ControllerInputManager cip) {
		Debug.Log ("GrabbedUpdate");
		UpdateBeamPosition ();
	}
}
