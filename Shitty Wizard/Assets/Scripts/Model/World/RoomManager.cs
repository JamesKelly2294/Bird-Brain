using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShittyWizzard.Utilities;
using ShittyWizzard.Model;
using System.Text.RegularExpressions;
using System.Linq;

namespace ShittyWizard.Model.World
{
	public class RoomManager: Manager<ShittyWizzard.Utilities.Event>
	{
		public Map Map;


		int _width = -1;
		public int Width {
			get {
				if (_width == -1) {
					int minX = Rooms [0].MinX;
					int maxX = Rooms [0].MaxX;
					foreach (Room r in Rooms) {
						if (r.MinX < minX)
							minX = r.MinX;
						if (r.MaxX > maxX)
							maxX = r.MaxX;
					}
					_width = maxX - minX;
				}
				return _width;
			}
		}

		int _height = -1;
		public int Height {
			get {
				if (_height == -1) {
					int minY = Rooms [0].MinY;
					int maxY = Rooms [0].MaxY;
					foreach (Room r in Rooms) {
						if (r.MinY < minY)
							minY = r.MinY;
						if (r.MaxY > maxY)
							maxY= r.MaxY;
					}
					_height = maxY - minY;
				}
				return _height;
			}
		}

		public List<RoomData> Rooms;
		public Dictionary<Tuple<int, int>, List<Room>> RoomPrototypes;
		public List<Edge<Vertex<Room>>> RoomGraph;

		int numberOfRooms;
		int max_iterations = 200;
		int iterations = 0;

		public RoomManager(Map map, int numberOfRooms) {
			this.Map = map;
			this.numberOfRooms = numberOfRooms;

			LoadRooms ();
			GenerateRooms ();
		}

		void LoadRooms() {
			RoomPrototypes = new Dictionary<Tuple<int, int>, List<Room>> ();

			var roomAssets = Resources.LoadAll("Rooms", typeof(TextAsset));
			Regex regex = new Regex (System.Environment.NewLine);
			for (int i = 0; i < roomAssets.Length; i++) {
				TextAsset roomAsset = (TextAsset)roomAssets [i];
				string[] lines = regex.Split (roomAsset.text);
				int h = lines.Length - 1;
				int w = 0;
				foreach (string line in lines) {
					if (line.Length > w) {
						w = line.Length;
					}
				}

				int x = 0;
				int y = h - 1;
				RoomData r = new RoomData (0, 0, w, h);
				foreach (string line in lines) {
					x = 0;
					foreach (char c in line) {
						switch (c) {
						case '#':
							r._tiles [x, y] = TileType.Wall;
							break;
						case '.':
							r._tiles [x, y] = TileType.Floor;
							break;
						default:
							r._tiles [x, y] = TileType.Empty;
							break;
						}
						x++;
					}
					y--;
				}
				Tuple<int, int> key = new Tuple<int, int>(w, h);
				if (!RoomPrototypes.ContainsKey (key)) {
					RoomPrototypes [key] = new List<Room> ();
				}
				RoomPrototypes [key].Add(r);
			}
		}

