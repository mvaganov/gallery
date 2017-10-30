using UnityEngine;
public class StartLocation : MonoBehaviour {
	[HideInInspector]
	public Vector3 position;
	void Start () { position = transform.position; }
}
