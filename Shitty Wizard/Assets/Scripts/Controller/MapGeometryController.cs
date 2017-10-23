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
public class MapGeometryController : MonoBehaviour {
	[SerializeField]
	private Texture2D tileMap = null;
	//[SerializeField]
	//private Texture2D tileMapNormals = null;

	private List<GameObject> m_geometry;

	private Map ActiveMap { 
		get {
			return MapController.Instance.ActiveMap;
		}
	}

	// Use this for initialization
	void Start ()
	{
		BuildInitialGeometry ();
	}

	bool NeighborsAreOfType(int x, int y, TileType type, bool includeDiagonal = false) {
		TileManager tm = ActiveMap.TileManager;
		bool lrud = tm.GetTileAt(x, y + 1).Type == type &&
			tm.GetTileAt(x, y - 1).Type == type &&
			tm.GetTileAt(x + 1, y).Type == type &&
			tm.GetTileAt(x - 1, y).Type == type;

		if (includeDiagonal) {
			return lrud &&
				tm.GetTileAt(x + 1, y + 1).Type == type &&
				tm.GetTileAt(x + 1, y - 1).Type == type &&
				tm.GetTileAt(x - 1, y + 1).Type == type &&
				tm.GetTileAt(x - 1, y - 1).Type == type;
		} else {
			return lrud;
		}
	}

	bool NeighborIsOfType(int x, int y, TileType type, bool includeDiagonal = false) {
		TileManager tm = ActiveMap.TileManager;
		bool lrud = tm.GetTileAt(x, y + 1).Type == type ||
			tm.GetTileAt(x, y - 1).Type == type ||
			tm.GetTileAt(x + 1, y).Type == type ||
			tm.GetTileAt(x - 1, y).Type == type;

		if (includeDiagonal) {
			return lrud ||
				tm.GetTileAt(x + 1, y + 1).Type == type ||
				tm.GetTileAt(x + 1, y - 1).Type == type ||
				tm.GetTileAt(x - 1, y + 1).Type == type ||
				tm.GetTileAt(x - 1, y - 1).Type == type;
		} else {
			return lrud;
		}
	}

	void BuildInitialGeometry ()
	{
		m_geometry = new List<GameObject> ();
		m_geometry.Add (new GameObject ());
		m_geometry [0].name = "Geometry";
		m_geometry [0].transform.parent = transform;

		TileManager tm = ActiveMap.TileManager;

		m_geometry [0].AddComponent<MeshRenderer> ();
		m_geometry [0].AddComponent<MeshFilter> ();

		MeshRenderer mr = m_geometry [0].GetComponent<MeshRenderer> ();
		MeshFilter mf = m_geometry [0].GetComponent<MeshFilter> ();

		Mesh m = new Mesh ();

		List<Vector3> vertices = new List<Vector3> ();
		List<Vector2> uvs = new List<Vector2> ();
		List<int> indices = new List<int> ();

		int currentIndex = 0;

		float tileTexWidth = 16.0f / tileMap.width;
		float tileTexHeight = 16.0f / tileMap.height;

		Vector2 ceilingLoc = new Vector2 (0.0f, 0.0f);
		Vector2 floorLoc = new Vector2 (1.0f, 0.0f);
		Vector2 wallLoc = new Vector2 (2.0f, 0.0f);

		Vector2 offset = new Vector2 (1.0f / tileMap.width, 1.0f / tileMap.height);

		m_geometry = new List<GameObject> ();
		GameObject colliderParent = new GameObject ();
		m_geometry.Add (colliderParent);
		colliderParent.name = "Colliders";
		colliderParent.layer = LayerMask.NameToLayer ("Wall");
		colliderParent.transform.parent = transform;


		Vector2 tileLoc;
		Vector2 tileOffset;
		for (int x = 0; x < ActiveMap.Width; x++) {
			for (int y = 0; y < ActiveMap.Height; y++) {

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
						bc.center = new Vector3 (0.5f, 1.0f, 0.5f);
						bc.size = new Vector3 (1.0f, 4.0f, 1.0f);
					}

					if(NeighborIsOfType(x, y, TileType.Floor, true) || NeighborIsOfType(x, y + 1, TileType.Floor, true)) {
						if (tm.GetTileAt (x, y + 1).Type == TileType.Wall) {
							tileLoc = ceilingLoc;
							tileOffset = Vector2.Scale (ceilingLoc, offset);

							vertices.Add (new Vector3 (x, 2.0f, y));
							vertices.Add (new Vector3 (x + 1.0f, 2.0f, y));
							vertices.Add (new Vector3 (x + 1.0f, 2.0f, y + 1.0f));
							vertices.Add (new Vector3 (x, 2.0f, y + 1.0f));

							uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
							uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
							uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));
							uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));

