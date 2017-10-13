using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

	int _minX;
	int _minY;

	int _width;
	int _height;

	public int MinX {
		get {
			return _minX;
		}
	}

	public int MaxX {
		get {
			return _minX + _width;
		}
	}

	public int MinY {
		get {
			return _minY;
		}
	}

	public int MaxY {
		get {
			return _minY + _height;
		}
	}

	public int Width {
		get {
			return _width;
		}
	}

	public int Height {
		get {
			return _height;
		}
	}

	public Vector2 Center {
		get {
			return new Vector2 (MinX + _width / 2.0f, MinY + _height / 2.0f);
		}
	}

	public Vector2 Position {
		get {
			return new Vector2 (MinX, MinY);
		} set {
			_minX = Mathf.RoundToInt(value.x);
			_minY = Mathf.RoundToInt(value.y);
		}
	}

	public Room(int minX, int minY, int width, int height) {
		this._minX = minX;
		this._minY = minY;

		this._width = width;
		this._height = height;
	}
}

public class RoomManager : MonoBehaviour {

	List<Room> rooms;
	List<GameObject> visualizedRooms;

	int numberOfRooms = 20;

	// Use this for initialization
	void Start () {
		Random.InitState (0);

		rooms = new List<Room> ();
		for (int i = 0; i < numberOfRooms; i++) {
			rooms.Add( new Room (
				(int)Random.Range(-5, 5), (int)Random.Range(-5, 5),
				(int)Random.Range(5, 10), (int)Random.Range(5, 10)
			));
		}

		VisualizeRooms ();
	}

	int max_iterations = 150;
	int iterations = 0;
	float time = 0.0f;
	// Update is called once per frame
	void Update () {
		if (time <= 0.05f) {
			time += Time.deltaTime;
			return;
		}
		time = 0.0f;

		if (iterations >= max_iterations) {
			return;
		}

		Debug.Log (iterations);

		if (SeparateRooms ()) {
			iterations += 1;
		} else {
			iterations = max_iterations;
		}
	}

	bool SeparateRooms() {
		Dictionary<Room, Vector2> dict = new Dictionary<Room, Vector2> ();
		VisualizeRooms ();
		bool mustSeparate = false;

		for (int i = 0; i < rooms.Count; i++) {
			Room first = rooms [i];
			for (int j = i + 1; j < rooms.Count; j++) {
				Room second = rooms [j];
				Vector2 axis = Vector2.zero;
				float penetration = float.PositiveInfinity;

				Vector2 penetrationVector = Vector2.zero;

				float x0 = second.MaxX - first.MinX;
				float x1 = first.MaxX - second.MinX;

				if (x0 <= 0.0f || x1 <= 0.0f) {
					continue;
				}

				float temp = (x0 < x1) ? x0 : -x1;
				penetrationVector += Vector2.right * temp;

				float y0 = second.MaxY - first.MinY;
				float y1 = first.MaxY - second.MinY;


				if (y0 <= 0.0f || y1 <= 0.0f) {
					continue;
				}

				temp = (y0 < y1) ? y0 : -y1;
				penetrationVector += Vector2.up * temp;

				if (dict.ContainsKey (first)) {
					Vector2 pen = dict [first];
					dict [first] -= penetrationVector;
				} else {
					dict [first] = -penetrationVector;
				}

				if (dict.ContainsKey (second)) {
					dict [second] += penetrationVector;
				} else {
					dict [second] = penetrationVector;
				}
				
				mustSeparate = true;
			}
		}

		for (int i = 0; i < rooms.Count; i++) {
			if (!dict.ContainsKey(rooms[i])) {
				continue;
			}

			rooms [i].Position = rooms [i].Position - dict [rooms [i]].normalized;
		}

		return mustSeparate;
	}

	void VisualizeRooms() {
		if (visualizedRooms != null) {
			foreach (GameObject o in visualizedRooms) {
				Destroy (o);
			}
			visualizedRooms = null;
		}
		Random.InitState (0);
		
		visualizedRooms = new List<GameObject> ();
		int count = 0;
		foreach (Room r in rooms) {
			GameObject go = GameObject.CreatePrimitive (PrimitiveType.Quad);
			go.transform.parent = transform.parent;
			go.transform.position = r.Center;
			go.transform.name = "Room " + count;
			go.GetComponent<MeshRenderer> ().material.color = new Color (
				Random.Range (0, 1.0f), Random.Range (0, 1.0f), Random.Range (0, 1.0f));
			go.transform.localScale = new Vector3 (r.Width, r.Height, 1.0f);
			visualizedRooms.Add (go);
			count++;
		}
	}
}
