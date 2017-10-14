using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShittyWizzard.Utilities;
using ShittyWizzard.Model;
using System.Text.RegularExpressions;

namespace ShittyWizard.Model.World
{
	public class RoomManager: Manager<ShittyWizzard.Utilities.Event>
	{
		public Map Map;

		public Dictionary<int, string> test;

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

		int numberOfRooms = 20;
		int max_iterations = 200;
		int iterations = 0;

		public RoomManager(Map map, int numberOfRooms, int minDim, int maxDim) {
			this.Map = map;
			this.numberOfRooms = numberOfRooms;

			RoomPrototypes = new Dictionary<Tuple<int, int>, List<Room>> ();

			var roomAssets = Resources.LoadAll("Rooms", typeof(TextAsset));
			Regex regex = new Regex ("\n");
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

			GenerateRooms (minDim, maxDim);

		}

		void GenerateRooms(int minDim, int maxDim) {
			_width = -1;
			_height = -1;
			Rooms = new List<RoomData> ();
			iterations = 0;

			for (int i = 0; i < numberOfRooms; i++) {
				RoomData tempRoom = new RoomData (
					                (int)Random.Range (-10, 10), (int)Random.Range (-10, 10),
					                (int)Random.Range (minDim, maxDim), (int)Random.Range (minDim, maxDim)
				                );
				Tuple<int, int> key = new Tuple<int, int> ((int)tempRoom.Width,
					                      (int)tempRoom.Height);
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

			RemoveOverlappingRooms ();

			Graph<RoomData> g = new Graph<RoomData>(Rooms, (x,y) => {
				return Vector2.Distance(x.data.Center, y.data.Center);
			});

			var mst = g.MinimumSpanningTree;

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