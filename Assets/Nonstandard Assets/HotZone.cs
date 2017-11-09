using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class HotZone : MonoBehaviour {
	[Tooltip("If not empty, only trigger if the triggering collider is tagged with this value")]
	public string triggerTag;
	[Tooltip("Transform to teleport\nSceneAsset to load a new scene\nAudioClip to play audio\nGameObject to SetActivate(true)")]
	public Object whatToTrigger;

	void Start () {
		Collider c = GetComponent<Collider> ();
		c.isTrigger = true;
	}

	public void DoTrigger(GameObject trigger){
		System.Type type = whatToTrigger.GetType ();
		if (type == typeof(Transform)) {
			Transform t = whatToTrigger as Transform;
			trigger.transform.position = t.position;
		} else if (type == typeof(UnityEditor.SceneAsset)) {
			SceneManager.LoadScene ( (whatToTrigger as UnityEditor.SceneAsset).name );
		} else if (type == typeof(AudioClip)) {
			AudioSource asource = GetComponent<AudioSource> ();
			if (asource == null) { asource = gameObject.AddComponent<AudioSource> (); }
			asource.clip = whatToTrigger as AudioClip;
			asource.Play ();
		} else if (type == typeof(GameObject)) {
			(whatToTrigger as GameObject).SetActive (true);
		} else {
			Debug.LogError ("Don't know how to trigger a "+type);
		}
	}

	void OnTriggerEnter (Collider col) {
		if (whatToTrigger == null && (col.gameObject.tag == "" || col.gameObject.tag == triggerTag))
			return;
		DoTrigger (col.gameObject);
	}
}
