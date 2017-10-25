using System;

namespace ShittyWizzard.Utilities
{
	public class Edge<T>
	{
		public T first;
		public T second;

		public float weight;

		private static int curId = 0;

		public Edge (T first, T second, float weight)
		{
			this.first = first;
			this.second = second;
			this.weight = weight;
			this.id = curId++;
		}

		public override string ToString ()
		{
			return string.Format ("({0}) [{1} <-> {2}]", weight, first, second);
		}

		public int id;
	}
}