							indices.Add (currentIndex);
							indices.Add (currentIndex + 3);
							indices.Add (currentIndex + 2);
							indices.Add (currentIndex);
							indices.Add (currentIndex + 2);
							indices.Add (currentIndex + 1);

							currentIndex += 4;
						}
					}

					if (tm.GetTileAt (x, y - 1).Type == TileType.Wall || tm.GetTileAt (x, y - 1).Type == TileType.Empty) {



						continue;
					}
					tileLoc = wallLoc;
					tileOffset = Vector2.Scale (wallLoc, offset);

					vertices.Add (new Vector3 (x, 0.0f, y));
					vertices.Add (new Vector3 (x + 1.0f, 0.0f, y));
					vertices.Add (new Vector3 (x + 1.0f, 1.0f, y));
					vertices.Add (new Vector3 (x, 1.0f, y));

					uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
					uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
					uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));
					uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));


					indices.Add (currentIndex);
					indices.Add (currentIndex + 3);
					indices.Add (currentIndex + 2);
					indices.Add (currentIndex);
					indices.Add (currentIndex + 2);
					indices.Add (currentIndex + 1);

					currentIndex += 4;

					vertices.Add (new Vector3 (x, 1.0f, y));
					vertices.Add (new Vector3 (x + 1.0f, 1.0f, y));
					vertices.Add (new Vector3 (x + 1.0f, 2.0f, y));
					vertices.Add (new Vector3 (x, 2.0f, y));

					uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
					uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
					uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));
					uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));

					indices.Add (currentIndex);
					indices.Add (currentIndex + 3);
					indices.Add (currentIndex + 2);
					indices.Add (currentIndex);
					indices.Add (currentIndex + 2);
					indices.Add (currentIndex + 1);

					currentIndex += 4;
					break;
				case TileType.Floor:
					tileLoc = floorLoc + Vector2.Scale(floorLoc, offset);
					tileOffset = Vector2.Scale (floorLoc, offset);

					vertices.Add (new Vector3 (x, 0.0f, y));
					vertices.Add (new Vector3 (x + 1.0f, 0.0f, y));
					vertices.Add (new Vector3 (x + 1.0f, 0.0f, y + 1.0f));
					vertices.Add (new Vector3 (x, 0.0f, y + 1.0f));

					uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
					uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 1) * tileTexHeight + tileOffset.y));
					uvs.Add (new Vector2 ((tileLoc.x + 1) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));
					uvs.Add (new Vector2 ((tileLoc.x + 0) * tileTexWidth + tileOffset.x, 1.0f - (tileLoc.y + 0) * tileTexHeight + tileOffset.y));

					indices.Add (currentIndex);
					indices.Add (currentIndex + 3);
					indices.Add (currentIndex + 2);
					indices.Add (currentIndex);
					indices.Add (currentIndex + 2);
					indices.Add (currentIndex + 1);

					currentIndex += 4;
					break;
				default:
					break;
				}

			}
		}

		for (int i = 0; i < colliderParent.transform.childCount; i++) {
			colliderParent.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer ("Wall");
		}

		m.SetVertices (vertices);
		m.SetUVs (0, uvs);
		m.SetIndices (indices.ToArray(), MeshTopology.Triangles, 0);
		m.RecalculateNormals ();

		mf.sharedMesh = m;

		mr.material = new Material (Shader.Find ("Standard"));
		mr.material.mainTexture = tileMap;
		mr.material.EnableKeyword ("_NORMALMAP");
		mr.material.SetFloat("_Glossiness", 0.0f);
		mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
		//mr.material.SetTexture ("_BumpMap", tileMapNormals);
	}
}