		void GenerateRooms() {
			_width = -1;
			_height = -1;
			Rooms = new List<RoomData> ();
			iterations = 0;

			var keys = new List<Tuple<int, int>> (RoomPrototypes.Keys);

			// Add a bunch of small rooms to push the larger rooms around.
			// We remove this later on...
//			for (int i = 0; i < numberOfRooms*3; i++) {
//				Tuple<int, int> key =  keys[Random.Range (0, RoomPrototypes.Keys.Count)];
//				RoomData tempRoom = new RoomData (
//					Random.Range(-15, 15),
//					Random.Range(-15, 15),
//					5, 
//					5
//				);
//				Rooms.Add (tempRoom);
//			}

			for (int i = 0; i < numberOfRooms; i++) {
				Tuple<int, int> key =  keys[Random.Range (0, RoomPrototypes.Keys.Count)];
				RoomData tempRoom = new RoomData (
					Random.Range(-25, 25),
					Random.Range(-25, 25),
					key.Item1, 
					key.Item2
				);
				List<Room> lst = RoomPrototypes [key];
				int random = Random.Range (0, (int)lst.Count);
				tempRoom._tiles = lst [random].Tiles;
				Rooms.Add (tempRoom);
			}


			while (iterations < max_iterations) {
				if (!SeparateRooms ()) {
					break;
				}
				iterations += 1;
			}

//			// remove the smaller rooms
//			Rooms.RemoveRange (0, numberOfRooms * 3);

			// remove the overlapping rooms
			RemoveOverlappingRooms ();

			Graph<RoomData> g = new Graph<RoomData>(Rooms, (x,y) => {
				return Vector2.Distance(x.data.Center, y.data.Center);
			});


			RoomGraph = new List<Edge<Vertex<Room>>> ();
			foreach(var e in g.MinimumSpanningTree) {
				Vertex<Room> first = new Vertex<Room> ((Room)e.first.data);
				Vertex<Room> second = new Vertex<Room> ((Room)e.second.data);
				Edge<Vertex<Room>> edge = new Edge<Vertex<Room>> (first, second, e.weight);
				edge.id = e.id;
				RoomGraph.Add (edge);
			}

			// add some edges back in to introduce loops
//			List<int> randomIndices = new List<int> ();
//			for (int i = 0; i < g.edgemap.Count; i++) {
//				randomIndices.Add (i);
//			}
//			randomIndices.OrderBy (a => Random.value);
			// THIS IS SO SLOW.
			// USE DELANY TRIANGULATION FOR EFFICIENT GRAPH BUILDING
//			float percentEdgesToAddBackIn = 0.75f;
//			int edgesToAddBackIn = (int)(percentEdgesToAddBackIn * g.MinimumSpanningTree.Count);
//			List<Edge<Vertex<RoomData>>> readdedEdges = new List<Edge<Vertex<RoomData>>> ();
//			for (int i = 0; i < g.edgemap.Count; i++) {
//				Edge<Vertex<RoomData>> first = g.edgemap [randomIndices[i]];
//				bool canAddEdge = true;
//				for (int j = 0; j < RoomGraph.Count; j++) {
//					Edge<Vertex<Room>> second = RoomGraph [j];
//
//					if (first.first.data == second.first.data) {
//						continue;
//					}
//
//					if (first.id == second.id) {
//						continue;
//					}
//
//					if (RoomEdgesIntersect (first.first.data, first.second.data, second.first.data, second.second.data)) {
//						canAddEdge = false;
//						break;
//					}
//				}
//
//				if (canAddEdge) {
//					readdedEdges.Add (first);
//				}
//			}
//			readdedEdges.OrderBy (a => a.weight);
//			for (int i = 0; i < readdedEdges.Count && i < edgesToAddBackIn; i++) {
//				Edge<Vertex<RoomData>> e = readdedEdges[i];
//				Vertex<Room> first = new Vertex<Room> ((Room)e.first.data);
//				Vertex<Room> second = new Vertex<Room> ((Room)e.second.data);
//				Edge<Vertex<Room>> edge = new Edge<Vertex<Room>> (first, second, e.weight);
//				RoomGraph.Add (edge);
//				Debug.Log ("ADDED EDGE " + i);
//				Debug.Log (edge);
//			}

			// I don't like doing this, because we are wasting a lot of
			// memory on empty tiles. In the future, it would be a good idea
			// to have localized tilemaps on a per room/per hallway basis

			int minY = Rooms [0].MinY;
			int minX = Rooms [0].MinX;
			foreach (Room r in Rooms) {
				if (r.MinY < minY)
					minY = r.MinY;
				if (r.MinX < minX)
					minX = r.MinX;
			}

			Dictionary<string, object> payload = new Dictionary<string, object> ();
			payload["width"] = Width;
			payload["height"] = Height;
			EmitEvent (GenericEventType.INITIALIZED, new ShittyWizzard.Utilities.Event(payload));
			foreach (RoomData r in Rooms) {
				r.Position = new Vector2(r.MinX - minX, r.MinY - minY);
				payload = new Dictionary<string, object> ();
				payload ["room"] = r;
				EmitEvent (GenericEventType.CREATED, new ShittyWizzard.Utilities.Event(payload));
			}
		}

		bool ShareRoom(Room first, Room second, Room third, Room fourth) {
			return first == third || first == fourth || second == third || second == fourth;
		}

		// inspired by code available at http://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
		int Orientation(Room p, Room q, Room r) {
			int orientation = (q.CenterY - p.CenterY) * (r.CenterX - q.CenterX) -
			          (q.CenterX - p.CenterX) * (r.CenterY - q.CenterY);

			if (orientation == 0) {
				return 0;
			}

			return (orientation > 0) ? 1 : 2;
		}

