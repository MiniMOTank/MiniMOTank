using UnityEngine;
using System.Collections;
using System;
using System.Threading;

namespace Lorance.RxSocket.Util {
	
	/**
	 * simple future warp system async callback
	 * */
	public class Future<T> {
		private Action<T> callBack;
		private T value;

		public Future() {}

		public Future(Func<T> cb) {
			completeWith(cb);
		}

		public void onComplete(Action<T> func) {
			callBack += func;
		}
			
		public void completeWith (Func<T> value) {
			this.value = value();
			if(callBack != null) callBack(this.value);
		}

		public Future<U> map<U>(Func<T, U> f){
			Future<U> p = new Future<U> ();
			this.onComplete ((val) => p.completeWith(() => f(val)));//val equals this.value
			return p;
		}

		public Future<U> flatMap<U>(Func<T, Future<U>> f){
			Future<U> p = new Future<U>();
			this.onComplete ((val) => p = f(val));
			return p;
		}
	}
}