using UnityEngine;
using System.Collections;

public class InputFireController : MonoBehaviour {
	public InputCommunicate inputCommunicate;
	public InputDirectCommunicate inputDirectCommunicate;
	void FixedUpdate () {
		if (inputCommunicate.onFire) {
			inputDirectCommunicate.playerCommunicate.onShooting = true;
		}
	}
}
