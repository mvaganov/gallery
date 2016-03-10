using UnityEngine;
using System.Collections;

public class Rotating : MonoBehaviour {

	public Vector3 rotationPerSecond = Vector3.zero;
	float seconds;
	Vector3 rotated = Vector3.zero;

	void Update () {
		seconds += Time.deltaTime;
		Vector3 expectedRotation = seconds * rotationPerSecond;
		Vector3 difference = expectedRotation - rotated;
		transform.Rotate (difference);
		rotated = expectedRotation;
		while (seconds > 1) {
			seconds -= 1;
			rotated -= rotationPerSecond;
		}
	}
}
