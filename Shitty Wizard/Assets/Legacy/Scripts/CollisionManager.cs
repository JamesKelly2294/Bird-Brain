//using UnityEngine;
//using System.Collections;
//
//// Nick Federico
//// Handles collisions
//// No Restrictions/Known Errors
//
//public class CollisionManager : MonoBehaviour 
//{
//	private SceneManager sm;
//
//	public Vector3 zomb;
//	public Vector3 hum;
//
//	// Use this for initialization
//	void Start () 
//	{
//		// gets the scene manager script
//		sm = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
//	}
//	
//	// Update is called once per frame
//	void Update () 
//	{
//		// goes through all zombies
//		foreach (GameObject z in sm.zombies) 
//		{
//			// goes through all humans
//			foreach (GameObject h in sm.humans) 
//			{
//				// finds the human and zombies positions
//				hum = new Vector3 (h.transform.position.x, h.transform.position.y, h.transform.position.z);
//				zomb = new Vector3 (z.transform.position.x, z.transform.position.y, z.transform.position.z);
//
//				// finds distance between humans and zombies
//				float distance = Vector3.Distance (hum, zomb);
//
//				// spawns a zombie and kills a human if the zombie gets close enough
//				if (distance < 2f) 
//				{
//					sm.zombies.Add((GameObject)Instantiate (sm.zombie, new Vector3 (h.transform.position.x, 0, h.transform.position.z), h.transform.rotation));
//					Destroy(h);
//					sm.humans.Remove(h);
//					return;
//				} 
//			}
//		}
//	}
//}
