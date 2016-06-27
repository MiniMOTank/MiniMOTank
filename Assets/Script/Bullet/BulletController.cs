using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {
	public float speed = 1f;
//	public Rigidbody2D regidbody2d;
	public GameObject boom;
	// Use this for initialization
	void Start () {
	
	}
	
	void FixedUpdate () {
		transform.Translate (-Vector3.down * speed * Time.fixedDeltaTime, Space.Self);
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Shotable") {
			//todo random rotation
			Instantiate(boom, transform.position, transform.rotation);
			Destroy (gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		if (collider.tag == "Boundary") {
			Destroy (gameObject);
		}
	}
}
