using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class RandomRotation : MonoBehaviour {
	Vector3 rot;
	public Vector3 weight;
	public bool useRandom = true;
	void Update () {
		if (useRandom) {
			rot += Random.insideUnitSphere;
		} else {
			rot = Vector3.one;
		}
		rot.x = Mathf.Clamp (rot.x, -90, 90) * weight.x;
		rot.y = Mathf.Clamp (rot.y, -90, 90) * weight.y;
		rot.z = Mathf.Clamp (rot.z, -90, 90) * weight.z;
		transform.Rotate (rot * Time.deltaTime);
//		rb.angularVelocity += rot;
//	}
//	Rigidbody rb;
//	void Start() {
//		rb = GetComponent<Rigidbody> ();
//		rb.useGravity = false;
	}
}
