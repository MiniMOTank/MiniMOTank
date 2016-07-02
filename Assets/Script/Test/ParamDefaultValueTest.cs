using UnityEngine;
using System.Collections;
using Lorance.RxScoket.Util;


public class ParamDefaultValueTest : MonoBehaviour {
//	readonly Option<string> NoneStr = None<string>.Empty;
	//NOT allow `NoneStr` as default value
	public void A(int a = 1, Option<string> x = default(None<string>)) {
		
	}
}
