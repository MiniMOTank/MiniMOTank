using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Lorance.RxScoket;

public class ClientConnectorSync : MonoBehaviour {
	public static void StartClient() {
		// Data buffer for incoming data.
//		byte[] bytes = new byte[1024];

		// Connect to a remote device.
		try {
			// Establish the remote endpoint for the socket.
			// This example uses port 11000 on the local computer.
			IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress,12010);

			// Create a TCP/IP  socket.
			Socket sender = new Socket(AddressFamily.InterNetwork, 
				SocketType.Stream, ProtocolType.Tcp );

			// Connect the socket to the remote endpoint. Catch any errors.
			try {
				sender.Connect(remoteEP);

				print("Socket connected to {0}" +
					sender.RemoteEndPoint.ToString());

				// Encode the data string into a byte array.
//				byte[] msg = Encoding.UTF8.GetBytes("This is a test<EOF>");

				byte protoGroup = 1;
				byte[] msg = Common.readyData(protoGroup, "This is a test<EOF>");

				// Send the data through the socket.
				sender.Send(msg);

				// Receive the response from the remote device.
//				int bytesRec = sender.Receive(bytes);
//				print("Echoed test = {0}" +
//					Encoding.UTF8.GetString(bytes,0,bytesRec));

				// Release the socket.
				sender.Shutdown(SocketShutdown.Both);
				sender.Close();
			} catch (ArgumentNullException ane) {
				print("ArgumentNullException : {0}" + ane.ToString());
			} catch (SocketException se) {
				print("SocketException : {0}" + se.ToString());
			} catch (Exception e) {
				print("Unexpected exception : {0}" + e.ToString());
			}

		} catch (Exception e) {
			print( e.ToString());
		}
	}

	// Use this for initialization
	void Start () {
		StartClient();
	}
}