		bool RoomEdgesIntersect(Room p1, Room q1, Room p2, Room q2) {
			int o1 = Orientation (p1, q1, p2);
			int o2 = Orientation (p1, q1, q2);
			int o3 = Orientation (p2, q2, p1);
			int o4 = Orientation (p2, q2, q1);

			// handles general case for intesection
			if (o1 != o2 && o3 != o4) {
				return true;
			}

			return false;
		}

		// end borrowed code

		void RemoveOverlappingRooms ()
		{
			List<RoomData> temp = new List<RoomData> ();
			for (int i = 0; i < Rooms.Count; i++) {
				RoomData first = Rooms [i];

				// this is inefficient.
				// papa james doesn't care.
				bool doesNotOverlap = true;
				for (int j = 0; j < Rooms.Count; j++) {
					if (i == j)
						continue;
					RoomData second = Rooms [j];

					if (second.MaxX - first.MinX <= 0.0f
					               || first.MaxX - second.MinX <= 0.0f
					               || second.MaxY - first.MinY <= 0.0f
					               || first.MaxY - second.MinY <= 0.0f) {
						continue;
					}
					doesNotOverlap = false;
					break;
				}

				if (doesNotOverlap) {
					temp.Add (first);
				}
			}
			Rooms = temp;
		}

		bool SeparateRooms ()
		{
			Dictionary<RoomData, Vector2> dict = new Dictionary<RoomData, Vector2> ();
			bool mustSeparate = false;

			for (int i = 0; i < Rooms.Count; i++) {
				RoomData first = Rooms [i];
				for (int j = i + 1; j < Rooms.Count; j++) {
					RoomData second = Rooms [j];
					Vector2 axis = Vector2.zero;

					Vector2 penetrationVector = Vector2.zero;

					float x0 = second.MaxX - first.MinX;
					float x1 = first.MaxX - second.MinX;

					if (x0 <= 0.0f || x1 <= 0.0f) {
						continue;
					}

					float temp = (x0 < x1) ? x0 : -x1;
					penetrationVector += Vector2.right * temp;

					float y0 = second.MaxY - first.MinY;
					float y1 = first.MaxY - second.MinY;


					if (y0 <= 0.0f || y1 <= 0.0f) {
						continue;
					}

					temp = (y0 < y1) ? y0 : -y1;
					penetrationVector += Vector2.up * temp;

					if (dict.ContainsKey (first)) {
						Vector2 pen = dict [first];
						dict [first] -= penetrationVector;
					} else {
						dict [first] = -penetrationVector;
					}

					if (dict.ContainsKey (second)) {
						dict [second] += penetrationVector;
					} else {
						dict [second] = penetrationVector;
					}

					mustSeparate = true;
				}
			}

			for (int i = 0; i < Rooms.Count; i++) {
				if (!dict.ContainsKey (Rooms [i])) {
					continue;
				}

				Rooms [i].Position = Rooms [i].Position - dict [Rooms [i]].normalized;
			}

			return mustSeparate;
		}
	}

	public class RoomData : Room
	{

		protected int _minX;
		protected int _minY;

		protected int _width;
		protected int _height;

		public TileType[,] _tiles;

		public int MinX {
			get {
				return _minX;
			}
		}

		public int MaxX {
			get {
				return _minX + _width;
			}
		}

		public int MinY {
			get {
				return _minY;
			}
		}

		public int MaxY {
			get {
				return _minY + _height;
			}
		}

		public int Width {
			get {
				return _width;
			}
		}

		public int Height {
			get {
				return _height;
			}
		}

		public TileType[,] Tiles { 
			get {
				return _tiles;
			}
		}

		public Vector2 Center {
			get {
				return new Vector2 (MinX + _width / 2.0f, MinY + _height / 2.0f);
			}
		}

		public int CenterX {
			get {
				return Mathf.RoundToInt (Center.x);
			}
		}

		public int CenterY {
			get {
				return Mathf.RoundToInt (Center.y);
			}
		}

		public Vector2 Position {
			get {
				return new Vector2 (MinX, MinY);
			}
			set {
				_minX = Mathf.RoundToInt (value.x);
				_minY = Mathf.RoundToInt (value.y);
			}
		}

		public override string ToString ()
		{
			return string.Format ("{0}", Center);
		}

		public RoomData (int minX, int minY, int width, int height)
		{
			this._minX = minX;
			this._minY = minY;

			this._width = width;
			this._height = height;

			this._tiles = new TileType[width, height];
		}
	}
}