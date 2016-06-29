using System;

namespace Functional
{
	public delegate R Func<out R>();
	public delegate R Func<in A, out R>(A a);
	public delegate R Func<in A, in B, out R>(A a, B b);
	public delegate R Func<in A, in B, in C, out R>(A a, B b, C c);
	public delegate R Func<in A, in B, in C, in D, out R>(A a, B b, C c, D d);
	public delegate R Func<in A, in B, in C, in D, in E, out R>(A a, B b, C c, D d, E e);

	public delegate void Action<in A>(A a);
}

