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

		public TileManager TileManager { get; protected set; }

		public int Width { get; protected set; }

		public int Height { get; protected set; }

		public Map (int width = 20, int height = 20)
		{
			this.Width = width;
			this.Height = height;

			SetupManagers ();

			//Debug.Log ("World created with " + (width * height) + " tiles.");
		}

		public void ResetWithTextAsset (TextAsset asset)
		{
			Regex regex = new Regex ("\n");
			string[] lines = regex.Split (asset.text);
			Height = lines.Length - 1;

			Width = 0;
			foreach (string line in lines) {
				if (line.Length > Width) {
					Width = line.Length;
				}
			}

			TileManager.ReinitializeFromAsset (asset);
		}

		public void Update (float delta)
		{

		}

		private void SetupManagers ()
		{
			TileManager = new TileManager (this);
		}
	}
}