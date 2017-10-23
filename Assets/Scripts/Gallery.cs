using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class Gallery : MonoBehaviour {
	[System.Serializable]
	public class Rotation {
		public bool randomRotate = true;
		public Vector3 weight = Vector3.zero;
	}
	public Rotation rotation;
	public List<GameObject> entries = new List<GameObject>();
	private SphereCollider sphere;


	// Use this for initialization
	void Start () {
		sphere = GetComponent<SphereCollider> ();
		float turn = 360.0f / entries.Count;
		GameObject cursor = new GameObject ();
		cursor.transform.SetParent (transform);
		cursor.transform.localPosition = new Vector3 (0, 0, -sphere.radius);
		cursor.transform.Rotate (-90, 0, 0);
		for (int i = 0; i < entries.Count; ++i) {
			if (entries [i] != null) {
				GameObject go = 
					Instantiate (entries [i], cursor.transform.position, cursor.transform.rotation) as GameObject;
				//go.GetComponent<Renderer> ().material.color = Votable.defaultColor;
				go.AddComponent<Votable> ();
				if (rotation.randomRotate || rotation.weight != Vector3.zero) {
					RandomRotation rr = go.AddComponent<RandomRotation> ();
					rr.useRandom = rotation.randomRotate;
					rr.weight = rotation.weight;
				}
				ResizeModel (go.name, go, 8);
				transform.Rotate (0, turn, 0);
				entries [i] = go;
			}
		}
		CmdLine.Instance.activeOnStart = false;
		CmdLine.Instance.onStartInteraction += RefreshRanking;
	}

	void ResizeMesh(string name, Mesh m, Vector3 offset, float scale) {
		// center all the points
		Vector3[] v = m.vertices;
		for (int i = 0; i < v.Length; ++i) {
			v[i] = v[i] + offset;
		}
		// scale points
		for (int i = 0; i < v.Length; ++i) {
			v[i] = v[i] * scale;
		}
		// set the points
		m.vertices = v;
		m.RecalculateBounds ();
	}

	void ResizeModel(string name, GameObject obj, float size) {
		MeshFilter mf = obj.GetComponent<MeshFilter> ();
		if (mf) {
			Bounds b = mf.mesh.bounds;
			print (name + " " + b+ " " +b.size.magnitude);//+ b);
			ResizeMesh (name, mf.mesh, -b.center, size/b.size.magnitude);
			b = mf.mesh.bounds;
		} else {
			Bounds b = new Bounds ();
			// calculate multi-mesh model
			//print(name+" has multiple parts ("+obj.transform.childCount+")");
			// first, get the total bounds
			for (int i = 0; i < obj.transform.childCount; ++i) {
				mf = obj.transform.GetChild (i).GetComponent<MeshFilter> ();
				if(mf) {
					b.Encapsulate (mf.mesh.bounds);
				}
			}
			//print (name + " " + b.size.magnitude);//+ b+" ("+obj.transform.childCount+" parts) ");
			// then resize everything based on the total bounds
			float scale = size/b.size.magnitude;
			for (int i = 0; i < obj.transform.childCount; ++i) {
				Transform ch = obj.transform.GetChild (i);
				ch.position -= b.center;
				ch.localScale *= scale;
			}
		}
	}

	bool rotated = false;

	float timer = 0;
	float refreshTime = 2;
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > refreshTime) {
			timer = 0;
			if (!rotated) {
				for (int i = 0; i < entries.Count; ++i) {
					if (entries[i] != null && entries [i].GetComponent<Votable> ()) {
						TextSpace ts = entries [i].GetComponent<Votable> ().GetTextSpace ();
						if (ts) {
							ts.FindText ().transform.Rotate (-90, 180, 0);
						}
					}
				}
				rotated = true;
			}
		}
//		if (Input.GetKeyDown (CmdLine.Instance.keyToShow)) {
//			RefreshRanking ();
//		}
	}

	public string results;

	void RefreshRanking() {
		entries.Sort (delegate(GameObject a, GameObject b) {
			float xv = 0, yv = 0;
			if(a){
				Votable x = a.GetComponent<Votable>();
				xv = x.votes;
			}
			if(b){
				Votable y = b.GetComponent<Votable>();
				yv = y.votes;
			}
			if (xv > yv)
				return -1;
			else if (yv > xv)
				return 1;
			else if(a != null && b != null) return a.name.CompareTo(b.name);
			else return 0;
		});
		results = "";
		for (int i = 0; i < entries.Count; ++i) {
			if (entries [i] != null) {
				Votable v = entries [i].GetComponent<Votable> ();
				string namedisplayed = v.name;
				string cloneSuffix = "(Clone)";
				if (namedisplayed.EndsWith (cloneSuffix)) {
					namedisplayed = namedisplayed.Substring (0, namedisplayed.Length - cloneSuffix.Length);
				}
				results += v.votes + ": " + namedisplayed + "\n";
			}
		}
		CmdLine.SetText (results);
//		GetTextSpace ().SetText (results);
//		UnityEngine.UI.Text t = GetTextSpace ().FindText ();
//		t.alignment = TextAnchor.UpperLeft;
//		t.color = Color.black;
//		t.fontSize = 4;
	}

	TextSpace text;
	public TextSpace GetTextSpace() {
		if (!text) {
			text = TextSpace.New ();
			text.gameObject.transform.SetParent(gameObject.transform);
			text.gameObject.transform.localPosition = Vector3.zero;
			text.gameObject.transform.rotation = transform.rotation;
		}
		return text;
	}
}
