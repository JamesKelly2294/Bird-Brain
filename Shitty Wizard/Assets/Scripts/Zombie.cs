using UnityEngine;
using System.Collections;

// Nick Federico
// Calculates how the zombie will move and draws its debug lines
// No Restrictions/Known Errors

public class Zombie : VehicleMovement
{
	public GameObject zombieTarget;
	private GameObject sceneManager;
	private SceneManager sm;
	
	public float seekWeight;
	public float maxForce;

	public Material mat1;
	public Material mat2;
	public Material mat3;
	public Material mat4;

	// Use this for initialization
	public override void Start () 
	{
		// gets the scene manager script
		sm = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();

		// sets starting values
		maxSpeed = 7f;
		seekWeight = 3f;
		maxForce = 15f;

		base.Start ();
	}

	/// <summary>
	/// Calculates which way the zombie will move
	/// </summary>
	protected override void CalcSteeringForces()
	{
		// creates the zombies force vector
		Vector3 zombieForce = new Vector3 (0, 0, 0);

		// finds which human is closest to the zombie
		if (sm.humans.Count >= 0) 
		{
			FindClosestHuman ();
		}

		// pursues a human if there is one, otherwise wanders
		if (sm.humans.Count > 0 && zombieTarget != null) 
		{
			zombieForce += Pursuit(zombieTarget);
		} 
		else 
		{
			zombieForce += Wander();
		}

		// avoids all obstacles
		foreach (GameObject o in sm.obstacles)
		{
			zombieForce += AvoidObstacle (o, 2f);
		}

		// keeps zombies in bounds
		zombieForce += StayInBounds () * seekWeight;

		// clamps the max force
		zombieForce = Vector3.ClampMagnitude (zombieForce, maxForce);

		// applys all forces
		ApplyForce (zombieForce);
	}

	/// <summary>
	/// Finds which human is closest to each zombie
	/// </summary>
	void FindClosestHuman()
	{
		// create a float to hold the closests distance; set it to infinity
		float closestDistance = Mathf.Infinity;

		// if there isn't any zombies, then the zombies do not have a target
		if (sm.humans.Count == 0) 
		{
			zombieTarget = null;
			return;
		}
		
		// run through the entire list of zombies
		for (int i = 0; i < sm.humans.Count; i++) 
		{
			// find the distance between human and zombie
			float distance = Vector3.Distance(gameObject.transform.position, sm.humans[i].transform.position);
			
			// if the distance is less than the closest distance
			if(distance < closestDistance)
			{
				// set the target to the closest human
				zombieTarget = sm.humans[i];
				
				// set the current distance as the closest distance 
				closestDistance = distance;
			}
		}
	}

	/// <summary>
	/// Draws the debug lines for the zombie
	/// </summary>
	void OnRenderObject ()
	{
		// if the debug switch is on
		if(sm.debug)
		{
			// First line
			mat1.SetPass (0);

			// Begin to draw lines
			GL.Begin (GL.LINES); 

			// first point of the line
			GL.Vertex (position);

			// second point of the line
			GL.Vertex ((transform.forward * 2) + position);	

			// finishes the line
			GL.End ();									
		
			// Second line
			mat2.SetPass (0);
			GL.Begin (GL.LINES);
			GL.Vertex (position);
			GL.Vertex ((transform.right * 2) + position);
			GL.End ();

			// Third line is only drawn if there are zombies
			if(zombieTarget != null)
			{
				mat4.SetPass (0);
				GL.Begin (GL.LINES);
				GL.Vertex (position);
				GL.Vertex (zombieTarget.transform.position);
				GL.End ();
			}
		
			// Fourth line
			mat3.SetPass (0);
			GL.Begin (GL.LINES);
			for (float angle = 0; angle < 360; angle += 1) 
			{
				Vector3 circle = new Vector3 (Mathf.Cos (angle * Mathf.Deg2Rad) * radius + (position + velocity).x, (position + velocity).y, Mathf.Sin (angle * Mathf.Deg2Rad) * radius + (position + velocity).z);
				GL.Vertex3 (circle.x, circle.y, circle.z);
			}
			GL.End ();
		}
	}
}
