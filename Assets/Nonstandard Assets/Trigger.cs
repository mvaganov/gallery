using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NS {
	[RequireComponent(typeof(Collider))]
	public class Trigger : MonoBehaviour {
		[Tooltip("* Transform: teleport triggering object to the Transform\n"+
			"* SceneAsset: load the scene\n"+
			"* AudioClip: play audio here\n"+
			"* GameObject: SetActivate(true)\n"+
			"* Material: set this Renderer's .material property\n"+
			"* <general>: activate a \'DoTrigger()\' method (if available)\n"+
			"* IEnumerable: trigger each element in the list")]
		public Object whatToTrigger;

		public enum TKind {
			scriptedOnly,
			onTriggerEnter, onTriggerExit, onTriggerStay, 
			onControllerColliderHit,
			onStart, onDestroy, onEnable, onDisable, onApplicationPause, onApplicationUnpause, onApplicationQuit, 
			onMouseEnter, onMouseOver, onMouseExit,
			onMouseDown, onMouseDrag, onMouseUp, onMouseUpAsButton,
			onBecameInvisible, onBecameVisible
		}
		[Tooltip("What triggers the object above")]
		public TKind kind = TKind.onTriggerEnter;
		[Tooltip("For colliders, if not empty, only trigger if the triggering collider is tagged with this value")]
		public string triggerTag;

		public static void DoTrigger(object triggeringObject, object whatToTrigger, GameObject triggeredGameObject){
			if (whatToTrigger == null) { Debug.LogError ("Don't know how to trigger null"); return; }
			System.Type type = whatToTrigger.GetType ();
			if (typeof(System.Action).IsAssignableFrom (type)) {
				System.Action a = whatToTrigger as System.Action;
				a.Invoke ();
			} else if (type == typeof(Transform)) {
				Transform t = whatToTrigger as Transform;
				Transform toMove = triggeringObject as Transform;
				if (toMove == null) {
					GameObject go = triggeringObject as GameObject;
					if (go != null) {
						toMove = go.transform;
					}
				}
				if (toMove != null) {
					toMove.position = t.position;
				}
			} else if (type == typeof(UnityEditor.SceneAsset)) {
				SceneManager.LoadScene ((whatToTrigger as UnityEditor.SceneAsset).name);
			} else if (type == typeof(AudioClip)) {
				AudioSource asource = triggeredGameObject.GetComponent<AudioSource> ();
				if (asource == null) {
					asource = triggeredGameObject.AddComponent<AudioSource> ();
				}
				asource.clip = whatToTrigger as AudioClip;
				asource.Play ();
			} else if (type == typeof(ParticleSystem)) {
				ParticleSystem ps = whatToTrigger as ParticleSystem;
				Transform t = ps.transform;
				t.position = triggeredGameObject.transform.position;
				t.rotation = triggeredGameObject.transform.rotation;
				ParticleSystem.ShapeModule sm = ps.shape;
				if(sm.shapeType == ParticleSystemShapeType.Mesh) {
					sm.mesh = triggeredGameObject.GetComponent<MeshFilter> ().mesh;
					sm.scale = triggeredGameObject.transform.lossyScale;
				} else if (sm.shapeType == ParticleSystemShapeType.MeshRenderer){
					sm.meshRenderer = triggeredGameObject.GetComponent<MeshRenderer> ();
				}
				ps.Play ();
				Debug.Log (ps.transform);
			} else if (type == typeof(GameObject)) {
				(whatToTrigger as GameObject).SetActive (true);
			} else if (type == typeof(UnityEngine.Material)) {
				Material m = whatToTrigger as Material;
				triggeredGameObject.GetComponent<Renderer> ().material = m;
			} else if(typeof(IEnumerable).IsAssignableFrom(type)) {
				IEnumerable ienum = whatToTrigger as IEnumerable;
				IEnumerator iter = ienum.GetEnumerator ();
				while (iter.MoveNext ()) {
					DoTrigger (triggeringObject, iter.Current, triggeredGameObject);
				}
			} else {
				System.Reflection.MethodInfo[] m = type.GetMethods ();
				bool invoked = false;
				for (int i = 0; i < m.Length; ++i) {
					if (m[i].Name == "DoTrigger" && m[i].GetParameters().Length == 0) {
						m[i].Invoke (whatToTrigger, new object[]{ });
						invoked = true;
						break;
					}
				}
				if(!invoked) {
					Debug.LogError ("Don't know how to trigger a " + type);
				}
			}
		}

		private static void ColToTrig(GameObject g) {
			Collider c = g.GetComponent<Collider> ();      if (c) { c.isTrigger = true; }
			Collider2D c2 = g.GetComponent<Collider2D> (); if (c2) { c2.isTrigger = true; }
		}

		void Start () { AddTriggers (gameObject, whatToTrigger, kind, triggerTag); }

		public static void AddTriggers(GameObject g, object w, TKind kind, string triggerTag) {
			bool is2D = false;
			switch(kind){
			case TKind.onTriggerEnter: case TKind.onTriggerExit: case TKind.onTriggerStay:
				is2D = g.GetComponent<Collider2D> () != null;
				break;
			}
			switch (kind) {
			case TKind.onTriggerEnter:if(is2D) AddTaggedTrigger<OnTriggerEnter2D_> (g,w,triggerTag);
			                          else     AddTaggedTrigger<OnTriggerEnter_>   (g,w,triggerTag); ColToTrig (g); break;
			case TKind.onTriggerExit: if(is2D) AddTaggedTrigger<OnTriggerExit2D_>  (g,w,triggerTag);
			                          else     AddTaggedTrigger<OnTriggerExit_>    (g,w,triggerTag); ColToTrig (g); break;
			case TKind.onTriggerStay: if(is2D) AddTaggedTrigger<OnTriggerStay2D_>  (g,w,triggerTag);
			                          else     AddTaggedTrigger<OnTriggerStay_>    (g,w,triggerTag); ColToTrig (g); break;
			case TKind.onControllerColliderHit:AddTaggedTrigger<OnControllerColliderHit_> (g,w,triggerTag); break;
			case TKind.onStart: 			AddTrigger<OnStart_>    (g,w); break;
			case TKind.onDestroy:			AddTrigger<OnDestroy_>  (g,w); break;
			case TKind.onEnable:			AddTrigger<OnEnable_>   (g,w); break;
			case TKind.onDisable:			AddTrigger<OnDisable_>  (g,w); break;
			case TKind.onApplicationPause:	AddTrigger<OnApplicationPause_>   (g,w); break;
			case TKind.onApplicationUnpause:AddTrigger<OnApplicationUnpause_> (g,w); break;
			case TKind.onApplicationQuit:	AddTrigger<OnApplicationQuit_>    (g,w); break;
			case TKind.onMouseEnter:		AddTrigger<OnMouseEnter_>(g,w); break;
			case TKind.onMouseOver:			AddTrigger<OnMouseOver_> (g,w); break;
			case TKind.onMouseExit:			AddTrigger<OnMouseExit_> (g,w); break;
			case TKind.onMouseDown:			AddTrigger<OnMouseDown_> (g,w); break;
			case TKind.onMouseDrag:			AddTrigger<OnMouseDrag_> (g,w); break;
			case TKind.onMouseUp:			AddTrigger<OnMouseUp_>   (g,w); break;
			case TKind.onMouseUpAsButton:	AddTrigger<OnMouseUpAsButton_> (g,w); break;
			case TKind.onBecameInvisible:	AddTrigger<OnBecameInvisible_> (g,w); break;
			case TKind.onBecameVisible:		AddTrigger<OnBecameVisible_> (g,w); break;
			}
		}

		public void DoTrigger (){ DoTrigger (gameObject, whatToTrigger, gameObject); }
		public class _TriggerBase : MonoBehaviour { 
			public object whatToTrigger;
			public void DoTrigger () { Trigger.DoTrigger (gameObject, whatToTrigger, gameObject); }
			public void DoTriggerMouse() { Trigger.DoTrigger (Input.mousePosition, whatToTrigger, gameObject); }
		}
		private static void AddTrigger<T>(GameObject gameObject, object whatToTrigger) where T : _TriggerBase {
			T t = gameObject.AddComponent<T> ();
			t.whatToTrigger = whatToTrigger;
		}

		public class OnStart_ : _TriggerBase { void Start() { DoTrigger(); }}
		public class OnDestroy_ : _TriggerBase { void OnDestroy() { DoTrigger(); }}
		public class OnEnable_ : _TriggerBase { void OnEnable() { DoTrigger(); }}
		public class OnDisable_ : _TriggerBase { void OnDisable() { DoTrigger(); }}
		public class OnBecameInvisible_ : _TriggerBase { void OnBecameInvisible() { DoTrigger(); }}
		public class OnBecameVisible_ : _TriggerBase { void OnBecameVisible() { DoTrigger(); }}
		public class OnMouseEnter_ : _TriggerBase { void OnMouseEnter() { DoTriggerMouse (); }}
		public class OnMouseOver_ : _TriggerBase { void OnMouseOver() { DoTriggerMouse (); }}
		public class OnMouseExit_ : _TriggerBase { void OnMouseExit() { DoTriggerMouse (); }}
		public class OnMouseDown_ : _TriggerBase { void OnMouseDown() { DoTriggerMouse (); }}
		public class OnMouseDrag_ : _TriggerBase { void OnMouseDrag() { DoTriggerMouse (); }}
		public class OnMouseUp_ : _TriggerBase { void OnMouseUp() { DoTriggerMouse (); }}
		public class OnMouseUpAsButton_ : _TriggerBase { void OnMouseMouseUpAsButton() { DoTriggerMouse (); }}

		public delegate void BooleanAction(bool b);
		public static void EquateUnityEditorPauseWithApplicationPause(BooleanAction b) {
			#if UNITY_EDITOR
			// This method is run whenever the playmode state is changed.
			UnityEditor.EditorApplication.pauseStateChanged += (UnityEditor.PauseState ps) => {
				b(ps == UnityEditor.PauseState.Paused);
			};
			#endif
		}
		public class OnApplicationPause_ : _TriggerBase { void OnApplicationPause(bool pauseStatus) {
			if (pauseStatus == true) { DoTrigger (); }
		} void Start() { EquateUnityEditorPauseWithApplicationPause (OnApplicationPause); }}
		public class OnApplicationUnpause_ : _TriggerBase { void OnApplicationPause(bool pauseStatus) {
			if (pauseStatus == false) { DoTrigger (); }
		} void Start() { EquateUnityEditorPauseWithApplicationPause (OnApplicationPause); }}

		public class OnApplicationQuit_ : _TriggerBase { void OnApplicationQuit() { DoTrigger(); }}
		//onTriggerEnter, onTriggerExit, onTriggerStay, 
		public class _TriggerAreaBase : MonoBehaviour {
			public object whatToTrigger;
			public string triggerTag;
			public bool IsTriggeringObject(GameObject o){ return triggerTag == "" || o.tag == triggerTag || o.tag == ""; }
			public void DoTrigger () { Trigger.DoTrigger (gameObject, whatToTrigger, gameObject); }
			public void DoTrigger (GameObject tobj) {
				if (IsTriggeringObject(tobj)) { Trigger.DoTrigger (tobj, whatToTrigger, gameObject); }
			}
		}
		private static void AddTaggedTrigger<T>(GameObject gameObject, object whatToTrigger, string triggerTag) where T : _TriggerAreaBase {
			T t = gameObject.AddComponent<T> ();
			t.whatToTrigger = whatToTrigger;
			t.triggerTag = triggerTag;
		}

		public class OnTriggerEnter_ : _TriggerAreaBase { void OnTriggerEnter (Collider col) { DoTrigger (col.gameObject); }}
		public class OnTriggerExit_ : _TriggerAreaBase { void OnTriggerExit (Collider col) { DoTrigger (col.gameObject); }}
		public class OnTriggerStay_ : _TriggerAreaBase { void OnTriggerStay (Collider col) { DoTrigger (col.gameObject); }}
		public class OnTriggerEnter2D_ : _TriggerAreaBase { void OnTriggerEnter2D (Collider2D col) { DoTrigger (col.gameObject); }}
		public class OnTriggerExit2D_ : _TriggerAreaBase { void OnTriggerExit2D (Collider2D col) { DoTrigger (col.gameObject); }}
		public class OnTriggerStay2D_ : _TriggerAreaBase { void OnTriggerStay2D (Collider2D col) { DoTrigger (col.gameObject); }}
		public class OnControllerColliderHit_ : _TriggerAreaBase { void OnControllerColliderHit(CharacterController col) { DoTrigger (col.gameObject); }}
	}
}