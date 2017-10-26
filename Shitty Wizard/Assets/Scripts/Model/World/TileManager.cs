using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ShittyWizzard.Model;
using ShittyWizzard.Utilities;
using System.Text.RegularExpressions;

namespace ShittyWizard.Model.World
{
	public class TileManager : Manager<Tile>
	{

		public Map Map { get; protected set; }

		private TileData[,] m_tiles;

		private int _width = -1;
		public int Width { get { return _width; } }

		private int _height = -1;
		public int Height { get { return _height; } }

		private Dictionary<TileType, List<Tile>> _typeToTileDict;

		public Tile GetRandomTileOfType(TileType t) {
			List<Tile> tiles = _typeToTileDict [t];
			return tiles [UnityEngine.Random.Range (0, tiles.Count)];
		}

		// warning: slow!
		public Tile GetRandomTileOfTypeInRoom(TileType t, Room r) {
			List<Tuple<int, int>> tilesOfType = new List<Tuple<int, int>> ();

			for (int x = 0; x < r.Width; x++) {
				for (int y = 0; y < r.Height; y++) {
					if (r.Tiles [x, y] == t) {
						tilesOfType.Add (new Tuple<int, int> (x, y));
					}
				}
			}

			if (tilesOfType.Count <= 0) {
				return null;
			}

			var xyPair = tilesOfType [UnityEngine.Random.Range (0, tilesOfType.Count)];
			return GetTileAt(xyPair.Item1 + r.MinX, xyPair.Item2 + r.MinY);
		}

		private TileManager() {
			_width = 0;
			_height = 0;
		}

		public TileManager (Map map)
		{
			this.Map = map;
			_width = Map.RoomManager.Width;
			_height = Map.RoomManager.Height;

			SetupTiles ();
			SetupTypeToTileDict ();
		}

		public static TileManager TileManagerForBossMap(Map map) {
			TileManager tm = new TileManager ();
			tm._width = 18;
			tm._height = 18;

			tm.m_tiles = new TileData[tm._width, tm._height];

			for (int x = 0; x < tm._width; x++) {
				for (int y = 0; y < tm._height; y++) {
					tm.m_tiles [x, y] = (TileData)tm.CreateTile (x, y);
				}
			}

			for (int x = 0; x < tm._width; x++) {
				for (int y = 0; y < tm._height; y++) {
					if (x == 0 || x == tm._width - 1 || y == 0 || y == 1 || y == tm._height - 1 | y == tm._height - 2) {
						tm.m_tiles [x, y].Type = TileType.Wall;
					} else {
						tm.m_tiles [x, y].Type = TileType.Floor;
					}
				}
			}

			tm.SetupTypeToTileDict ();

			return tm;
		}

		private void SetupTypeToTileDict() {
			_typeToTileDict = new Dictionary<TileType, List<Tile>> ();

			Array values = Enum.GetValues(typeof(TileType));
			foreach (TileType tt in values) {
				_typeToTileDict [tt] = new List<Tile> ();
			}


			for (int x = 0; x < _width; x++) {
				for (int y = 0; y < _height; y++) {
					Tile t = GetTileAt (x, y);
					_typeToTileDict [t.Type].Add(t);
				}
			}
		}

