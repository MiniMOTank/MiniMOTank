using UnityEngine;
using System.Collections;

/// <summary>
/// todo cache bullet object
/// </summary>
public class PlayerShotController : MonoBehaviour {
	public PlayerCommunicate playerCommunicate;

	public GameObject bullet;
	public Transform bulletTransform;
	public bool onShooting;
	public float spareTime = 1f;
	public float accTime = 0f;

	void FixedUpdate () {
		onShooting = playerCommunicate.onShooting;
		if (accTime <= spareTime) {//can't shoot
			accTime += Time.fixedDeltaTime;
		} else {
			if (onShooting) {//should shooting
				accTime = 0f;
				playerCommunicate.onShooting = false;
				//do shooting
				Instantiate(bullet,bulletTransform.position, bulletTransform.rotation);
			} else {
				//ensure current shooting is complete
				playerCommunicate.onShooting = false;
			}
		}
	}
}
