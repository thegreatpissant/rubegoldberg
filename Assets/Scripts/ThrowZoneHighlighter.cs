using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowZoneHighlighter : MonoBehaviour {

	public Material Ok;
	public Material NotOK;
	public MeshRenderer meshRenderer;

	// Use this for initialization
	void Start () {
		meshRenderer = gameObject.GetComponent<MeshRenderer> ();
	}

	private float tileOffset = 1;

	void FixedUpdate () {
		tileOffset += 1f;
		Vector2 offset = new Vector2 (tileOffset, 0);
		Ok.mainTextureOffset = Ok.mainTextureOffset + offset;
		NotOK.mainTextureOffset = NotOK.mainTextureOffset + offset; 
	}

	void OnTriggerEnter (Collider col) {
		if (col.CompareTag("Throwable")) {
			meshRenderer.material = Ok;
		}
	}

	void OnTriggerExit (Collider col) {
		if (col.CompareTag ("Throwable")) {
			meshRenderer.material = NotOK;
		}
	}
}
