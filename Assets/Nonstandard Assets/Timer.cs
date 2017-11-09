using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NonStandard assets
namespace NS {
	public class Timer : MonoBehaviour {
		[System.Serializable]
		public class ToDo {
			public string description;
			public long when;
			public System.Action what;
			public ToDo(long when, System.Action what, string description = null) {
				this.description = description != null ? description : what.Method.Name;
				this.when = when; this.what = what;
			}
		}
		/// <summary>using a List, which is contiguous memory, because it's faster than a liked list MOST of time, because of cache misses, and reasonable data loads</summary>
		public List<ToDo> queue = new List<ToDo>();
		/// <summary>The singleton</summary>
		private static Timer s_instance = null;
		public static Timer Instance() {
			if (s_instance == null) {
				if((s_instance = FindObjectOfType(typeof(Timer)) as Timer) == null) { // find the instance
					GameObject g = new GameObject("<" + s_instance.GetType().Name + ">");
					s_instance = g.AddComponent<Timer>(); // if there is no instance, create one
				}
			}
			return s_instance;
		}
		/// <summary>While this is zero, use system time. As soon as time becomes perturbed, by pause or time scale, keep track of game-time. To reset time back to realtime, use SynchToRealtime()</summary>
		private long alternativeTime = 0;
		/// <summary>The timer counts in milliseconds, Unity measures in fractions of a second. This value reconciles fractional milliseconds.</summary>
		private float leftOverTime = 0;

		public static long NowRealtime() { return System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond; }

		public long Now() { return alternativeTime == 0 ? NowRealtime() : alternativeTime; }

		public static long now() { return Instance ().Now (); }

		public void SynchToRealtime() { alternativeTime = 0; }

		private int BestIndexFor(long soon){
			int index = 0;
			for (index = 0; index < queue.Count; ++index) {
				if (queue [index].when > soon) break;
			}
			return index;
		}

		public void SetTimeout(System.Action action, long delayMilliseconds) {
			long soon = Now () + delayMilliseconds;
			queue.Insert(BestIndexFor (soon), new ToDo(soon, action));
		}

		public static void setTimeout(System.Action action, long delayMilliseconds) {
			Instance ().SetTimeout (action, delayMilliseconds);
		}

		void OnApplicationPause(bool paused) { if (alternativeTime == 0) { alternativeTime = Now (); } }
		
		void Start () {
			#if UNITY_EDITOR
			// This method is run whenever the playmode state is changed.
			UnityEditor.EditorApplication.pauseStateChanged += (UnityEditor.PauseState ps) => {
				if (ps == UnityEditor.PauseState.Paused && alternativeTime == 0) { alternativeTime = Now(); }
			};
			#endif
			if (s_instance != null) { throw new System.Exception ("there should only be one timer!"); }
			s_instance = this;
			alternativeTime = 0;
		}

		void Update () {
			long now;
			if (alternativeTime == 0) {
				now = Now ();
				if (Time.timeScale != 1) { alternativeTime = now; }
			} else {
				float deltaTimeMs = (Time.deltaTime * 1000);
				long deltaTimeMsLong = (long)(deltaTimeMs + leftOverTime);
				alternativeTime += deltaTimeMsLong;
				leftOverTime = deltaTimeMs - deltaTimeMsLong;
				now = alternativeTime;
			}
			while (queue.Count > 0 && queue [0].when <= now) {
				ToDo todo = queue [0];
				queue.RemoveAt (0);
				todo.what.Invoke ();
			}
		}
	}
}