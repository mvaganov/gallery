// http://answers.unity3d.com/questions/622154/is-there-a-callback-function-or-event-for-a-resolu.html
namespace System
{
	using System.Collections;
	using UnityEngine;
	public static class ActionExtensions
	{
		public static void PerformAfterCoroutine<T>(this Action action)
			where T : YieldInstruction, new()
		{
			CoroutineBehaviour.StartCoroutine<T>(action);
		}
	}
}