using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShittyWizard.Controller.Game;
using ShittyWizard.Model.World;
using ShittyWizzard.Utilities;

public class MapGeometryController : MonoBehaviour {
	[SerializeField]
	private Texture2D tileMap = null;

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


		Vector2 tileLoc;
		Vector2 tileOffset;
		for (int x = 1; x < ActiveMap.Width - 1; x++) {
			for (int y = 1; y < ActiveMap.Height - 1; y++) {

				Tile t = tm.GetTileAt (x, y);

				switch (t.Type) {
				case TileType.Wall:
					if (tm.GetTileAt (x, y - 1).Type == TileType.Wall) {
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

		m.SetVertices (vertices);
		m.SetUVs (0, uvs);
		m.SetIndices (indices.ToArray(), MeshTopology.Triangles, 0);
		m.RecalculateNormals ();

		mf.sharedMesh = m;

		mr.material = new Material (Shader.Find ("Standard"));
		mr.material.mainTexture = tileMap;
	}
}
