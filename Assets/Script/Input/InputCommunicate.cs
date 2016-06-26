using UnityEngine;
using System.Collections;

public class InputCommunicate : MonoBehaviour {
	public int onHorizontalDirection;
	public int onVerticalDirection;

	public bool onFire;

	// Update is called once per frame
	void FixedUpdate () {
		//to int
		float hor = Input.GetAxisRaw ("Horizontal");
		if (Mathf.Abs (hor) < 0.01f) {
			onHorizontalDirection = 0;
		} else if (hor > 0f) {
			onHorizontalDirection = 1;
		} else {
			onHorizontalDirection = -1;
		}

		float ver = Input.GetAxisRaw ("Vertical");
		if (Mathf.Abs (ver) < 0.01f) {
			onVerticalDirection = 0;
		} else if (ver > 0f) {
			onVerticalDirection = 1;
		} else {
			onVerticalDirection = -1;
		}

		onFire = Input.GetButtonDown ("Fire1");
	}
}
