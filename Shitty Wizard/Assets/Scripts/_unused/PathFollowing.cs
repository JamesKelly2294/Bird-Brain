//using UnityEngine;
//using System.Collections;
//
//// Nick Federico
//// Controls how an object will follow the created path
//// No Restrictions/Known Errors
//
//public class PathFollowing : VehicleMovement 
//{
//	private SceneManager sm;
//	private DebugLines dl;
//
//	public Vector3 futurePos;
//	public Vector3 ultimateForce;
//	public Vector3 pointForce;
//	public Vector3 yCheck;
//	public Vector3 startFuturePos;
//	public Vector3 currLine;
//	public Vector3 closestPoint;
//	public Vector3 separateForce;
//
//	public float pointDistance;
//	public float scaleProj;
//	public float pathRadius;
//	public float separateWeight;
//	public int point;
//
//	public Material mat1;
//	public Material mat2;
//
//	// Use this for initialization
//	public override void Start () 
//	{
//		sm = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
//		dl = GameObject.Find ("SceneManager").GetComponent<DebugLines> ();
//
//		point = 0;
//		pathRadius = 3f;
//		separateWeight = 2f;
//		maxSpeed = 10;
//
//		base.Start ();
//	}
//	
//	// Update is called once per frame
//	protected override void CalcSteeringForces()
//	{
//		ultimateForce = Vector3.zero;
//		ultimateForce += CalcPoint ();
//		ultimateForce += Separate ();
//		
//		ApplyForce (ultimateForce);
//	}
//
//	Vector3 CalcPoint()
//	{	
//		// Seeks the current waypoint
//		pointForce = Seek (sm.pathList [point].transform.position);
//		
//		// If waypoint is too close, change the waypoint
//		if (Vector3.Distance (transform.position, sm.pathList [point].transform.position) < 1.5f) 
//		{
//			point++;
//		}
//		if (point > 7) 
//		{
//			point = 0;
//		}
//		
//		// If the object moves out of the radius of the path, seek the closest point
//		if (Vector3.Distance (transform.position, CalcClosePoint ()) > pathRadius) 
//		{
//			pointForce += Seek (closestPoint);
//		}
//		
//		return pointForce;
//	}
//
//	/// <summary>
//	/// Calculates the Closest point on a line segment
//	/// </summary>
//	Vector3 CalcClosePoint()
//	{
//		// Gets future position
//		futurePos = transform.position + velocity * 1f;
//		
//		// Gets BFP (Beginning future position)
//		startFuturePos = futurePos - sm.pathList [point].transform.position;
//		
//		//Get the line segment between waypoints, if statement is used to change the index for last line segment
//		if (point == 0) 
//		{
//			currLine = sm.pathList [point].transform.position - sm.pathList [7].transform.position;
//		}
//		else 
//		{
//			currLine = sm.pathList [point].transform.position - sm.pathList [point - 1].transform.position;
//		}
//		
//		// Gets the scalar project of BFP onto the line segment
//		scaleProj = Vector3.Dot (startFuturePos, currLine) / currLine.magnitude;
//		
//		// Gets the closest point on the line segment 
//		closestPoint = sm.pathList [point].transform.position + currLine.normalized * scaleProj;
//		
//		return closestPoint;
//	}
//
//	/// <summary>
//	/// Separates from any other path followers.
//	/// </summary>
//	protected Vector3 Separate()
//	{
//		separateForce = Vector3.zero;
//		
//		//checks for each member to see if it is close enough for a need to separate
//		foreach (GameObject tank in sm.pathFollowingList)
//		{
//			
//			// adds a strong flee force if within 3 units
//			if (Vector3.Distance(tank.transform.position, this.transform.position) < 6f)
//			{
//				separateForce += (Flee (tank.transform.position) * separateWeight);
//			}
//			// adds a week flee force if within 6 units
//			if (Vector3.Distance(tank.transform.position, this.transform.position) < 5)
//			{
//				separateForce += (Flee (tank.transform.position) * separateWeight);
//			}
//		}
//		
//		return separateForce;
//	}
//
//	// draws debug lines for future position and closest point
//	void OnRenderObject()
//	{
//		if (dl.debug) 
//		{
//			mat1.SetPass (0);
//			GL.Begin(GL.LINES);
//			GL.Vertex(transform.position);
//			GL.Vertex(futurePos);
//			GL.End();
//
//			mat2.SetPass (0);
//			GL.Begin(GL.LINES);
//			GL.Vertex(futurePos);
//			GL.Vertex(closestPoint);
//			GL.End();
//
//			G
//		}
//	}
//}
