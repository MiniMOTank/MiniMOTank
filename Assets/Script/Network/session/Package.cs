using UnityEngine;
using System.Collections;

public class Package : MonoBehaviour{
	public static int s_level = 0;
	public static void Log(string msg, int level = 0) {
		if (level <= s_level) {
			print (msg);
		}
	}
}