		private void SetupTiles ()
		{
			m_tiles = new TileData[_width, _height];

			for (int x = 0; x < _width; x++) {
				for (int y = 0; y < _height; y++) {
					m_tiles [x, y] = (TileData)CreateTile (x, y);
				}
			}

			foreach (Room r in Map.RoomManager.Rooms) {
				for (int x = r.MinX; x < r.MaxX; x++) {
					for (int y = r.MinY; y < r.MaxY; y++) {
						m_tiles [x, y].Type = r.Tiles[x - r.MinX, y - r.MinY];
					}
				}
			}

			// this is really poorly designed, but it works
			// it's an unfortunate necessity due to the fact that walls
			// must be 2 tiles thick in the y direction
			foreach (Edge<Vertex<Room>> e in Map.RoomManager.RoomGraphEdgeList) {
				// vertical hallways
				int x = Mathf.RoundToInt(e.first.data.Center.x);
				int yStart = Mathf.RoundToInt(e.first.data.Center.y);
				int yEnd = Mathf.RoundToInt(e.second.data.Center.y);

				if (yStart > yEnd) {
					int temp = yStart;
					yStart = yEnd;
					yEnd = temp;
				}

				for (int i = yStart; i <= yEnd; i++) {
					if (GetTileAt (x - 2, i).Type == TileType.Empty) {
						m_tiles [x - 2, i].Type = TileType.Wall;
					}
					if (GetTileAt (x + 2, i).Type == TileType.Empty) {
						m_tiles [x + 2, i].Type = TileType.Wall;
					}
					m_tiles [x - 1, i].Type = TileType.Floor;
					m_tiles [x + 1, i].Type = TileType.Floor;
					m_tiles [x, i].Type = TileType.Floor;
				}

				// horizontal hallways
				int y = Mathf.RoundToInt(e.second.data.Center.y);
				int xStart = Mathf.RoundToInt(e.first.data.Center.x);
				int xEnd = Mathf.RoundToInt(e.second.data.Center.x);

				if (xStart > xEnd) {
					int temp = xStart;
					xStart = xEnd;
					xEnd = temp;
				}

				for (int i = xStart - 1; i <= xEnd + 1; i++) {
					if (GetTileAt (i, y + 2).Type == TileType.Empty) {
						m_tiles [i, y + 2].Type = TileType.Wall;
					}
					if (GetTileAt (i, y + 3).Type == TileType.Empty) {
						m_tiles [i, y + 3].Type = TileType.Wall;
					}
					if (GetTileAt (i, y - 2).Type == TileType.Empty) {
						m_tiles [i, y - 2].Type = TileType.Wall;
					}
					if (GetTileAt (i, y - 3).Type == TileType.Empty) {
						m_tiles [i, y - 3].Type = TileType.Wall;
					}
					m_tiles [i, y].Type = TileType.Floor;
					m_tiles [i, y - 1].Type = TileType.Floor;
					m_tiles [i, y + 1].Type = TileType.Floor;
				}

				// corners of hallways
				for (int row = y - 2; row <= y + 2; row++) {
					for (int col = x - 1; col <= x + 1; col++) {
						SetEmptyNeighborsToType (col, row, TileType.Wall);
					}
				}
			}
		}

		private void SetEmptyNeighborsToType(int x, int y, TileType type) {
			for (int row = y - 2; row <= y + 2; row++) {
				for (int col = x - 1; col <= x + 1; col++) {
					if (row == y && col == x) {
						continue;
					}
					Tile t = GetTileAt (col, row);
					if (GetTileAt (col, row).Type == TileType.Empty) {
						SetTileType (t, type);
					}
				}
			}
		}

		private void InitializeTestTiles (int width, int height)
		{
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					m_tiles [x, y].Type = TileType.Floor;
				}
			}
		}

		public Tile GetTileById (uint id)
		{
			return m_tiles [id & 0xFFFF, (id >> 16) & 0xFFFF];
		}

		public Tile GetTileAt (int x, int y)
		{
			if (x >= m_tiles.GetLength (0)) {
				x = m_tiles.GetLength (0) - 1;
			}
			if (y >= m_tiles.GetLength (1)) {
				y = m_tiles.GetLength (1) - 1;
			}
			if (x < 0) {
				x = 0;
			}
			if (y < 0) {
				y = 0;
			}

			return m_tiles [x, y];
		}

		public void SetTileType (Tile tt, TileType value)
		{
			var t = (TileData)tt;
			var old = t.Type;
			t.Type = value;

			// Inform others of change
			if (old != t.Type) {
				EmitEvent (GenericEventType.CHANGED, t);
			}
		}

		public Tile CreateTile (int x, int y)
		{
			return new TileData (x, y);
		}

		public class TileData : Tile
		{
			public uint Id { get; set; }

			public TileType Type { get; set; }

			public int X { get { return (int)(Id & 0xFFFF); } }

			public int Y { get { return (int)((Id >> 16) & 0xFFFF); } }

			public bool IsWalkable { get { return MovementCost > 0; } }

			public float MovementCost {
				get {
					float tileCost = 0;
					switch (Type) {
					case TileType.Wall:
						tileCost += 0;
						break;
					default:
						tileCost += 1;
						break;
					}
					return tileCost;
				}
			}

			public override string ToString ()
			{
				return string.Format ("[TileData: Type={0}, X={1}, Y={2}, IsWalkable={3}, MovementCost={4}]", Type, X, Y, IsWalkable, MovementCost);
			}

			public TileData (int x, int y)
			{
				if (x < 0 || x >= 65535 || y < 0 || y >= 65535) {
					throw new ArgumentOutOfRangeException ();
				}
				this.Id = ((uint)x | ((uint)y << 16));
			}
		}
	}
}
