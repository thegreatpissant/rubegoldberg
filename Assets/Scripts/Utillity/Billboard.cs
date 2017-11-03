using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {


	public GameObject target = null;

	// Update is called once per frame
	void Update () {
		Vector3 yOffset = new Vector3 (0f, transform.position.y - target.transform.position.y, 0f);
		this.transform.LookAt (target.transform.position + yOffset);
	}
}
