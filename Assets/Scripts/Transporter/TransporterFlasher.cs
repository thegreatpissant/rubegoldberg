using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransporterFlasher : MonoBehaviour {
	[SerializeField]
	private float offsetAmnt = 0.1f;

	private MeshRenderer mesh;
	private Material mat;
		private Vector2 offset;
	void Start () {
		mesh = gameObject.GetComponent<MeshRenderer> ();
		mat = mesh.sharedMaterial; // gameObject.GetComponent<Material> ();
		offset = mat.mainTextureOffset; // .GetTextureOffset (0);
	}
	// Update is called once per frame
	void Update () {
		offset.y += offsetAmnt * Time.deltaTime;
		mat.mainTextureOffset = offset; //  SetTextureOffset (0, offset);
	}
}
