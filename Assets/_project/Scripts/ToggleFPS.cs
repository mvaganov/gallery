using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class ToggleFPS : MonoBehaviour {
	void Start () {
		DisableIfCmd ();
		CmdLine.Instance.onStartInteraction += DisableIfCmd;
		CmdLine.Instance.onStopInteraction += DisableIfCmd;
	}
	void DisableIfCmd() {
		SetPlayerControl (!CmdLine.Instance.IsInteractive ());
	}
	public void SetPlayerControl(bool enabled) {
		FirstPersonController fps = GetComponent<FirstPersonController> ();
		if (fps) {
			fps.enabled = enabled;
		}
		if (!enabled) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		} else {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}
