using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NS {
	[RequireComponent(typeof(Collider))]
	public class Trigger : MonoBehaviour {
		[Tooltip("If not empty, only trigger if the triggering collider is tagged with this value")]
		public string triggerTag;
		[Tooltip("Transform to teleport to\nSceneAsset to load a new scene\nAudioClip to play audio\nGameObject to SetActivate(true)")]
		public Object whatToTrigger;

		void Start () {
			Collider c = GetComponent<Collider> ();
			c.isTrigger = true;
		}

		public static void DoTrigger(object triggeringObject, object whatToTrigger, GameObject triggeredGameObject){
			if (whatToTrigger == null) { Debug.LogError ("Don't know how to trigger null"); return; }
			System.Type type = whatToTrigger.GetType ();
			if (typeof(System.Action).IsAssignableFrom(type)) {
				System.Action a = whatToTrigger as System.Action;
				a.Invoke();
			} else if (type == typeof(Transform)) {
				Transform t = whatToTrigger as Transform;
				Transform toMove = triggeringObject as Transform;
				if (toMove == null) {
					GameObject go = triggeringObject as GameObject;
					if (go != null) { toMove = go.transform; }
				}
				if (toMove != null) { toMove.position = t.position; }
			} else if (type == typeof(UnityEditor.SceneAsset)) {
				SceneManager.LoadScene ( (whatToTrigger as UnityEditor.SceneAsset).name );
			} else if (type == typeof(AudioClip)) {
				AudioSource asource = triggeredGameObject.GetComponent<AudioSource> ();
				if (asource == null) { asource = triggeredGameObject.AddComponent<AudioSource> (); }
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
			DoTrigger (col.gameObject, whatToTrigger, gameObject);
		}
	}
}