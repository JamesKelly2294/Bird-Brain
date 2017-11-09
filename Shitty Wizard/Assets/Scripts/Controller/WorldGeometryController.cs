using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShittyWizard.Controller.Game;
using ShittyWizard.Model.World;
using ShittyWizzard.Utilities;

// Responsible for generating the map geometry from a given tile map
// w.r.t colliders:
// 	there are many methods of optimizing how we decide to create our box colliders
// 	the following link: https://love2d.org/wiki/TileMerging suggests a method of
//	merging our wall tiles together to come up with a minimal set of colliders
//
//	for simplicity's sake, I have decided to simply create 1 box collider per wall
//	that sits on a border
namespace ShittyWizard.Controller.Game
{
	public class WorldGeometryController : MonoBehaviour
	{
		[SerializeField]
		private Texture2D tileMap = null;
		//[SerializeField]
		//private Texture2D tileMapNormals = null;

		private List<GameObject> m_geometry;

		private Map ActiveMap {
			get {
				return WorldController.Instance.ActiveLevel;
			}
		}

		// Use this for initialization
		void Start ()
		{
		}

		bool NeighborsAreOfType (int x, int y, TileType type, bool includeDiagonal = false)
		{
			TileManager tm = ActiveMap.TileManager;
			bool lrud = tm.GetTileAt (x, y + 1).Type == type &&
			           tm.GetTileAt (x, y - 1).Type == type &&
			           tm.GetTileAt (x + 1, y).Type == type &&
			           tm.GetTileAt (x - 1, y).Type == type;

			if (includeDiagonal) {
				return lrud &&
				tm.GetTileAt (x + 1, y + 1).Type == type &&
				tm.GetTileAt (x + 1, y - 1).Type == type &&
				tm.GetTileAt (x - 1, y + 1).Type == type &&
				tm.GetTileAt (x - 1, y - 1).Type == type;
			} else {
				return lrud;
			}
		}

		bool NeighborIsOfType (int x, int y, TileType type, bool includeDiagonal = false)
		{
			TileManager tm = ActiveMap.TileManager;
			bool lrud = tm.GetTileAt (x, y + 1).Type == type ||
			           tm.GetTileAt (x, y - 1).Type == type ||
			           tm.GetTileAt (x + 1, y).Type == type ||
			           tm.GetTileAt (x - 1, y).Type == type;

			if (includeDiagonal) {
				return lrud ||
				tm.GetTileAt (x + 1, y + 1).Type == type ||
				tm.GetTileAt (x + 1, y - 1).Type == type ||
				tm.GetTileAt (x - 1, y + 1).Type == type ||
				tm.GetTileAt (x - 1, y - 1).Type == type;
			} else {
				return lrud;
			}
		}

		GameObject CreateMeshGameObject (string name, Transform parent)
		{
			GameObject go = new GameObject ();
			go.name = name;
			go.transform.parent = parent;
			go.AddComponent<MeshRenderer> ();
			go.AddComponent<MeshFilter> ();
			go.isStatic = true;
			go.layer = LayerMask.NameToLayer ("Geometry");

			return go;
		}

		public void BuildInitialGeometry (float textureOffsetX, float textureOffsetY) {
			if (m_geometry != null) {
				foreach (GameObject go in m_geometry) {
					Destroy (go);
				}
				m_geometry = null;
			}
			m_geometry = new List<GameObject> ();

			int chunkWidth = 15;
			int chunkHeight = 15;

			TileManager tm = ActiveMap.TileManager;

			int chunkNumber = 0;
			for (int x = 0; x < tm.Width; x += chunkWidth) {
				for (int y = 0; y < tm.Height; y += chunkHeight) {
					int width = chunkWidth;
					int height = chunkHeight;
					if (tm.Width - x < width) {
						width = tm.Width - x;
					}
					if (tm.Height - y < height) {
						height = tm.Height - y;
					}

					BuildChunk (x, y, width, height, chunkNumber, textureOffsetX, textureOffsetY);
					chunkNumber++;
				}
			}
		}

