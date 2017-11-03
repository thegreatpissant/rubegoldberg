using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBoard : MonoBehaviour {

	public float force = 0.7f;
	// Use this for initialization
	void Start () {
		
	}
	
	void OnTriggerEnter ( Collider col ) {
		Debug.Log ("TRAMPOLINE COLLISION");
		if (col.gameObject.CompareTag ("Throwable")) {
			Debug.Log ("BALLLLLL");
			Rigidbody rb = col.gameObject.GetComponent<Rigidbody> ();
			col.gameObject.GetComponent<Rigidbody> ().AddForce (transform.up * force * rb.velocity.magnitude, ForceMode.Impulse);
		}
	}

}
