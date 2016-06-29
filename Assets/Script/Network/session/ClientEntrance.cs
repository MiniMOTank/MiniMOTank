using System.Collections;
using UniRx;
using System;
using System.Net;
using System.Net.Sockets;

namespace Lorance.RxScoket.Session {
	public class ClientEntrance {
		private string remoteHost;
		private int remotePort;
		public ClientEntrance(string remoteHost, int remotePort) {
			this.remoteHost = remoteHost;
			this.remotePort = remotePort;
		}

		public IObservable<ConnectedSocket> Connect() {
			IPHostEntry ipHostInfo = Dns.GetHostEntry(remoteHost);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, remotePort);

			// Create a TCP/IP socket.
			Socket client = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);

			var connectSub = new ReplaySubject<ConnectedSocket>();

//			IObservable<ConnectedSocket> connectObv = Observable.Create (new Func<IObserver<ConnectedSocket>, IDisposable>())
			// Connect to the remote endpoint.
			client.BeginConnect( remoteEP, 
				new AsyncCallback(ConnectCallback), new ConnectObject(connectSub, client));

			return connectSub.AsObservable();
		}

		private class ConnectObject {
			public ReplaySubject<ConnectedSocket> connectObv;
			public Socket client;

			public ConnectObject(
				ReplaySubject<ConnectedSocket> connectObv,
				Socket client) {
				this.connectObv = connectObv;
				this.client = client;
			}
		}

		private static void ConnectCallback(IAsyncResult ar) {
			try {
				// Retrieve the socket from the state object.
				ConnectObject connectObject = (ConnectObject) ar.AsyncState;

				// Complete the connection.
				connectObject.client.EndConnect(ar);
				connectObject.connectObv.OnNext(new ConnectedSocket(connectObject.client));
				connectObject.connectObv.OnCompleted();
				Package.Log("Socket connected to {0}" +
					connectObject.client.RemoteEndPoint.ToString());
			} catch (Exception e) {
				Console.Write(e.ToString());
			}
		}
	}
}
