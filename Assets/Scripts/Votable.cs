using UnityEngine;
using System.Collections;

public class Votable : MonoBehaviour {
	public string details;

	public int votes = 0;

	TextSpace text;

	public static Color defaultColor = new Color(0.75f, 0.75f, 0.75f);

	public TextSpace GetTextSpace() {
		if (!text) {
			text = TextSpace.New ();
			text.gameObject.transform.SetParent(gameObject.transform);
			text.gameObject.transform.localPosition = Vector3.zero;
			text.gameObject.transform.rotation = transform.rotation;
		}
		return text;
	}

	void Start() {
		GetTextSpace().SetText ("votes: " + votes);
		Collider c = GetComponent<Collider> ();
		if (!c) {
			MeshFilter m = GetComponent<MeshFilter> ();
			MeshCollider mc = gameObject.AddComponent<MeshCollider> ();
			mc.sharedMesh = m.mesh;
			SphereCollider sc = gameObject.AddComponent<SphereCollider> ();
			sc.radius = 3;
			sc.isTrigger = true;
		}
	}

	public void AddVote(int delta){ SetVote (votes + delta); }

	public void SetVote(int votes) {
		if (votes != this.votes) {
			this.votes = votes;
			text.SetText ("votes: " + votes);
		}
	}
}
