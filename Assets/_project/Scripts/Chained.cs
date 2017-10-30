using UnityEngine;
using System.Collections;

public class Chained : MonoBehaviour {
	public BoxCollider boundary;
	void Start() { if(GetComponent<StartLocation> () == null) { gameObject.AddComponent<StartLocation> (); } }
	void FixedUpdate () {
		if (boundary) {
			if (!boundary.bounds.Contains (transform.position)) {
				StartLocation startLoc = GetComponent<StartLocation> ();
				if (!startLoc) { Debug.LogError ("Missing StartLocation!"); }
				transform.position = startLoc.position;
			}
		}
	}
}
