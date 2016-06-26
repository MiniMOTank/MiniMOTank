using UnityEngine;
using System.Collections;

public class PlayerMoveController : MonoBehaviour {
	public PlayerCommunicate playerCommunicate;
//	public PlayerDirectCommunicate playerDirectCommunicate;

	public float speed;
	public Direction dir;
	public int dirValue;

	Vector2 moveVector = new Vector2 ();
	Rigidbody2D regidbody;

	void Awake () {
		regidbody = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate () {
		dir = playerCommunicate.direction;
		speed = playerCommunicate.speed;
		dirValue = playerCommunicate.dirValue;

		if (dir == Direction.Horizontal) {
			moveVector.Set (speed * dirValue * Time.fixedDeltaTime, 0f);
		} else if (dir == Direction.Vertical) {
			moveVector.Set (0f, speed * dirValue * Time.fixedDeltaTime);
		} else {
			moveVector.Set (0f, 0f);
		}

		moveVector.Set (transform.position.x + moveVector.x, transform.position.y + moveVector.y);
		regidbody.MovePosition(moveVector);

		if (dir == Direction.Horizontal) {
			if (dirValue < 0)
				regidbody.MoveRotation (90f);
			else
				regidbody.MoveRotation (-90f);
		} else if (dir == Direction.Vertical) {
			if (dirValue < 0)
				regidbody.MoveRotation (180f);
			else
				regidbody.MoveRotation (0f);
		}
			
	}
}
