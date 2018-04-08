using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerList : MonoBehaviour {
	public List<Object> whatToTrigger;
	public NS.Trigger.TKind kind = NS.Trigger.TKind.onTriggerEnter;
	public string triggerTag;
	public void DoTrigger (){ NS.Trigger.DoTrigger (gameObject, whatToTrigger, gameObject); }
	void Start () { NS.Trigger.AddTriggers (gameObject, whatToTrigger, kind, triggerTag); }
}
