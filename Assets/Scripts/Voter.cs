using UnityEngine;
using System.Collections;

public class Voter : MonoBehaviour {

	Votable selected;

	bool shiftDown = false;
	void Update () {
		if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
			shiftDown = true;
		}
		if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) {
			shiftDown = false;
		}
		Ray r = new Ray (transform.position, transform.forward);
		RaycastHit hit = new RaycastHit ();
		if (Physics.Raycast (r, out hit)) {
			Votable v = hit.transform.GetComponent<Votable> ();
			if (v != selected) {
				if (selected != null) {
					selected.GetComponent<Renderer> ().material.color = Votable.defaultColor;
				}
				selected = v;
				if (selected != null) {
					selected.GetComponent<Renderer> ().material.color = new Color (1, 1, 1);
				}
			}
			if (selected) {
				if (Input.GetMouseButtonDown (0) || Input.GetAxis ("Mouse ScrollWheel") > 0) {
					selected.AddVote (1);
				}
				if (Input.GetMouseButtonDown (1) || Input.GetAxis ("Mouse ScrollWheel") < 0) {
					selected.AddVote (-1);
				}
				if (Input.GetMouseButtonDown (2)) {
					selected.SetVote (0);
				}
				int dir = shiftDown ? -1 : 1;
				if (Input.GetKeyDown (KeyCode.X)) {
					selected.transform.Rotate (30 * dir, 0, 0);
				}
				if (Input.GetKeyDown (KeyCode.Y)) {
					selected.transform.Rotate (0, 30 * dir, 0);
				}
				if (Input.GetKeyDown (KeyCode.Z)) {
					selected.transform.Rotate (0, 0, 30 * dir);
				}
				if (Input.GetKeyDown (KeyCode.Minus)) {
					selected.transform.localScale = gameObject.transform.localScale * Mathf.Pow (2, -1);
				}
				if (Input.GetKeyDown (KeyCode.Equals)) {
					selected.transform.localScale = gameObject.transform.localScale * Mathf.Pow (2, 1);
				}
			}
		} else {
			if (selected != null) {
				selected.GetComponent<Renderer> ().material.color = Votable.defaultColor;
			}
			selected = null;
		}
	}
}
