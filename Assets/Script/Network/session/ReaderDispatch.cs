using UnityEngine;
using System.Collections.Generic;
using Lorance.RxScoket.Util;
using Lorance.RxScoket;

namespace Lorance.RxScoket.Session{
	class TmpBufferOverLoadException : RuntimeException{
		public TmpBufferOverLoadException(string message)
			: base(message)
		{
		}
		public TmpBufferOverLoadException(){
		}
	}

	public class ReaderDispatch {
		private int maxLength;
		private PaddingProto tmpProto;
		private static None<byte> NoneByte = new None<byte> ();
		private static None<CompletedProto> NoneProto = new None<CompletedProto> ();
		public ReaderDispatch(PaddingProto tmpProto, int maxLength) {
			this.tmpProto = tmpProto;
			this.maxLength = maxLength;
		}

		public ReaderDispatch(PaddingProto tmpProto) {
			maxLength = Configration.TEMPBUFFER_LIMIT;
		}

		public ReaderDispatch(){
			this.tmpProto = new PaddingProto (new None<byte>(), new None<BufferedLength>(), new ByteBuffer(new byte[0]));
		}

		public IOption<Queue<CompletedProto>> receive(ByteBuffer src){
			src.Flip ();
			var rst = receiveHelper (src, new None<Queue<CompletedProto>>());
			src.Clear ();
			return rst;
		}

		private IOption<Queue<CompletedProto>> receiveHelper(ByteBuffer src, IOption<Queue<CompletedProto>> completes) {
			if (tmpProto.uuidOpt is None<byte>) {
				var uuidOpt = tryGetByte (src);
				tmpProto = new PaddingProto (uuidOpt, new None<BufferedLength> (), null);
				var lengthOpt = uuidOpt.FlatMap<BufferedLength> ((byte uuid) => {
					return TryGetLength (src, new None<PendingLength> ());
				});

//				IOption<CompletedProto> protoOpt = Option<CompletedProto>.Empty;
				IOption<CompletedProto> protoOpt = lengthOpt.FlatMap<CompletedProto> ((lengthVal) => {
					if (lengthVal is CompletedLength) {
						int length = lengthVal.Value ();
						if (length > maxLength) {
							throw new TmpBufferOverLoadException ("length - " + length);
						} else if (src.Remaining () < length) {
							var newBf = new ByteBuffer (length);
							tmpProto = new PaddingProto (uuidOpt, lengthOpt, newBf.Put (src));
							return Option<CompletedProto>.Empty;
						} else {
							tmpProto = new PaddingProto (Option<byte>.Empty, Option<BufferedLength>.Empty, new ByteBuffer (0));
							var newAf = new byte[length];
							src.Get (newAf, 0, length);
							var completed = new CompletedProto (uuidOpt.Get (), length, new ByteBuffer (newAf));
							return new Some<CompletedProto> (completed);
						}
					} else {
//						var lengthPending = (PendingLength)lengthVal;
//						byte[] arrived = lengthPending.arrived;
//						int number = lengthPending.arrivedNumber;
						tmpProto = new PaddingProto (uuidOpt, lengthOpt, new ByteBuffer (0));
						return Option<CompletedProto>.Empty;
					}
				});

				if (protoOpt is None<CompletedProto>) {
					return completes;
				} else {
					var completed = (CompletedProto)protoOpt.Get ();
					if (completes.IsEmpty ()) {
						var completedQueue = new Queue<CompletedProto> ();
						completedQueue.Enqueue (completed);
						return receiveHelper (src, new Some<Queue<CompletedProto>> (completedQueue));
					} else {
						return receiveHelper (src, completes.Map ((a) => {
							a.Enqueue (completed);
							return a;
						}));
					}
				}
			} else if (!tmpProto.uuidOpt.IsEmpty () && tmpProto.lengthOpt.IsEmpty ()) {
				var uuid = tmpProto.uuidOpt.Get ();
				var lengthOpt = TryGetLength (src, new None<PendingLength> ());
				var protoOpt = lengthOpt.FlatMap<CompletedProto> ((lengthValue) => {
					if (lengthValue is CompletedLength) {
//						var length = lengthValue.Value ();
						tmpProto = new PaddingProto (new Some<byte> (uuid), lengthOpt, new ByteBuffer (0));
						return ReadLoad (src, tmpProto);
					} else { //if (tmpProto is PaddingProto) {
						tmpProto = new PaddingProto (new Some<byte> (uuid), lengthOpt, new ByteBuffer (0));
						return NoneProto;
					}
				});

				if (protoOpt is None<CompletedProto>) {
					return completes;
				} else {
					var completed = (CompletedProto)protoOpt.Get ();
					if (completes.IsEmpty ()) {
						var completedQueue = new Queue<CompletedProto> ();
						completedQueue.Enqueue (completed);
						return receiveHelper (src, new Some<Queue<CompletedProto>> (completedQueue));
					} else {
						return receiveHelper (src, completes.Map ((a) => {
							a.Enqueue (completed);
							return a;
						}));
					}
				}
			} else if (!tmpProto.uuidOpt.IsEmpty () && !(tmpProto.lengthOpt is Some<PendingLength>)) {
				var uuid = tmpProto.uuidOpt.Get ();
				var pending = (PendingLength)tmpProto.lengthOpt.Get ();
				var lengthOpt = TryGetLength (src, new Some<PendingLength> (pending));
				var protoOpt = lengthOpt.FlatMap<CompletedProto> ((lengthValue) => {
					if (lengthValue is CompletedLength) {
//						var length = lengthValue.Value ();
						var lengthedProto = new PaddingProto (new Some<byte> (uuid), lengthOpt, new ByteBuffer (0));
						return ReadLoad (src, lengthedProto);
					} else {
						tmpProto = new PaddingProto (new Some<byte> (uuid), lengthOpt, new ByteBuffer (0));
						return NoneProto;
					}
				});

				if (protoOpt is None<CompletedProto>) {
					return completes;
				} else {
					var completed = (CompletedProto)protoOpt.Get ();
					if (completes.IsEmpty ()) {
						var completedQueue = new Queue<CompletedProto> ();
						completedQueue.Enqueue (completed);
						return receiveHelper (src, new Some<Queue<CompletedProto>> (completedQueue));
					} else {
						return receiveHelper (src, completes.Map ((a) => {
							a.Enqueue (completed);
							return a;
						}));
					}
				}
			} else { //CompletedLength
				var uuid = tmpProto.uuidOpt.Get ();
				var lengthOpt = tmpProto.lengthOpt;
				var length = lengthOpt.Get().Value ();
				var padding = tmpProto.loading;
				IOption<CompletedProto> protoOpt = null;
				if(padding.Position + src.Remaining() < length){
					tmpProto = new PaddingProto (new Some<byte>(uuid), lengthOpt, padding.Put(src));
					protoOpt = NoneProto;
				} else {
					tmpProto = new PaddingProto (new None<byte> (), new None<BufferedLength> (), new ByteBuffer (0));
					var needLength = length - padding.Position;
					var newAf = new byte[needLength];
					src.Get (newAf, 0, needLength);

					var completed = new CompletedProto (uuid, length, padding.Put (newAf));
					protoOpt = new Some<CompletedProto> (completed);
				}

				if (protoOpt is None<CompletedProto>) {
					return completes;
				} else {
					var completed = (CompletedProto)protoOpt.Get ();
					if (completes.IsEmpty ()) {
						var completedQueue = new Queue<CompletedProto> ();
						completedQueue.Enqueue (completed);
						return receiveHelper (src, new Some<Queue<CompletedProto>> (completedQueue));
					} else {
						return receiveHelper (src, completes.Map ((a) => {
							a.Enqueue (completed);
							return a;
						}));
					}
				}
			}
//			return new None<Queue<CompletedProto>> ();
		}

