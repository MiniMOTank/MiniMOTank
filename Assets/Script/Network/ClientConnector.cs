using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
// State object for receiving data from remote device.
public class StateObject {
	// Client socket.
	public Socket workSocket = null;
	// Size of receive buffer.
	public const int BufferSize = 256;
	// Receive buffer.
	public byte[] buffer = new byte[BufferSize];
	// Received data string.
	public StringBuilder sb = new StringBuilder();
}

public class AsynchronousClient : MonoBehaviour {
	// The port number for the remote device.
	private const int port = 12010;

	// ManualResetEvent instances signal completion.
//	private static ManualResetEvent connectDone = 
//		new ManualResetEvent(false);
//	private static ManualResetEvent sendDone = 
//		new ManualResetEvent(false);
//	private static ManualResetEvent receiveDone = 
//		new ManualResetEvent(false);

	// The response from the remote device.
//	private static String response = String.Empty;

	public static void StartClient() {
		// Connect to a remote device.
		try {
			print("Connect to a remote device.");
			// Establish the remote endpoint for the socket.
			// The name of the 
			// remote device is "host.contoso.com".
			IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

			// Create a TCP/IP socket.
			Socket client = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);

			// Connect to the remote endpoint.
			client.BeginConnect( remoteEP, 
				new AsyncCallback(ConnectCallback), client);
//			connectDone.WaitOne();
			Thread.Sleep(1000);
			// Send test data to the remote device.

			print("Send test data to the remote device.");
			Send(client,"hello world , 你好，北京！");
//			sendDone.WaitOne();

			// Receive the response from the remote device.
			Receive(client);
//			receiveDone.WaitOne();

			// Write the response to the console.
//			Console.WriteLine("Response received : {0}", response);

//			Thread.Sleep(2000);
			// Release the socket.
			client.Shutdown(SocketShutdown.Both);
			client.Close();

		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}

	private static void ConnectCallback(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;

			// Complete the connection.
			client.EndConnect(ar);

			print("Socket connected to {0}" +
				client.RemoteEndPoint.ToString());

			// Signal that the connection has been made.
//			connectDone.Set();
		} catch (Exception e) {
			print(e.ToString());
		}
	}

	private static void Receive(Socket client) {
		try {
			// Create the state object.
			StateObject state = new StateObject();
			state.workSocket = client;

			// Begin receiving the data from the remote device.
			client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
				new AsyncCallback(ReceiveCallback), state);
		} catch (Exception e) {
			print(e.ToString());
		}
	}

	private static void ReceiveCallback( IAsyncResult ar ) {
		try {
			// Retrieve the state object and the client socket 
			// from the asynchronous state object.
			StateObject state = (StateObject) ar.AsyncState;
			Socket client = state.workSocket;

			// Read data from the remote device.
			int bytesRead = client.EndReceive(ar);

			if (bytesRead > 0) {
				// There might be more data, so store the data received so far.
				state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));

				// Get the rest of the data.
				client.BeginReceive(state.buffer,0,StateObject.BufferSize,0,
					new AsyncCallback(ReceiveCallback), state);
			} else {//todo ???????
				// All the data has arrived; put it in response.
				if (state.sb.Length > 1) {
//					response = state.sb.ToString();
				}
				client.EndReceive(ar);
				// Signal that all bytes have been received.
//				receiveDone.Set();
			}
		} catch (Exception e) {
			print(e.ToString());
		}
	}

	private static void Send(Socket client, String data) {
		// Convert the string data to byte data using UTF-8 encoding.
		byte[] byteUID = new byte[1]{1};
		byte[] load = Encoding.UTF8.GetBytes(data);
		byte[] length = ToBigEnd (BitConverter.GetBytes (load.Length));

		byte[] byteData = concat (concat (byteUID, length), load);
		// Begin sending the data to the remote device.
		client.BeginSend(byteData, 0, byteData.Length, 0,
			new AsyncCallback(SendCallback), client);
	}

	/// <summary>
	/// </summary>
	/// <returns>The big end.</returns>
	/// int type to data
	private static byte[] ToBigEnd(byte[] src) {
		if (BitConverter.IsLittleEndian){
			Array.Reverse(src);
		}
		return src;
	}

	private static byte[] concat(byte[] first, byte[] second) {
		byte[] final = new byte[first.Length + second.Length];
		Array.Copy(first, 0, final, 0, first.Length);
		Array.Copy(second, 0, final, first.Length, second.Length);
		return final;
	}

	private static void SendCallback(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;

			// Complete sending the data to the remote device.
			int bytesSent = client.EndSend(ar);
			print("Sent {0} bytes to server." + bytesSent);

			// Signal that all bytes have been sent.
//			sendDone.Set();
		} catch (Exception e) {
			print(e.ToString());
		}
	}

	public static int Main(String[] args) {
		StartClient();
		return 0;
	}
}
