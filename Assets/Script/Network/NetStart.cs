using UnityEngine;
using System.Collections;
using System;
using System.Text;
using Lorance.RxScoket.Session;
using UniRx;
using Lorance.RxScoket;

public class NetStart : MonoBehaviour {
	ClientEntrance client;
	IObservable<ConnectedSocket> socket;
	IObservable<CompletedProto> readObv;

	// Use this for initialization
	void Start () {
//		AsynchronousClient.StartClient();
		client = new ClientEntrance("localhost", 10127);
		socket = client.Connect();
		readObv = socket.SelectMany ((x) => {
			return x.startReading();
		});

		readObv.Subscribe ((x) => {
			print("completed proto - " + x);
		});

		socket.Subscribe ((x) => {
			print("connect~");
			x.send(new ByteBuffer(Common.readyData((byte)1, "hi server ~")));
		});
	}
}
