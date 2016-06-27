using UnityEngine;
using System.Collections;

public enum Direction {
	Idle, Horizontal, Vertical
}

public class PlayerCommunicate : MonoBehaviour {
	public float speed;
	public Direction direction;
	public int dirValue = 0;

	public bool isDie = false;
	public bool onShooting = false;
}