		public void BuildChunk (int startX, int startY, int width, int height, int chunkNumber, float textureOffsetX, float textureOffsetY)
		{
			TileManager tm = ActiveMap.TileManager;

			GameObject chunkGO = new GameObject ();
			chunkGO.name = "Chunk " + chunkNumber;
			chunkGO.transform.parent = transform;
			chunkGO.layer = LayerMask.NameToLayer ("Geometry");
			m_geometry.Add (chunkGO);

			GameObject wallsGO = CreateMeshGameObject ("Walls " + chunkNumber, chunkGO.transform);
			MeshRenderer walls_mr = wallsGO.GetComponent<MeshRenderer> ();
			MeshFilter walls_mf = wallsGO.GetComponent<MeshFilter> ();

			GameObject ceilingsGO = CreateMeshGameObject ("Ceilings " + chunkNumber, chunkGO.transform);
			MeshRenderer ceilings_mr = ceilingsGO.GetComponent<MeshRenderer> ();
			MeshFilter ceilings_mf = ceilingsGO.GetComponent<MeshFilter> ();

			GameObject floorsGO = CreateMeshGameObject ("Floors " + chunkNumber, chunkGO.transform);
			MeshRenderer floors_mr = floorsGO.GetComponent<MeshRenderer> ();
			MeshFilter floors_mf = floorsGO.GetComponent<MeshFilter> ();

			List<Vector3> walls_vertices = new List<Vector3> ();
			List<Vector2> walls_uvs = new List<Vector2> ();
			List<int> walls_indices = new List<int> ();
			int walls_currentIndex = 0;

			List<Vector3> ceilings_vertices = new List<Vector3> ();
			List<Vector2> ceilings_uvs = new List<Vector2> ();
			List<int> ceilings_indices = new List<int> ();
			int ceilings_currentIndex = 0;

			List<Vector3> floors_vertices = new List<Vector3> ();
			List<Vector2> floors_uvs = new List<Vector2> ();
			List<int> floors_indices = new List<int> ();
			int floors_currentIndex = 0;

			float tileTexWidth = 16.0f / tileMap.width;
			float tileTexHeight = 16.0f / tileMap.height;

			Vector2 tilesetOffset = new Vector2 (textureOffsetX, textureOffsetY);
			Vector2 ceilingLoc = new Vector2 (0.0f, 0.0f) + tilesetOffset;
			Vector2 floorLoc = new Vector2 (1.0f, 0.0f) + tilesetOffset;
			Vector2 wallLoc = new Vector2 (2.0f, 0.0f) + tilesetOffset;

			Vector2 offset = new Vector2 (1.0f / tileMap.width, 1.0f / tileMap.height);

			GameObject colliderParent = new GameObject ();
			m_geometry.Add (colliderParent);
			colliderParent.name = "Colliders " + chunkNumber;
			colliderParent.layer = LayerMask.NameToLayer ("Wall");
			colliderParent.transform.parent = chunkGO.transform;

			Vector2 tileLoc;
			Vector2 tileOffset;
			for (int x = startX; x < startX + width; x++) {
				for (int y = startY; y < startY + height; y++) {

					Tile t = tm.GetTileAt (x, y);

					switch (t.Type) {
					case TileType.Wall:
						if (!NeighborsAreOfType (x, y, TileType.Wall)) {
							GameObject collider = new GameObject ();
							collider.AddComponent<BoxCollider> ();
							BoxCollider bc = collider.GetComponent<BoxCollider> ();
							collider.transform.position = new Vector3 (x, 0.0f, y);
							collider.transform.parent = colliderParent.transform;
							collider.transform.name = string.Format ("Collider ({0}, {1})", x, y);
							bc.center = new Vector3 (0.5f, 4.0f, 0.5f);
							bc.size = new Vector3 (1.0f, 8.0f, 1.0f);
						}

						if (NeighborIsOfType (x, y, TileType.Floor, true) || NeighborIsOfType (x, y + 1, TileType.Floor, true)) {
							if (tm.GetTileAt (x, y + 1).Type == TileType.Wall) {
								tileLoc = ceilingLoc;
								tileOffset = Vector2.Scale (ceilingLoc, offset);

								ceilings_vertices.Add (new Vector3 (x, 2.0f, y));
								ceilings_vertices.Add (new Vector3 (x + 1.0f, 2.0f, y));
								ceilings_vertices.Add (new Vector3 (x + 1.0f, 2.0f, y + 1.0f));
								ceilings_vertices.Add (new Vector3 (x, 2.0f, y + 1.0f));

								ceilings_uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
								ceilings_uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
								ceilings_uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));
								ceilings_uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));

								ceilings_indices.Add (ceilings_currentIndex);
								ceilings_indices.Add (ceilings_currentIndex + 3);
								ceilings_indices.Add (ceilings_currentIndex + 2);
								ceilings_indices.Add (ceilings_currentIndex);
								ceilings_indices.Add (ceilings_currentIndex + 2);
								ceilings_indices.Add (ceilings_currentIndex + 1);

								ceilings_currentIndex += 4;
							}
						}

