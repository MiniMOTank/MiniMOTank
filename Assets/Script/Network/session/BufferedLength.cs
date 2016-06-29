using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Lorance.RxScoket.Session{
	public abstract class BufferedLength{
		public abstract int Value();
	}

	public class PendingLength : BufferedLength {
		public readonly byte[] arrived;
		public int arrivedNumber;

		public PendingLength(
			byte[] arrived,
			int arrivedNumber) {
			this.arrived = arrived;
			this.arrivedNumber = arrivedNumber;
		}

		public override int Value() {
			throw new System.Exception ("length not completed");
		}
	}

	public class CompletedLength : BufferedLength {
		public readonly int length;
		public CompletedLength(int length) {
			this.length = length;
		}

		public override int Value() {
			return length;
		}
	}
}