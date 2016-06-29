using UnityEngine;
using System.Collections;
using System.Text;
using System.Net;
using System;

namespace Lorance.RxScoket {
	public class Common {
		//only use the method if src form system type such as system int, float, double.
		public static byte[] ToBigEnd(byte[] src) {
			if (BitConverter.IsLittleEndian){
				Array.Reverse(src);
			}
			return src;
		}

		public static byte[] concat(byte[] first, byte[] second) {
			byte[] final = new byte[first.Length + second.Length];
			Array.Copy(first, 0, final, 0, first.Length);
			Array.Copy(second, 0, final, first.Length, second.Length);
			return final;
		}

		public static byte[] readyData(byte protoGroup, string data) {
			byte[] byteUID = new byte[1]{protoGroup};
			byte[] load = Encoding.UTF8.GetBytes(data);
			byte[] length = ToBytes(load.Length);

			return concat (concat (byteUID, length), load);
		}

		//BigEnd
		public static int ToInt(byte[] src) {
//			var srcBigEnd = ToBigEnd (src);
			return src [3] & 0xFF |
				(src [2] & 0xFF) << 8 |
				(src [1] & 0xFF) << 16 |
				(src [0] & 0xFF) << 24;
		}

		public static byte[] ToBytes(int src) {
			var bts = BitConverter.GetBytes (src);
			return ToBigEnd (bts);
		}
	}
}