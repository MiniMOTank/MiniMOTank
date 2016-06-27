using UnityEngine;
using System.Collections;

/// <summary>
/// todo add boom animation
/// </summary>
public class BoomController : MonoBehaviour {
	public float existTime = 1.5f;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, existTime);
	}
}
