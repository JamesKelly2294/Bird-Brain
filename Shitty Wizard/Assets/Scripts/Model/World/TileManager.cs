using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ShittyWizzard.Model;
using ShittyWizzard.Utilities;

namespace ShittyWizard.Model.World
{
	public class TileManager : Manager<Tile> {

		public Map Map { get; protected set; }

		private TileData[,] m_tiles;

		public TileManager(Map map) {
			this.Map = map;

			SetupTiles (map.Width, map.Height);
			InitializeTestTiles (map.Width, map.Height);
		}

		private void SetupTiles(int width, int height) {
			m_tiles = new TileData[width, height];

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					m_tiles [x, y] = (TileData)CreateTile(x, y);
				}
			}
		}

		private void InitializeTestTiles(int width, int height) {
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (y <= height / 3 + 2 && y >= height / 3 - 2) {
						if (x < width / 2 - width / 5 || x > width / 2 + width / 5) {
							m_tiles [x, y].Type = TileType.Wall;
						}
					}

					if (y == 0 || y == 1 || y == height - 1 || y == height - 2 || x == 0 || x == width - 1) {
						m_tiles [x, y].Type = TileType.Wall;
					}
				}
			}
		}

		public Tile GetTileById( uint id ) {
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

			public int X { get { return (int)(Id & 0xFFFF);   } }

			public int Y { get { return (int)((Id >> 16) & 0xFFFF);   } }

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
				if ( x < 0 || x >= 65535 || y < 0 || y >= 65535 ) {
					throw new ArgumentOutOfRangeException();
				}
				this.Id = ( (uint)x | ( (uint)y << 16 ) );
			}
		}

		void GenerateWalls() {
	//		GameObject walls = new GameObject ("walls");
	//		walls.AddComponent<MeshRenderer> ();
	//		walls.AddComponent<MeshFilter> ();
	//
	//		Mesh m = new Mesh ();
	//
	//		Vector3[] verts =  {
	//			new Vector3(0.0f, 0.0f, 0.0f),
	//			new Vector3(0.0f, 1.0f, 0.0f),
	//			new Vector3(1.0f, 1.0f, 0.0f),
	//			new Vector3(1.0f, 0.0f, 0.0f)
	//		};
	//
	//		Vector2[] uvs =  {
	//			new Vector2(0.0f, 0.0f),
	//			new Vector2(0.0f, 2.0f),
	//			new Vector2(1.0f, 2.0f),
	//			new Vector2(1.0f, 0.0f)
	//		};
	//
	//		int[] indices = {
	//			0, 1, 2,
	//			2, 3, 0
	//		};
	//
	//		m.vertices = verts;
	//		m.uv = uvs;
	//		m.SetIndices (indices, MeshTopology.Triangles, 0);
	//
	//		m.RecalculateNormals();
	//
	//		MeshFilter mf = walls.GetComponent<MeshFilter> ();
	//		mf.mesh = m;
	//
	//		MeshRenderer mr = walls.GetComponent<MeshRenderer> ();
	//		mr.material = new Material (Shader.Find ("Standard"));
	//		mr.material.mainTexture = textureMap;
	//		mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
	//
	//		walls.transform.position = new Vector3 (12.0f, 0.0f, 7.0f);
	//		walls.transform.localScale = new Vector3 (1.0f, 2.0f, 1.0f);
	//		walls.transform.SetParent (transform);
	//
	//		////
	//
	//		GameObject ceilings = new GameObject ("ceilings");
	//		ceilings.AddComponent<MeshRenderer> ();
	//		ceilings.AddComponent<MeshFilter> ();
	//
	//		Mesh ceiling_m = new Mesh ();
	//
	//		Vector3[] ceilingVerts =  {
	//			new Vector3(0.0f, 0.0f, 0.0f),
	//			new Vector3(0.0f, 0.0f, 1.0f),
	//			new Vector3(1.0f, 0.0f, 1.0f),
	//			new Vector3(1.0f, 0.0f, 0.0f)
	//		};
	//
	//		Vector2[] ceilingUVs =  {
	//			new Vector2(0.0f, 0.0f),
	//			new Vector2(0.0f, 1.0f),
	//			new Vector2(1.0f, 1.0f),
	//			new Vector2(1.0f, 0.0f)
	//		};
	//
	//		ceiling_m.vertices = ceilingVerts;
	//		ceiling_m.SetIndices (m.GetIndices (0), MeshTopology.Triangles, 0);
	//		ceiling_m.RecalculateNormals ();
	//		ceiling_m.uv = ceilingUVs;
	//
	//		MeshFilter ceiling_mf = ceilings.GetComponent<MeshFilter> ();
	//		ceiling_mf.mesh = ceiling_m;
	//
	//		MeshRenderer ceiling_mr = ceilings.GetComponent<MeshRenderer> ();
	//		ceiling_mr.material = new Material (Shader.Find ("Standard"));
	//		ceiling_mr.material.mainTexture = TEST_CEILING_TILE;
	//
	//		ceilings.transform.position = new Vector3 (12.0f, 2.0f, 7.0f);
	//		ceilings.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
	//		ceilings.transform.SetParent (transform);
	//
	//		walls.AddComponent<BoxCollider> ();
	//		walls.GetComponent<BoxCollider> ().size = new Vector3 (1.0f, 1.0f, 2.0f);
	//		walls.GetComponent<BoxCollider> ().center = new Vector3 (0.5f, 0.5f, 1.0f);
		}
	}
}
