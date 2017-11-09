// http://answers.unity3d.com/questions/622154/is-there-a-callback-function-or-event-for-a-resolu.html
namespace UnityEngine {    
	using System;

	public static class ScreenExtensions
	{
		public static event Action<Resolution> ResolutionChanged;

		public static void ApplyToScreen(this Resolution resolution, bool fullscreen)
		{
			Screen.SetResolution(resolution.width, resolution.height, fullscreen, resolution.refreshRate);
			if (ResolutionChanged != null)
				( (Action)(() => ResolutionChanged(resolution)) )
					.PerformAfterCoroutine<WaitForEndOfFrame>();
		}
	}
}