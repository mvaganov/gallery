using UnityEngine;
using System.Collections;

public class TextSpace : MonoBehaviour {

	private static TextSpace singleton;
	private UnityEngine.UI.Text t;

	public static TextSpace New() { 
		GameObject go = Instantiate (singleton.gameObject) as GameObject;
		return go.GetComponent<TextSpace> ();
	}

	// Use this for initialization
	void Start () {
		if (!singleton) {
			singleton = this;
		}
	}

	public UnityEngine.UI.Text FindText() {
		GameObject go = transform.GetChild(0).gameObject;
		t = go.GetComponent<UnityEngine.UI.Text> ();
		return t;
	}

	public void SetText(string text) {
		if (!t)
			FindText ();
		t.text = text;
	}
}
