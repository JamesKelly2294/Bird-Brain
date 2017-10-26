using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using ShittyWizard.Model.World;
using System.Diagnostics;

namespace ShittyWizard.Controller.Game
{
	public class WorldController : MonoBehaviour
	{
		public static WorldController Instance { get; protected set; }

		public List<GameObject> enemies;
		public GameObject player;

		// World and tile data
		public Map ActiveLevel { 
			get {
				return ActiveWorld.ActiveLevel;
			}
		}

		public World ActiveWorld {
			get; protected set;
		}

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
			entities.transform.tag = "Entities";

			Tile t = ActiveLevel.TileManager.GetRandomTileOfTypeInRoom(TileType.Floor, ActiveLevel.RoomManager.PlayerStartRoom);
			GameObject ply = Instantiate (player);
			ply.transform.position = new Vector3 (t.X, 0.0f, t.Y);
			Camera.main.GetComponent<CameraController> ().target = ply.transform;
			ply.transform.parent = entities.transform;
			ply.transform.name = "Player";

			for (int i = 0; i < numberOfEnemies; i++) {
				t = ActiveLevel.TileManager.GetRandomTileOfType (TileType.Floor);
				GameObject enemyType = enemies [UnityEngine.Random.Range (0, enemies.Count)];
				GameObject enemy = Instantiate (enemyType);
				enemy.transform.position = new Vector3 (t.X + 0.5f, 0.0f, t.Y + 0.5f);
				enemy.GetComponent<EnemyController> ().target = ply.transform;
				enemy.transform.parent = entities.transform;
			}

			GameObject lights = new GameObject ();
			lights.transform.parent = transform;
			lights.transform.name = "Lights";
			for (int i = 0; i < 30; i++) {
				t = ActiveLevel.TileManager.GetRandomTileOfType (TileType.Floor);
				GameObject light = new GameObject();
				light.transform.name = "Light";
				light.AddComponent<Light> ();
				light.GetComponent<Light> ().color = new Color(
					UnityEngine.Random.Range(0.5f, 0.7f), 
					UnityEngine.Random.Range(0.2f, 0.5f), 
					UnityEngine.Random.Range(0.2f, 0.5f)
				);
				light.GetComponent<Light> ().range = 30.0f;
				light.transform.position = new Vector3 (t.X + 0.5f, 1.5f, t.Y + 0.5f);
				light.transform.parent = lights.transform;
			}
		}

		void Update ()
		{
			ActiveWorld.Update (Time.deltaTime);
		}
			
		void CreateEmptyWorld ()
		{
			// Create a world with Empty tiles
			ActiveWorld = new World ();
		}

		public Tile GetTileAtWorldCoord (Vector3 coord)
		{
			int x = Mathf.RoundToInt (coord.x);
			int y = Mathf.RoundToInt (coord.y);

			return WorldController.Instance.ActiveLevel.TileManager.GetTileAt (x, y);
		}
	}
}