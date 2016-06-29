using System;
using System.Net.Sockets;
using System.Collections.Generic;
using UniRx;
using System.IO;
using System.Text;
using Lorance.RxSocket.Util;
using System.Threading;

namespace Lorance.RxScoket.Session {
	public class ConnectedSocket {
		private Socket socket;
		private Subject<CompletedProto> completedProtosSubj = new Subject<CompletedProto>();
		private Attachment readAttach;
		private ReaderDispatch readerDispatch = new ReaderDispatch();
		private Semaphore sendDone = new Semaphore(1, 1);
		public ConnectedSocket (Socket socket) {
			this.socket = socket;
			readAttach = new Attachment (new ByteBuffer(new byte[Configration.READBUFFER_LIMIT]), socket);
		}

		public IObservable<CompletedProto> startReading() {
			Package.Log ("beginReading - ");
			beginReadLoop ();
			return completedProtosSubj.AsObservable();
		}

		public void send(ByteBuffer data) {
			send (data.Bytes);
		}

		public void send(byte[] data) {
			sendDone.WaitOne();
			socket.BeginSend(data, 0, data.Length, SocketFlags.None,
				new AsyncCallback((ar) => {
					try {
						// Retrieve the socket from the state object.
						Socket skt = (Socket) ar.AsyncState;

						// Complete sending the data to the remote device.
						int bytesSent = skt.EndSend(ar);
						Package.Log("Sent" + bytesSent + " bytes.", 70);

						// Signal that all bytes have been sent.
						sendDone.Release();
					} catch (Exception e) {
						Package.Log("send data fail - " + e.ToString());
					}
				}), socket);
		}

		private void beginReadLoop() {
			read(readAttach).onComplete((ach) => {
				Package.Log("read bytes completed - " + ach);
				var protosOpt = readerDispatch.receive(ach.byteBuffer);
				protosOpt.Foreach( (protos) => {
					foreach(CompletedProto proto in protos) {
						completedProtosSubj.OnNext(proto);
					}
				});
				beginReadLoop();
			});
		}

		private Future<Attachment> read(Attachment readAttach) {
			Future<Attachment> f = new Future<Attachment> ();
			socket.BeginReceive (readAttach.byteBuffer.Bytes, 0, readAttach.byteBuffer.Bytes.Length, 0,
				ar => { try {
						Attachment state = (Attachment) ar.AsyncState;
						Socket client = state.client;
						// Read data from the remote device.
						int bytesRead = client.EndReceive(ar);
						if (bytesRead > 0) {
							f.completeWith(() => readAttach);
						}
					} catch (Exception e) {
						Package.Log(e.ToString());
					}
				},
				readAttach);
			return f;
		}
	}
}
