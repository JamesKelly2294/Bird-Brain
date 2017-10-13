using System;

namespace ShittyWizzard.Utilities
{
	public class Edge<T>
	{
		public T first;
		public T second;

		public float weight;

		public Edge (T first, T second, float weight)
		{
			this.first = first;
			this.second = second;
			this.weight = weight;
		}

		public override string ToString ()
		{
			return string.Format ("({0}) [{1} <-> {2}]", weight, first, second);
		}
	}
}