						if (tm.GetTileAt (x, y - 1).Type == TileType.Wall || tm.GetTileAt (x, y - 1).Type == TileType.Empty) {



							continue;
						}
						tileLoc = wallLoc;
						tileOffset = Vector2.Scale (wallLoc, offset);

						walls_vertices.Add (new Vector3 (x, 0.0f, y));
						walls_vertices.Add (new Vector3 (x + 1.0f, 0.0f, y));
						walls_vertices.Add (new Vector3 (x + 1.0f, 1.0f, y));
						walls_vertices.Add (new Vector3 (x, 1.0f, y));

						walls_uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
						walls_uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
						walls_uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));
						walls_uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));


						walls_indices.Add (walls_currentIndex);
						walls_indices.Add (walls_currentIndex + 3);
						walls_indices.Add (walls_currentIndex + 2);
						walls_indices.Add (walls_currentIndex);
						walls_indices.Add (walls_currentIndex + 2);
						walls_indices.Add (walls_currentIndex + 1);

						walls_currentIndex += 4;

						walls_vertices.Add (new Vector3 (x, 1.0f, y));
						walls_vertices.Add (new Vector3 (x + 1.0f, 1.0f, y));
						walls_vertices.Add (new Vector3 (x + 1.0f, 2.0f, y));
						walls_vertices.Add (new Vector3 (x, 2.0f, y));

						walls_uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
						walls_uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
						walls_uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));
						walls_uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));

						walls_indices.Add (walls_currentIndex);
						walls_indices.Add (walls_currentIndex + 3);
						walls_indices.Add (walls_currentIndex + 2);
						walls_indices.Add (walls_currentIndex);
						walls_indices.Add (walls_currentIndex + 2);
						walls_indices.Add (walls_currentIndex + 1);

						walls_currentIndex += 4;
						break;
					case TileType.Floor:
						tileLoc = floorLoc + Vector2.Scale (floorLoc, offset);
						tileOffset = Vector2.Scale (floorLoc, offset);

						floors_vertices.Add (new Vector3 (x, 0.0f, y));
						floors_vertices.Add (new Vector3 (x + 1.0f, 0.0f, y));
						floors_vertices.Add (new Vector3 (x + 1.0f, 0.0f, y + 1.0f));
						floors_vertices.Add (new Vector3 (x, 0.0f, y + 1.0f));

						floors_uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
						floors_uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
						floors_uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));
						floors_uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));

						floors_indices.Add (floors_currentIndex);
						floors_indices.Add (floors_currentIndex + 3);
						floors_indices.Add (floors_currentIndex + 2);
						floors_indices.Add (floors_currentIndex);
						floors_indices.Add (floors_currentIndex + 2);
						floors_indices.Add (floors_currentIndex + 1);

						floors_currentIndex += 4;
						break;
					default:
						break;
					}

				}
			}

			if (floors_vertices.Count == 0 && ceilings_vertices.Count == 0 && walls_vertices.Count == 0) {
				Destroy (chunkGO);
				return;
			}

			for (int i = 0; i < colliderParent.transform.childCount; i++) {
				colliderParent.transform.GetChild (i).gameObject.layer = LayerMask.NameToLayer ("Wall");
			}

			SetupMesh (walls_vertices, walls_uvs, walls_indices, walls_mf, walls_mr);
			SetupMesh (ceilings_vertices, ceilings_uvs, ceilings_indices, ceilings_mf, ceilings_mr);
			SetupMesh (floors_vertices, floors_uvs, floors_indices, floors_mf, floors_mr);
		}

		void SetupMesh (List<Vector3> vertices, List<Vector2> uvs, List<int> indices, MeshFilter mf, MeshRenderer mr)
		{
			Mesh mesh = new Mesh ();
			mesh.SetVertices (vertices);
			mesh.SetUVs (0, uvs);
			mesh.SetIndices (indices.ToArray (), MeshTopology.Triangles, 0);
			mesh.RecalculateNormals ();

			mf.sharedMesh = mesh;

			mr.material = new Material (Shader.Find ("Standard"));
			mr.material.mainTexture = tileMap;
			mr.material.EnableKeyword ("_NORMALMAP");
			mr.material.SetFloat ("_Glossiness", 0.0f);
			mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
		}
	}

}
