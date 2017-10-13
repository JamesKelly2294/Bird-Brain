using System;

namespace ShittyWizzard.Utilities
{
	public class Vertex<T>
	{
		public T data;

		public Vertex (T data)
		{
			this.data = data;
		}

		public int ID {
			get {
				return GetHashCode ();
			}
		}

		public override bool Equals (System.Object obj)
		{
			if (obj == null || GetType () != obj.GetType ())
				return false;

			Vertex<T> v = (Vertex<T>)obj;
			return this.ID == v.ID;
		}

		public override int GetHashCode ()
		{
			return data.GetHashCode ();
		}
	}
}

