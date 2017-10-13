using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge<T> {
	public T first;
	public T second;

	public float weight;

	public Edge(T first, T second, float weight) {
		this.first = first;
		this.second = second;
		this.weight = weight;
	}

	public override string ToString() {
		return string.Format("({0}) [{1} <-> {2}]", weight, first, second);
	}
}

public class Vertex<T> {
	private static int currentID = 0;

	public T data;
	private int id;

	public Vertex(T data) {
		this.data = data;
		this.id = currentID;
		currentID += 1;
	}

	public int ID {
		get {
			return id;
		}
	}

	public override bool Equals(System.Object obj) {
		if (obj == null || GetType() != obj.GetType())
         return false;

    Vertex<T> v = (Vertex<T>)obj;
    return this.id == v.id;
	}

	public override int GetHashCode() {
		return this.id;
	}
}

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

		public override bool Equals(System.Object obj) {
			if (obj == null || GetType() != obj.GetType())
					 return false;

			Room r = (Room)obj;
			return this.MinX == r.MinX && this.MinY == r.MinY
				&& this.Width == r.Width && this.Height == r.Height;
		}

		public override int GetHashCode() {
			return (int)((float)(this.MinX * 1000 + this.MinY) /
				(this.Width * 100 + this.Height * 10));
		}

		public override string ToString() {
			return string.Format("{0}", Center);
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
    int max_iterations = 200;
    int iterations = 0;

    // Use this for initialization
    void Start () {
        rooms = new List<Room> ();
        for (int i = 0; i < numberOfRooms; i++) {
            rooms.Add( new Room (
                (int)Random.Range(-10, 10), (int)Random.Range(-10, 10),
                (int)Random.Range(7, 9), (int)Random.Range(7, 9)
            ));
        }

        while (iterations < max_iterations) {
            if (!SeparateRooms ()) {
                break;
            }
            iterations += 1;
        }

        RemoveOverlappingRooms ();
				BuildEdgeMap();
        VisualizeRooms ();
    }

		List<Edge<Vertex<Room>>> edgemap;
		List<Vertex<Room>> vertexmap;
		public void BuildEdgeMap() {
			edgemap = new List<Edge<Vertex<Room>>>();
			vertexmap = new List<Vertex<Room>>();

			for (int i = 0 ; i < rooms.Count; i++) {
				vertexmap.Add(new Vertex<Room>(rooms[i]));
			}

			for (int i = 0; i < rooms.Count; i++) {
					Room first = rooms [i];
					Vertex<Room> firstV = vertexmap.Find(x => x.data.Equals(first));
					for (int j = i + 1; j < rooms.Count; j++) {
							Room second = rooms[j];
							Vertex<Room> secondV = vertexmap.Find(x => x.data.Equals(second));

							float distance = Vector2.Distance(first.Center, second.Center);
							Edge<Vertex<Room>> e = new Edge<Vertex<Room>>(
								firstV, secondV, distance
							);
							edgemap.Add(e);
					}
			}

			// sort edges in ascending order
			edgemap.Sort((first, second) => {
				float res = first.weight - second.weight;
				if (res > 0) {
					return 1;
				} else if (res < 0) {
					return -1;
				} else {
					return 0;
				}
			});

			Dictionary<int, int> parents = new Dictionary<int, int>();
			foreach(Vertex<Room> v in vertexmap) {
				parents[v.ID] = -1;
			}

			List<Edge<Vertex<Room>>> MST = new List<Edge<Vertex<Room>>>();
			foreach (Edge<Vertex<Room>> e in edgemap) {
				int x = Find(parents, e.first.ID);
				int y = Find(parents, e.second.ID);

				if (x == y) {
					// cycle detected, don't add edge
					continue;
				}

				Union(parents, e.first, e.second);
				MST.Add(e);

				if (MST.Count >= vertexmap.Count - 1) {
					break;
				}
			}

			edgemap = MST;
		}

		int Find(Dictionary<int, int> parents, int v) {
			if (parents[v] == -1) {
				return v;
			}
			return Find(parents, parents[v]);
		}

		void Union(Dictionary<int, int> parents, Vertex<Room> first, Vertex<Room> second) {
			int firstParent = Find(parents, first.ID);
			int secondParent = Find(parents, second.ID);

			parents[firstParent] = secondParent;
		}

    void Update () {

    }

    void RemoveOverlappingRooms() {
        List<Room> temp = new List<Room> ();
        for (int i = 0; i < rooms.Count; i++) {
            Room first = rooms [i];

            // this is inefficient.
            // papa james doesn't care.
            bool doesNotOverlap = true;
            for (int j = 0; j < rooms.Count; j++) {
                if (i == j)
                    continue;
                Room second = rooms [j];

                if (second.MaxX - first.MinX <= 0.0f
                    || first.MaxX - second.MinX <= 0.0f
                     || second.MaxY - first.MinY <= 0.0f
                    || first.MaxY - second.MinY <= 0.0f ) {
                    continue;
                }
                doesNotOverlap = false;
                break;
            }

            if (doesNotOverlap) {
                temp.Add (first);
            }
        }
        rooms = temp;
    }

    bool SeparateRooms() {
        Dictionary<Room, Vector2> dict = new Dictionary<Room, Vector2> ();
        bool mustSeparate = false;

        for (int i = 0; i < rooms.Count; i++) {
            Room first = rooms [i];
            for (int j = i + 1; j < rooms.Count; j++) {
                Room second = rooms [j];
                Vector2 axis = Vector2.zero;

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

				count = 0;

				foreach (Edge<Vertex<Room>> e in edgemap) {
					int positionCount = 3;
					Vector3[] positions = new Vector3[positionCount];
					GameObject edgemapGO = new GameObject();
					edgemapGO.transform.parent = transform.parent;
					edgemapGO.transform.name = "Edge";
					edgemapGO.AddComponent<LineRenderer> ();
					edgemapGO.GetComponent<LineRenderer> ().positionCount = positionCount;
					positions[0] = new Vector3(Mathf.Round(e.first.data.Center.x), Mathf.Round(e.first.data.Center.y), -1.0f);
					positions[1] = new Vector3(Mathf.Round(e.first.data.Center.x), Mathf.Round(e.second.data.Center.y), -1.0f);
					positions[2] = new Vector3(Mathf.Round(e.second.data.Center.x), Mathf.Round(e.second.data.Center.y), -1.0f);
					edgemapGO.GetComponent<LineRenderer> ().SetPositions(positions);
					edgemapGO.GetComponent<LineRenderer> ().startWidth = 0.2f;
					edgemapGO.GetComponent<LineRenderer> ().endWidth = 0.2f;
					visualizedRooms.Add (edgemapGO);
          count++;
        }

    }
}
