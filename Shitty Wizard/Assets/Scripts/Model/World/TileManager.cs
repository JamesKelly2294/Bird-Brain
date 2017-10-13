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

		public TileManager (Map map)
		{
			this.Map = map;
			_width = Map.RoomManager.Width;
			_height = Map.RoomManager.Height;

			SetupTiles ();
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
