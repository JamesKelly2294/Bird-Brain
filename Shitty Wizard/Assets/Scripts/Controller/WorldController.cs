using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using ShittyWizard.Model.World;
using ShittyWizzard.Utilities;
using System.Collections;

namespace ShittyWizard.Controller.Game
{
	[System.Serializable]
	public class EnemySpawnRate
	{
		public string Name = "Enemy";
		public GameObject enemyPrefab;
		public float spawnRate = 1.0f;
	}

	public class WorldController : MonoBehaviour
	{
		public static WorldController Instance { get; protected set; }

		public EnemySpawnRate[] enemies;
		public GameObject playerPrefab;
		public GameObject bossPrefab;
		public GameObject staircase;
		public WorldGeometryController WorldGeometryController;
		public GUIController GUIController;

		private GameObject m_player;

		// World and tile data
		public Map ActiveLevel { 
			get {
				return ActiveWorld.ActiveLevel;
			}
		}

		public World ActiveWorld {
			get;
			protected set;
		}

		public Camera camera;

		[Range (0, 100)]
		public int enemiesPerFloor = 30;

		[Range (0.0f, 1.0f)]
		public float enemiesPerFloorSpread = 0.2f;

		[Range (1, 10)]
		public int numberOfFloors = 3;

		[Range (1, 20)]
		public int roomsPerFloor = 10;

		[Range (0.0f, 1.0f)]
		public float roomsPerFloorSpread = 0.2f;

		public bool DebugMode = false;

		private float initialCameraOrthographicSize = 0.0f;

		void Awake ()
		{
			if (Instance != null) {
				UnityEngine.Debug.LogError ("Trying to instantiate already existing singleton:" +
				" MapController!");
			}        
			Instance = this;

			initialCameraOrthographicSize = Camera.main.orthographicSize;

			CreateEmptyWorld ();

			AdvanceLevel ();
		}

		void Update ()
		{
			ActiveWorld.Update (Time.deltaTime);
		}

		void CreateEntitiesForBoss ()
		{
			camera.GetComponent<CameraController>().UpdateOrthographicSize(initialCameraOrthographicSize * 2.0f);

			m_player.transform.position = new Vector3 (
				ActiveWorld.ActiveLevel.TileManager.Width / 2.0f,
				0.0f,
				ActiveWorld.ActiveLevel.TileManager.Height / 3.0f
			);

			GameObject boss = Instantiate (bossPrefab);
			GUIController.SetBoss(boss);
			boss.transform.position = new Vector3 (
				ActiveWorld.ActiveLevel.TileManager.Width / 2.0f,
				0.0f,
				ActiveWorld.ActiveLevel.TileManager.Height / 2.0f
			);
			boss.GetComponent<Owlman> ().offset = boss.transform.position;
			boss.GetComponent<Owlman> ().target = m_player;
			boss.transform.parent = transform;


			GameObject lights = new GameObject ();
			lights.transform.parent = transform;
			lights.transform.name = "Lights";
			for (int i = 0; i < 8; i++) {
				Tile t = ActiveLevel.TileManager.GetRandomTileOfType (TileType.Floor);
				GameObject light = new GameObject ();
				light.transform.name = "Light";
				light.AddComponent<Light> ();
				light.GetComponent<Light> ().color = new Color (
					UnityEngine.Random.Range (0.5f, 0.8f), 
					UnityEngine.Random.Range (0.1f, 0.2f), 
					UnityEngine.Random.Range (0.1f, 0.2f)
				);
				light.GetComponent<Light> ().range = 50.0f;
				light.transform.position = new Vector3 (t.X + 0.5f, 1.5f, t.Y + 0.5f);
				light.transform.parent = lights.transform;
			}
		}

