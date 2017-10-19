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

		public List<GameObject> enemies;
		public GameObject player;

		// World and tile data
		public Map ActiveMap { get; protected set; }

		[Range(1, 20)]
		public int numberOfRooms;

		[Range(0, 100)]
		public int numberOfEnemies;

		void Awake ()
		{
			if (Instance != null) {
				UnityEngine.Debug.LogError ("Trying to instantiate already existing singleton:" +
					" MapController!");
			}        
			Instance = this;

			CreateEmptyWorld ();

			GameObject entities = new GameObject ();
			entities.transform.parent = transform;
			entities.transform.name = "Entities";

			Tile t = ActiveMap.TileManager.GetRandomTileOfType (TileType.Floor);
			GameObject ply = Instantiate (player);
			ply.transform.position = new Vector3 (t.X, 0.0f, t.Y);
			Camera.main.GetComponent<CameraController> ().target = ply.transform;
			ply.transform.parent = entities.transform;
			ply.transform.name = "Player";

			for (int i = 0; i < numberOfEnemies; i++) {
				t = ActiveMap.TileManager.GetRandomTileOfType (TileType.Floor);
				GameObject enemyType = enemies [UnityEngine.Random.Range (0, enemies.Count)];
				GameObject enemy = Instantiate (enemyType);
				enemy.transform.position = new Vector3 (t.X + 0.5f, 0.0f, t.Y + 0.5f);
				enemy.GetComponent<EnemyController> ().target = ply.transform;
				enemy.transform.parent = entities.transform;
			}
		}

		void Update ()
		{
			ActiveMap.Update (Time.deltaTime);
		}
			
		void CreateEmptyWorld ()
		{
			// Create a world with Empty tiles
			ActiveMap = new Map (numberOfRooms);
		}

		public Tile GetTileAtWorldCoord (Vector3 coord)
		{
			int x = Mathf.RoundToInt (coord.x);
			int y = Mathf.RoundToInt (coord.y);

			return MapController.Instance.ActiveMap.TileManager.GetTileAt (x, y);
		}
	}
}