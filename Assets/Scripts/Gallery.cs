using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class Gallery : MonoBehaviour {

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
			GameObject go = 
				Instantiate (entries [i], cursor.transform.position, cursor.transform.rotation) as GameObject;
			go.GetComponent<Renderer> ().material.color = Votable.defaultColor;
			go.AddComponent<Votable> ();
			transform.Rotate (0, turn, 0);
			entries [i] = go;
		}
	}

	bool rotated = false;

	float timer = 0;
	float refreshTime = 2;
	
	// Update is called once per frame
	void Update () {
		if (!rotated) {
			for (int i = 0; i < entries.Count; ++i) {
				entries [i].GetComponent<Votable> ().GetTextSpace ().FindText ().transform.Rotate (-90, 180, 0);
			}
			rotated = true;
		}
		timer += Time.deltaTime;
		if (timer > refreshTime) {
			timer = 0;
			RefreshRanking ();
		}
	}

	public string results;

	void RefreshRanking() {
		entries.Sort (delegate(GameObject a, GameObject b) {
			Votable x = a.GetComponent<Votable>();
			Votable y = b.GetComponent<Votable>();
			if (x.votes > y.votes)
				return -1;
			else if (y.votes > x.votes)
				return 1;
			else return a.name.CompareTo(b.name);
		});
		results = "";
		for (int i = 0; i < entries.Count; ++i) {
			Votable v = entries [i].GetComponent<Votable> ();
			results += v.votes + ": " + v.name + "\n";
		}
		GetTextSpace ().SetText (results);
		UnityEngine.UI.Text t = GetTextSpace ().FindText ();
		t.alignment = TextAnchor.UpperLeft;
		t.color = Color.black;
		t.fontSize = 4;
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
