using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShittyWizard.Model.World;
using ShittyWizzard.Utilities;
using System.Text.RegularExpressions;

namespace ShittyWizard.Model.World
{
	public class Map
	{

		public RoomManager RoomManager { get; protected set; }
		public TileManager TileManager { get; protected set; }

		public int Width { get { return TileManager.Width; } }
		public int Height { get { return TileManager.Height; } }

		public static Map NewBossMap() {
			Map m = new Map ();
			m.RoomManager = new RoomManager (m, 0);
			m.TileManager = TileManager.TileManagerForBossMap (m);

			return m;
		}

		private Map() {
		}

		public Map (int numberOfRooms)
		{
			SetupManagers (numberOfRooms);
		}

		public void Update (float delta)
		{

		}

		private void SetupManagers (int numberOfRooms)
		{
			RoomManager = new RoomManager (this, numberOfRooms);
			TileManager = new TileManager (this);
		}

		public int FindClosestSquareOfTwo(int input) {
			int power = 0;
			int result = (int)Mathf.Pow (2, power);
			while (result < input) {
				power++;
				result = (int)Mathf.Pow (2, power);
			}
			return result;
		}

		public Texture2D GenerateMinimap() {
//			int closestWidth = FindClosestSquareOfTwo(TileManager.Width);
//			int closestHeight = FindClosestSquareOfTwo(TileManager.Height);
//			int max = closestWidth;
//			if (closestWidth < closestHeight) {
//				max = closestHeight;
//			}
//			Texture2D tex = new Texture2D (max, max);
			Texture2D tex = new Texture2D (TileManager.Width, TileManager.Height);

			var pixels = tex.GetPixels ();

			tex.filterMode = FilterMode.Point;

			for (int x = 0; x < tex.width; x++) {
				for (int y = 0; y < tex.height; y++) {
					pixels [y * tex.width + x] = new Color (0.0f, 0.0f, 0.0f, 0.0f);
				}
			}

			int yOffset = 0;
			int xOffset = 0;
//			int yOffset = Mathf.FloorToInt((max - TileManager.Height) / 2.0f);
//			int xOffset = Mathf.FloorToInt((max - TileManager.Width) / 2.0f);

			for (int x = 0; x < TileManager.Width; x++) {
				for (int y = 0; y < TileManager.Height; y++) {
					Tile t = TileManager.GetTileAt (x, y);
					switch (t.Type) {
					case TileType.Wall:
						if (TileManager.HasNeighborOfType (t, TileType.Floor)) {
							pixels [(y+yOffset) * tex.width + (x+xOffset)] = new Color (0.5f, 0.5f, 0.7f, 1.0f);
						}
						break;
					case TileType.Floor:
						pixels [(y+yOffset) * tex.width + (x+xOffset)] = new Color (0.1f, 0.1f, 0.55f, 1.0f);
						break;
					case TileType.Empty:
					default:
						pixels [(y+yOffset) * tex.width + (x+xOffset)] = new Color (0.0f, 0.0f, 0.0f, 0.0f);
						break;
					}
				}
			}

			tex.filterMode = FilterMode.Point;
			tex.wrapMode = TextureWrapMode.Clamp;
			tex.SetPixels (pixels);
			tex.Apply ();

			return tex;
		}
	}
}