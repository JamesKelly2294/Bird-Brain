using System;
using System.Collections.Generic;

namespace ShittyWizzard.Utilities
{
	public class Vertex<T>
	{
		public T data;

		public HashSet<Vertex<T>> neighbors;

		public Vertex (T data)
		{
			this.data = data;
			this.neighbors = new HashSet<Vertex<T>> ();
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

		public bool Visited = false;

		public override int GetHashCode ()
		{
			return data.GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ("Vertex ({0})", data);
		}
	}
}

