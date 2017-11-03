using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuItem : MonoBehaviour {

	[SerializeField]
	private GameObject myPrefab;

	public GameObject MyPrefab {
		get {
			return myPrefab;
		}
	}
}
