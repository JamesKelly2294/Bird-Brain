using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using ShittyWizard.Model.World;
using System.Diagnostics;

namespace ShittyWizard.Controller.Game
{
	public class MapController : MonoBehaviour
	{
		public static MapController Instance { get; protected set; }

		// World and tile data
		public Map ActiveMap { get; protected set; }

		void Awake ()
		{
			if (Instance != null) {
				UnityEngine.Debug.LogError ("Trying to instantiate already existing singleton:" +
					" MapController!");
			}        
			Instance = this;

			CreateEmptyWorld ();
		}

		void Update ()
		{
			ActiveMap.Update (Time.deltaTime);
		}
			
		void CreateEmptyWorld ()
		{
			// Create a world with Empty tiles
			ActiveMap = new Map ();
		}

		public Tile GetTileAtWorldCoord (Vector3 coord)
		{
			int x = Mathf.RoundToInt (coord.x);
			int y = Mathf.RoundToInt (coord.y);

			return MapController.Instance.ActiveMap.TileManager.GetTileAt (x, y);
		}
	}
}