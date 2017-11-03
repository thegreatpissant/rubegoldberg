using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour {

	[SerializeField]
	private float windspeed;

	List<GameObject> Colliders = new List<GameObject> ();

	void OnTriggerEnter (Collider col) {
		if (!col.gameObject.GetComponent<BallReset>() ) {
			return;
		}
		if( !Colliders.Contains (col.gameObject)) {
			Debug.Log ("Blowing on object");
			Colliders.Add (col.gameObject);
		}
	}

	void OnTriggerExit (Collider col) {
		Colliders.Remove (col.gameObject);
		Debug.Log ("No more blowing");
	}

	void FixedUpdate () {
		foreach (GameObject col in Colliders) {
/*			Debug.Log (col);*/
			float mag = (transform.position - col.transform.position).magnitude;
/*			Debug.Log ("Magnitude: " + mag.ToString ());*/
			col.GetComponent<Rigidbody> ().AddForce (transform.forward * windspeed * (1f / mag));
		}		
	}

}
