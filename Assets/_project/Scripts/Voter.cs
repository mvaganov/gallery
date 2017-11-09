using UnityEngine;
using System.Collections;

public class Voter : MonoBehaviour {

	Votable selected;

	public GameObject selectionVisual;
	Transform compass, rotator;


	void Start() {
		compass = CreateCompass ();
		rotator = CreateRotator ();
		compass.gameObject.SetActive (false);
		rotator.gameObject.SetActive (false);
	}


	bool shiftDown = false;
	public enum UIState {selecting, grabbing, rotating, scaling};
	UIState state = UIState.selecting;
	Vector3 relativeOffset, originalPosition;


	void FixedUpdate () {
		switch(state){
		case UIState.grabbing:
			float scroll = Input.GetAxis ("Mouse ScrollWheel");
			if (scroll > 0) {
				float d = relativeOffset.magnitude;
				if (d > 1) {
					relativeOffset = relativeOffset.normalized * (d + 1);
				}
			}
			if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
				float d = relativeOffset.magnitude;
				if (d > 1) {
					relativeOffset = relativeOffset.normalized * (d - 1);
				}
			}
			selected.transform.position = transform.TransformPoint (relativeOffset);
			if (Input.GetButtonDown ("Fire2") || Input.GetKeyDown (KeyCode.Escape)) {
				selected.transform.position = originalPosition;
				state = UIState.selecting;
			}
			if (Input.GetKeyDown (KeyCode.G) || Input.GetButtonDown ("Fire1") || Input.GetKeyDown (KeyCode.Return)) {
				state = UIState.selecting;
			}
			break;
		case UIState.selecting:
			shiftDown = Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);
			Ray r = new Ray (transform.position + transform.forward, transform.forward);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (r, out hit)) {
				Votable v = hit.transform.GetComponent<Votable> ();
				selected = v;
				if (selected == null) {
					selectionVisual.transform.localScale = Vector3.zero;
					SetCompassTo (null);
				} else {
					selectionVisual.transform.position = selected.transform.position;
					selectionVisual.transform.localScale = Vector3.one;
					SetCompassTo (selected.gameObject);
				}
				if (selected) {
					if (Input.GetKeyDown (KeyCode.G)) {
						originalPosition = selected.transform.position;
						relativeOffset = transform.InverseTransformPoint (selected.transform.position);
						state = UIState.grabbing;
					}
					if (Input.GetButtonDown ("Fire1") || Input.GetAxis ("Mouse ScrollWheel") > 0) {
						selected.AddVote (1);
					}
					if (Input.GetButtonDown ("Fire2") || Input.GetAxis ("Mouse ScrollWheel") < 0) {
						selected.AddVote (-1);
					}
					if (Input.GetKeyDown (KeyCode.Escape)) {
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
						selected.transform.localScale *= 3.0f / 4;
					}
					if (Input.GetKeyDown (KeyCode.Equals)) {
						selected.transform.localScale *= 4.0f / 3;
					}
				}
			} else {
				if (selected != null) {
					selectionVisual.transform.localScale = Vector3.zero;
					SetCompassTo (null);
				}
				selected = null;
			}
			break;
		}
	}

	void SetupLine(LineRenderer lr, Material m, Color color) {
		lr.useWorldSpace = false;
		lr.material = m;
		lr.material.color = color;
	}

	public Transform CreateCompass(){
		GameObject yaxis = null, xaxis = null, zaxis = null;
		Material m = Lines.FindShaderMaterial ("GUI/Text Shader");
		LineRenderer lr;
		lr = Lines.MakeArrow (ref xaxis, Vector3.zero, Vector3.right, 3, Color.red, 0.0675f, 0.0675f);
		SetupLine(lr, m, Color.red);
		lr = Lines.MakeArrow (ref yaxis, Vector3.zero, Vector3.up, 3, Color.green, 0.0675f, 0.0675f);
		SetupLine(lr, m, Color.green);
		lr = Lines.MakeArrow (ref zaxis, Vector3.zero, Vector3.forward, 3, Color.blue, 0.0675f, 0.0675f);
		SetupLine(lr, m, Color.blue);
		xaxis.transform.SetParent (yaxis.transform);
		zaxis.transform.SetParent (yaxis.transform);
		return yaxis.transform;
	}

	public Transform CreateRotator() {
		GameObject yaxis = null, xaxis = null, zaxis = null;
		Material m = Lines.FindShaderMaterial ("GUI/Text Shader");
		LineRenderer lr;
		lr = Lines.MakeArcArrow (ref xaxis, 180, 24, 3, Vector3.right, Quaternion.Euler(45,0,0)*Vector3.up);
		SetupLine(lr, m, Color.red);
		lr = Lines.MakeArcArrow (ref yaxis, 180, 24, 3, Vector3.up, Quaternion.Euler(0,45,0)*Vector3.forward);
		SetupLine(lr, m, Color.green);
		lr = Lines.MakeArcArrow (ref zaxis, 180, 24, 3, Vector3.forward, Quaternion.Euler(0,0,45)*Vector3.right);
		SetupLine(lr, m, Color.blue);
		xaxis.transform.SetParent (yaxis.transform);
		zaxis.transform.SetParent (yaxis.transform);
		return yaxis.transform;
	}

	void SetCompassTo(GameObject go) {
		if (go == null) {
			compass.gameObject.SetActive (false);
		} else {
			compass.gameObject.SetActive (true);
			Transform t = compass;
			t.position = go.transform.position;
			t.rotation = go.transform.rotation;
			t.SetParent (go.transform);
		}
	}

}
