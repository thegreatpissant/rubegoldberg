using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour {

	[SerializeField]
	private float angle;

	[SerializeField]
	private Vector3 rotation;

	[SerializeField]
	private float magnitude = 100f;
	void FixedUpdate () {
		transform.Rotate (rotation, Time.deltaTime * magnitude * angle);
	}
}
