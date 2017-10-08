using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherTileManager : MonoBehaviour {

    public Texture2D textureMap;
    public Texture2D TEST_CEILING_TILE;
    public GameObject floorTile;
    public GameObject wallTile;
    public int width = 10;
    public int height = 10;

    const float sideScale = 1.0f / 0.70710666564f; //1.0f / Mathf.Sin (45.0f * Mathf.Deg2Rad);
    const float sideOffset = sideScale / 2.0f;

    // Use this for initialization
    void Start() {
        int layer = LayerMask.NameToLayer("Background");

        floorTile.transform.localScale = new Vector3(1.0f / 10.0f, 1.0f, sideScale / 10.0f);
        wallTile.transform.localScale = new Vector3(1.0f, sideScale, sideScale * 2.0f);

        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                if (col == 12 && (row == 7 || row == 8)) {
                    continue;
                }
                GameObject tile = Instantiate<GameObject>(floorTile);
                tile.transform.parent = transform;
                tile.transform.position = new Vector3(col + 0.5f, 0, row * sideScale + sideOffset);
                tile.layer = layer;
            }
        }

        //		for (int row = 12; row < 14; row++) {
        //			for (int col = 4; col < 14; col++) {
        //				GameObject tile = Instantiate<GameObject> (wallTile);
        //				tile.transform.parent = transform;
        //				tile.transform.position = new Vector3 (col + 0.5f, row * sideScale + sideOffset, -sideScale);
        //			}
        //		}

        GenerateWalls();

    }

    // Update is called once per frame
    void Update() {

    }

    // THIS IS NOT EVEN CLOSE TO GOOD CODE
    // this is only for testing
    // it will be completely refactored
    void GenerateWalls() {
        GameObject walls = new GameObject("walls");
        walls.AddComponent<MeshRenderer>();
        walls.AddComponent<MeshFilter>();

        Mesh m = new Mesh();

        Vector3[] verts =  {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, -1.0f),
            new Vector3(1.0f, 0.0f, -1.0f),
            new Vector3(1.0f, 0.0f, 0.0f)
        };

        Vector2[] uvs =  {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.0f, 2.0f),
            new Vector2(1.0f, 2.0f),
            new Vector2(1.0f, 0.0f)
        };

        int[] indices = {
            0, 1, 2,
            2, 3, 0
        };

        m.vertices = verts;
        m.uv = uvs;
        m.SetIndices(indices, MeshTopology.Triangles, 0);

        m.RecalculateNormals();

        MeshFilter mf = walls.GetComponent<MeshFilter>();
        mf.mesh = m;

        MeshRenderer mr = walls.GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Standard"));
        mr.material.mainTexture = textureMap;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;

        walls.transform.position = new Vector3(12.0f, 7.0f * sideScale, 0.0f);
        walls.transform.localScale = new Vector3(1.0f, sideScale, sideScale * 2.0f);
        walls.transform.SetParent(transform);

        ////

        GameObject ceilings = new GameObject("ceilings");
        ceilings.AddComponent<MeshRenderer>();
        ceilings.AddComponent<MeshFilter>();

        Mesh ceiling_m = new Mesh();

        Vector3[] ceilingVerts =  {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(1.0f, 1.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f)
        };

        Vector2[] ceilingUVs =  {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(1.0f, 0.0f)
        };

        ceiling_m.vertices = ceilingVerts;
        ceiling_m.SetIndices(m.GetIndices(0), MeshTopology.Triangles, 0);
        ceiling_m.RecalculateNormals();
        ceiling_m.uv = ceilingUVs;

        MeshFilter ceiling_mf = ceilings.GetComponent<MeshFilter>();
        ceiling_mf.mesh = ceiling_m;

        MeshRenderer ceiling_mr = ceilings.GetComponent<MeshRenderer>();
        ceiling_mr.material = new Material(Shader.Find("Standard"));
        ceiling_mr.material.mainTexture = TEST_CEILING_TILE;

        ceilings.transform.position = new Vector3(12.0f, 7.0f * sideScale, -2.0f * sideScale);
        ceilings.transform.localScale = new Vector3(1.0f, sideScale, 1.0f);
        ceilings.transform.SetParent(transform);

        walls.AddComponent<BoxCollider>();
        walls.GetComponent<BoxCollider>().size = new Vector3(1.0f, 2.0f, 1.0f);
        walls.GetComponent<BoxCollider>().center = new Vector3(0.5f, 1.0f, -0.5f);
    }
}