		void CreateEntitiesForLevel ()
		{
			camera.GetComponent<CameraController>().UpdateOrthographicSize(initialCameraOrthographicSize);

			enemiesPerFloor += Mathf.RoundToInt (enemiesPerFloor * UnityEngine.Random.Range (0.0f, enemiesPerFloorSpread));

			Room startRoom = ActiveLevel.RoomManager.PlayerStartRoom;
			m_player.transform.position = new Vector3 (startRoom.CenterX, 0.0f, startRoom.CenterY);
			GUIController.playerGO = m_player;
			GUIController.player = m_player.GetComponent<EntityPlayer> ();

			GameObject entities = new GameObject ();
			entities.transform.parent = transform;
			entities.transform.name = "Entities";
			entities.transform.tag = "Entities";

			Room endRoom = ActiveLevel.RoomManager.StaircaseRoom;
			Tile t;
			if (DebugMode) {
				t = ActiveLevel.TileManager.GetTileAt (Mathf.FloorToInt (ActiveLevel.RoomManager.PlayerStartRoom.CenterX - 2), Mathf.FloorToInt (ActiveLevel.RoomManager.PlayerStartRoom.CenterY) - 1);
			} else {
				t = ActiveLevel.TileManager.GetTileAt (Mathf.FloorToInt (endRoom.CenterX), Mathf.FloorToInt (endRoom.CenterY) - 1);
			}
			GameObject stc = Instantiate (staircase);
			stc.GetComponent<Staircase> ().player = m_player;
			stc.GetComponent<Staircase> ().worldController = this;
			stc.transform.position = new Vector3 (t.X, 0.0f, t.Y+1.0f);
			stc.transform.parent = entities.transform;
			stc.transform.name = "Staircase";
			GUIController.staircaseGO = stc;

			Vector2 playerPos = new Vector2 (m_player.transform.position.x, m_player.transform.position.z);
			float enemyEliminationRadius = 15.0f;
			int maxEnemyTypes = Mathf.RoundToInt (((float)ActiveWorld.CurrentFloorNumber / (float)ActiveWorld.MaximumFloors) * (float)enemies.Length);

			List<Tuple<float, GameObject>> spawnRates = new List<Tuple<float, GameObject>> ();
			float currentSpawnRateMax = 0.0f;
			for (int i = 0; i < maxEnemyTypes; i++) {
				currentSpawnRateMax += enemies [i].spawnRate;
				var tuple = new Tuple<float, GameObject> (currentSpawnRateMax, enemies [i].enemyPrefab);
				spawnRates.Add (tuple);
			}

			int enemiesForThisFloor = (int)(enemiesPerFloor * (1.0f + UnityEngine.Random.Range (-enemiesPerFloorSpread, enemiesPerFloorSpread)));
			for (int i = 0; i < enemiesForThisFloor; i++) {
				t = ActiveLevel.TileManager.GetRandomTileOfType (TileType.Floor);
				if (Vector2.Distance (new Vector2 (t.X, t.Y), playerPos) < enemyEliminationRadius) {
					continue;
				}

				// find enemy to spawn randomly
				float randomSpawnRateVal = UnityEngine.Random.Range (0, currentSpawnRateMax);
				GameObject enemyToSpawn = enemies [0].enemyPrefab;
				for (int j = 0; j < enemies.Length; j++) {
					if (randomSpawnRateVal < spawnRates [j].Item1) {
						enemyToSpawn = spawnRates [j].Item2;
						break;
					}
				}

				GameObject enemyType = enemyToSpawn;
				GameObject enemy = Instantiate (enemyType);
				enemy.transform.position = new Vector3 (t.X + 0.5f, 0.0f, t.Y + 0.5f);
				enemy.GetComponent<EnemyController> ().target = m_player.transform;
				enemy.transform.parent = entities.transform;
			}

			GameObject lights = new GameObject ();
			lights.transform.parent = entities.transform;
			lights.transform.name = "Lights";

			foreach (Room r in ActiveWorld.ActiveLevel.RoomManager.Rooms) {
				for (int i = 0; i < 4; i++) {
					t = ActiveWorld.ActiveLevel.TileManager.GetRandomTileOfTypeInRoom (TileType.Floor, r);
					GameObject light = new GameObject ();
					light.transform.name = "Light";
					light.AddComponent<Light> ();
					light.GetComponent<Light> ().color = new Color (
						UnityEngine.Random.Range (0.5f, 0.7f), 
						UnityEngine.Random.Range (0.2f, 0.5f), 
						UnityEngine.Random.Range (0.2f, 0.5f)
					);
					light.GetComponent<Light> ().range = 30.0f;
					light.transform.position = new Vector3 (t.X + 0.5f, 1.5f, t.Y + 0.5f);
					light.transform.parent = lights.transform;
				}
			}
		}

		public void AdvanceLevel ()
		{
			for (int i = 0; i < transform.childCount; i++) {
				GameObject go = transform.Find ("Entities").gameObject;
				if (go != null) {
					Destroy (go);
				}
			}

			if (m_player == null) {
				m_player = Instantiate (playerPrefab);
				camera.GetComponent<CameraController> ().target = m_player.transform;
				m_player.transform.parent = transform;
				m_player.transform.name = "Player";
			}

			ActiveWorld.AdvanceLevel ();

			if (!ActiveWorld.IsBossLevel) {
				CreateEntitiesForLevel ();
				WorldGeometryController.BuildInitialGeometry (0.0f, 0.0f);
			} else {
				CreateEntitiesForBoss ();
				WorldGeometryController.BuildInitialGeometry (3.0f, 0.0f);
			}


			GUIController.UpdateForNewLevel (ActiveWorld.CurrentFloorNumber.ToString ());


		}

		void CreateEmptyWorld ()
		{
			// Create a world with Empty tiles
			ActiveWorld = new World (numberOfFloors, roomsPerFloor, roomsPerFloorSpread);
		}

		public Tile GetTileAtWorldCoord (Vector3 coord)
		{
			int x = Mathf.RoundToInt (coord.x);
			int y = Mathf.RoundToInt (coord.y);

			return WorldController.Instance.ActiveLevel.TileManager.GetTileAt (x, y);
		}

		public void LoadWinScene(float length) {
			StartCoroutine (LoadWinSceneE (length));
		}

		private IEnumerator LoadWinSceneE(float _length) {
			yield return new WaitForSeconds(2.5f);

			SceneManager.LoadScene ("MenuSceneWin");
		}
	}
}