		private Option<byte> tryGetByte(ByteBuffer bf) {
			if (bf.Remaining () >= 1)
				return new Some<byte> (bf.Get ());
			else 
				return NoneByte;
		}

		private IOption<BufferedLength> TryGetLength (ByteBuffer bf, IOption<PendingLength> lengthOpt) {
			var remaining = bf.Remaining ();

			if (lengthOpt is None<PendingLength>) {
				if (remaining < 1)
					return new None<PendingLength> ();
				else if (1 <= remaining && remaining < 4) {
					byte[] lengthByte = new byte[4];
					bf.Get (lengthByte, 0, remaining);
					return new Some<PendingLength> (new PendingLength (lengthByte, remaining));
				} else {
					var length = bf.GetInt ();
					return new Some<CompletedLength> (new CompletedLength (length));
				}
			} else {
				var pendingLength = lengthOpt.Get ();
				int need = 4 - pendingLength.arrivedNumber;
				if (remaining >= need) {
					bf.Get (pendingLength.arrived, pendingLength.arrivedNumber, remaining);
					return new Some<CompletedLength> (new CompletedLength(Common.ToInt (pendingLength.arrived)));
				} else {
					bf.Get (pendingLength.arrived, pendingLength.arrivedNumber, remaining);
					pendingLength.arrivedNumber += remaining;
					return lengthOpt;
				}
			}
		}

		//require(paddingProto.lengthOpt.isDefined)
		//require(paddingProto.lengthOpt.get.isInstanceOf[CompletedLength])
		private IOption<CompletedProto> ReadLoad(ByteBuffer src, PaddingProto paddingProto){
			int length = paddingProto.lengthOpt.Get ().Value();
			if (length > maxLength)
				throw new TmpBufferOverLoadException ();
			if (src.Remaining () < length) {
				var newBf = new ByteBuffer (length);
				tmpProto = new PaddingProto (paddingProto.uuidOpt, paddingProto.lengthOpt, newBf.Put (src));
				return NoneProto;
			} else {
				tmpProto = new PaddingProto (NoneByte, new None<BufferedLength>(), new ByteBuffer(0));
				var newAf = new byte[length];
				src.Get (newAf, 0, length);
				var completed = new CompletedProto (paddingProto.uuidOpt.Get (), length, new ByteBuffer(newAf));
				return new Some<CompletedProto> (completed);
			}
		}
	}
}
