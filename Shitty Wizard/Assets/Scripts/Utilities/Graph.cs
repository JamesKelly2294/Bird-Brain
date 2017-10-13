using System;
using System.Collections.Generic;

namespace ShittyWizzard.Utilities
{
	public class Graph<T>
	{

		List<Edge<Vertex<T>>> edgemap;
		List<Vertex<T>> vertexmap;

		public delegate float DistanceCalculation(Vertex<T> first, Vertex<T> second);

		public Graph (List<T> data, DistanceCalculation d)
		{
			edgemap = new List<Edge<Vertex<T>>>();
			vertexmap = new List<Vertex<T>> ();

			for (int i = 0; i < data.Count; i++) {
				vertexmap.Add (new Vertex<T> (data [i]));
			}

			for (int i = 0; i < data.Count; i++) {
				T first = data [i];
				Vertex<T> firstV = vertexmap.Find (x => x.data.Equals (first));
				for (int j = i + 1; j < data.Count; j++) {
					T second = data [j];
					Vertex<T> secondV = vertexmap.Find (x => x.data.Equals (second));

					float distance = d (firstV, secondV);
					Edge<Vertex<T>> e = new Edge<Vertex<T>> (
						firstV, secondV, distance
					);
					edgemap.Add (e);
				}
			}
		}

		public List<Edge<Vertex<T>>> MinimumSpanningTree {
			get {
				return GenerateMinimumSpanningTree ();
			}
		}

		public List<Edge<Vertex<T>>> GenerateMinimumSpanningTree ()
		{
			// sort edges in ascending order
			edgemap.Sort ((first, second) => {
				float res = first.weight - second.weight;
				if (res > 0) {
					return 1;
				} else if (res < 0) {
					return -1;
				} else {
					return 0;
				}
			});

			Dictionary<int, int> parents = new Dictionary<int, int> ();
			foreach (Vertex<T> v in vertexmap) {
				parents [v.ID] = -1;
			}

			List<Edge<Vertex<T>>> MST = new List<Edge<Vertex<T>>> ();
			foreach (Edge<Vertex<T>> e in edgemap) {
				int x = Find (parents, e.first.ID);
				int y = Find (parents, e.second.ID);

				if (x == y) {
					// cycle detected, don't add edge
					continue;
				}

				Union (parents, e.first, e.second);
				MST.Add (e);

				if (MST.Count >= vertexmap.Count - 1) {
					break;
				}
			}

			return MST;
		}

		int Find (Dictionary<int, int> parents, int v)
		{
			if (parents [v] == -1) {
				return v;
			}
			return Find (parents, parents [v]);
		}

		void Union (Dictionary<int, int> parents, Vertex<T> first, Vertex<T> second)
		{
			int firstParent = Find (parents, first.ID);
			int secondParent = Find (parents, second.ID);

			parents [firstParent] = secondParent;
		}

	}
}

