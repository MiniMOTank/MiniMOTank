using System;
using System.IO;
using Lorance.RxScoket.Util;

namespace Lorance.RxScoket.Session
{
	public abstract class BufferedProto{}

	public class PaddingProto : BufferedProto {
		public readonly Option<byte> uuidOpt;
		public readonly Option<BufferedLength> lengthOpt;
		public readonly ByteBuffer loading;

		public PaddingProto (
			Option<byte> uuidOpt, 
			Option<BufferedLength> lengthOpt,
			ByteBuffer loading){
			this.uuidOpt = uuidOpt;
			this.lengthOpt = lengthOpt;
			this.loading = loading;
		}
	}

	public class CompletedProto : BufferedProto {
		public readonly byte uuid;
		public readonly int length;
		public readonly ByteBuffer loaded;

		public CompletedProto (
			byte uuid, 
			int length,
			ByteBuffer loaded){

			this.uuid = uuid;
			this.length = length;
			this.loaded = loaded;
		}
	}
}
