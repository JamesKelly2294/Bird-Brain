//using UnityEngine;
//using System.Collections;
//
//// Nick Federico
//// Calculates how the human will move and draws its debug lines
//// No Restrictions/Known Errors
//
//public class Human : VehicleMovement
//{
//	public GameObject zombieChaser;
//
//	public float seekWeight;
//	public float fleeWeight;
//	public float maxForce;
//
//	private SceneManager sm;
//
//	public Material mat1;
//	public Material mat2;
//	public Material mat3;
//	
//	// Use this for initialization
//	public override void Start () 
//	{
//		// gets the scene manager script
//		sm = GameObject.Find("SceneManager").GetComponent<SceneManager>();
//
//		// sets starting values
//		maxSpeed = 5f;
//		seekWeight = 3f;
//		fleeWeight = 3f;
//		maxForce = 20f;
//		base.Start ();
//	}
//
//	/// <summary>
//	/// Calculates which way the human will move
//	/// </summary>
//	protected override void CalcSteeringForces()
//	{
//		// creates the zombies force vector
//		Vector3 humanForce = new Vector3 (0, 0, 0);
//
//		// makes the human wander always
//		humanForce += Wander();
//
//		// checks through all zombies and evades if the zombie is too close
//		foreach (GameObject z in sm.zombies) 
//		{
//			Vector3 humanPos = gameObject.transform.position;
//			Vector3 zombiePos = z.transform.position;
//
//			float distance = Vector3.Distance (humanPos, zombiePos);
//
//			if (distance < 6.0f) 
//			{
//				humanForce += Evade(z) * fleeWeight;
//			}
//
//			// keeps the human in bounds
//			humanForce += StayInBounds () * seekWeight;
//		}
//
//		// avoids all obstacles
//		foreach(GameObject o in sm.obstacles)
//		{
//			humanForce += AvoidObstacle(o, 2f);
//		}
//
//		// clamps the max human force
//		humanForce = Vector3.ClampMagnitude (humanForce, maxForce);
//
//		// applys all forces
//		ApplyForce (humanForce);
//	}
//
//	/// <summary>
//	/// Draws the debug lines for the zombie
//	/// </summary>
//	void OnRenderObject ()
//	{
//		// if the debug switch is on
//		if (sm.debug)
//		{
//			// First line
//			mat1.SetPass (0);
//
//			// begins to draw the line
//			GL.Begin (GL.LINES);
//
//			// first point of the line
//			GL.Vertex (position);
//
//			// second point of the line
//			GL.Vertex ((transform.forward * 2) + position);
//
//			// finishes the line
//			GL.End ();
//		
//			// Second line
//			mat2.SetPass (0);
//			GL.Begin (GL.LINES);
//			GL.Vertex (position);
//			GL.Vertex ((transform.right * 2) + position);
//			GL.End ();
//
//			// Third line
//			mat3.SetPass (0);
//			GL.Begin (GL.LINES);
//			for (float angle = 0; angle < 360; angle += 1) 
//			{
//				Vector3 circle = new Vector3 (Mathf.Cos (angle * Mathf.Deg2Rad) * radius + (position + velocity).x, (position + velocity).y, Mathf.Sin (angle * Mathf.Deg2Rad) * radius + (position + velocity).z);
//				GL.Vertex3 (circle.x, circle.y, circle.z);
//			}
//			GL.End ();
//		}
//	}
//}
