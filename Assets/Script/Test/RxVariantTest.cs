using UnityEngine;
using System.Collections;
using UniRx;

public class RxVariantTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Subject<Dog> sub = new Subject<Dog> ();
		sub.Subscribe ((x) => {
			print("x - " + x);
		});

		sub.OnNext (new Dog());

		//Note Observable do NOT support (in)variant
//		IObservable<Animal> ani = sub.Select ((x) => {
		IObservable<Cat> ani = sub.Select ((x) => {
			return new Cat();
		});
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

class Animal {}
class Dog: Animal {}
class Cat: Animal {}