using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTileManager : MonoBehaviour {

    public GameObject iceTilePrefab;
    public bool onIce = false;

    Dictionary<int, GameObject> tileDictionary;

	// Use this for initialization
	void Start () {
        tileDictionary = new Dictionary<int, GameObject>();
	}
	
	// Update is called once per frame
	void Update () {

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position + Vector3.down * 1.0f, Vector3.up, out hit)) {
            if (hit.transform.tag == "IceTile") {
                onIce = true;
            } else {
                onIce = false;
            }
        } else {
            onIce = false;
        }

	}

    public void AddTile(Vector2 _pos) {

        int x = (int)Mathf.Floor(_pos.x);
        int y = (int)Mathf.Floor(_pos.y);
        int hash = HashPosition(x, y);

        if (!tileDictionary.ContainsKey(hash)) {
            GameObject newTile = Instantiate(iceTilePrefab, new Vector3(x + 0.5f, 0.01f, y + 0.5f), Quaternion.identity);
            IceTile iceTile = newTile.GetComponent<IceTile>();
            iceTile.Init(this, hash);
            tileDictionary.Add(hash, newTile);
        }

    }

    private int HashPosition(int x, int y) {
        return x * 1000000 + y;
    }

    public void RemoveTile(int hash) {
        Destroy(tileDictionary[hash]);
        tileDictionary.Remove(hash);
    }

}
