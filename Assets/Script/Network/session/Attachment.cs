using UnityEngine;
using System.Collections;
using System.IO;
using System.Net.Sockets;

namespace Lorance.RxScoket.Session {
	public class Attachment {
		public ByteBuffer byteBuffer;
		public Socket client;

		public Attachment(ByteBuffer byteBuffer, Socket client) {
			this.byteBuffer = byteBuffer;
			this.client = client;
		}
	}
}