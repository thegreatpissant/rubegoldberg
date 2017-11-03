using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoverer : MonoBehaviour {

	public Vector3 movementVector;
	public float frequency;
	public float distance;
	private Vector3 originalGlobalPosition;
	private Vector3 originalLocalPosition;
	//  originalTransform;
	// Use this for initialization
	void Start () {
		originalGlobalPosition = gameObject.transform.position;
		originalLocalPosition = gameObject.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.parent) {
			transform.localPosition = originalLocalPosition + movementVector * Mathf.Sin (Time.time * frequency) * distance * transform.localScale.magnitude ;
		} else {
			transform.position = originalGlobalPosition + movementVector * Mathf.Sin (Time.time * frequency) * distance;
		}
	}
}
