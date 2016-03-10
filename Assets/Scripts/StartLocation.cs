using UnityEngine;
using System.Collections;

public class StartLocation : MonoBehaviour {
	public Vector3 position;
	void Start () {
		position = transform.position;
	}
}
