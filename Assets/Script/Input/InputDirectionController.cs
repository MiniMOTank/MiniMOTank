using UnityEngine;
using System.Collections;

public class InputDirectionController : MonoBehaviour
{
	public InputCommunicate inputCommunicate;
	public InputDirectCommunicate inputDirectCommunicate;

	public int onHor;
	public int onVer;

	public bool preH;
	public bool preV;
	public Direction preDirection;

	void FixedUpdate ()
	{
		onHor = inputCommunicate.onHorizontalDirection;
		onVer = inputCommunicate.onVerticalDirection;

		int dirValue;
		Direction curDir;

		if (onHor != 0 && onVer == 0) {
			HState (out dirValue, out curDir, true, false);
		} else if (onHor == 0 && onVer != 0) {
			VState (out dirValue, out curDir, false, true);
		} else if (onHor == 0 && onVer == 0) {
			IdleState (out dirValue, out curDir, false, false);
		} else // onHor != 0 && onVer != 0
			if (!preH && !preV) {//special condition
				HState (out dirValue, out curDir, true, false);
			} else if (preH  && !preV) {
					VState (out dirValue, out curDir, true, true);
			} else if (!preH && preV) {
					HState (out dirValue, out curDir, true, true);	
			} else {// if (preH && preV) {
				if (preDirection == Direction.Horizontal) {
					HState (out dirValue, out curDir, true, true);
				} else if(preDirection == Direction.Vertical) {
					VState (out dirValue, out curDir, true, true);
				} else {
					//can't arrived
					print("can't arrive");
					HState (out dirValue, out curDir, true, false);
				}
			}
		
		inputDirectCommunicate.playerCommunicate.direction = curDir;
		inputDirectCommunicate.playerCommunicate.dirValue = dirValue;
	}

	void HState (out int dirValue, out Direction curDir, bool h, bool v)
	{
		dirValue = onHor;
		curDir = Direction.Horizontal;
		preH = h;
		preV = v;
		preDirection = curDir;
	}

	void VState (out int dirValue, out Direction curDir, bool h, bool v)
	{
		dirValue = onVer;
		curDir = Direction.Vertical;
		preH = h;
		preV = v;
		preDirection = curDir;
	}

	void IdleState (out int dirValue, out Direction curDir, bool h, bool v)
	{
		dirValue = 0;
		curDir = Direction.Idle;
		preH = h;
		preV = v;
		preDirection = Direction.Idle;
	}
}
