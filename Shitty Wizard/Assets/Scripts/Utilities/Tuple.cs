using System;

namespace ShittyWizzard.Utilities
{
	public class Tuple<T1, T2>
	{
		public T1 Item1;

		public T2 Item2;

		public Tuple(T1 one, T2 two) {
			Item1 = one;
			Item2 = two;
		}

		public override int GetHashCode() {
			return Item1.GetHashCode () * 100000 + Item2.GetHashCode ();
		}

		public override bool Equals (System.Object obj)
		{
			if (obj == null || GetType () != obj.GetType ())
				return false;

			Tuple<T1, T2> v = (Tuple<T1, T2>)obj;
			return this.GetHashCode() == v.GetHashCode();
		}
	}
}

