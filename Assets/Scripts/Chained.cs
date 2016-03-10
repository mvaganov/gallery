using UnityEngine;
using System.Collections;

[RequireComponent(typeof(StartLocation))]
public class Chained : MonoBehaviour {

	public BoxCollider boundary;
	private StartLocation startLoc;

	void Start() {
		//boundary = GetComponent<BoxCollider> ();
		startLoc = GetComponent<StartLocation> ();
	}

	void Update () {
		if (boundary) {
			if (!boundary.bounds.Contains (transform.position)) {
				transform.position = startLoc.position;
			}
		}
	}
}
