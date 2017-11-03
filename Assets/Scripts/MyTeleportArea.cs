using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTeleportArea : MonoBehaviour {

	MeshRenderer myMesh;
	Material myMaterial;

	// Use this for initialization
	void Awake () {
		gameObject.layer = LayerMask.NameToLayer ("TeleportLayer");
	}
	void Start () {
		myMesh = gameObject.GetComponent<MeshRenderer> ();	
		myMaterial = myMesh.material;
	}
	
	public void Hightlight (bool highlight) {
		if (highlight) {
			myMesh.material = ControllerInputManager.leftController.EnabledTeleportMaterial;
		} else {
			myMesh.material = myMaterial; // teleportManager.DisabledTeleportMaterial;
		}
	}
}
