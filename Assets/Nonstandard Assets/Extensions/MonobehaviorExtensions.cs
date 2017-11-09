// http://answers.unity3d.com/questions/622154/is-there-a-callback-function-or-event-for-a-resolu.html
namespace UnityEngine
{    
	using System;
	using System.Collections;

	public static class MonoBehaviourExtensions
	{
		/// <summary>
		/// Performs an Action after a YieldInstruction. 
		/// </summary>
		public static void StartCoroutine<T>(this MonoBehaviour monoBehaviour, Action action)
			where T : YieldInstruction, new()
		{
			monoBehaviour.StartCoroutine(Coroutine<T>(action));
		}

		static IEnumerator Coroutine<T>(Action action) where T : YieldInstruction, new()                
		{
			yield return new T();
			action();
		}
	}

	public class CoroutineBehaviour : MonoBehaviour 
	{
		static MonoBehaviour Instance = new GameObject { hideFlags = HideFlags.HideAndDontSave }
			.AddComponent<CoroutineBehaviour>();

		public static void StartCoroutine<T>(Action action) where T : YieldInstruction, new()
		{
			Instance.StartCoroutine<T>(action);
		}
	}
}