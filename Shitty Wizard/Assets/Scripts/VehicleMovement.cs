using UnityEngine;
using System.Collections;

// Nick Federico
// Controls all of the vectors in the scene
// No Restrictions/Known Errors

// gives each object a character controller
[RequireComponent (typeof(CharacterController))]

public abstract class VehicleMovement : MonoBehaviour 
{
	// Vectors for force-based movement
	public Vector3 position;
	public Vector3 direction;
	public Vector3 acceleration;
	public Vector3 velocity;

	public float mass;
	public float maxSpeed;
	public float radius;

	private float x;
	private float y;
	private float wanderDistance;
	private float offsetAngle;
	private float wanderCircleRadius;

	protected abstract void CalcSteeringForces ();

	// Use this for initialization
	public virtual void Start () 
	{
		radius = 1f;
		mass = 1;
		wanderDistance = 10f;
		offsetAngle = 20f;
		wanderCircleRadius = 8f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		CalcSteeringForces ();
		UpdatePosition ();
		SetTransform ();
	}

	/// <summary>
	/// UpdatePosition
	/// Calculate a new position for this vehicle based on incoming forces
	/// </summary>
	void UpdatePosition()
	{
		// Grab the world position from the transform component
		position = gameObject.transform.position;

		// Step 1: Add accel to vel * time
		velocity += acceleration * Time.deltaTime;

		gameObject.GetComponent<CharacterController> ().Move (velocity * Time.deltaTime);

		// Step 2: Add velocity to position
		position += velocity * Time.deltaTime;

		// Step 3: Derive a direction
		direction = velocity.normalized;

		// Step 4: Zero out acceleration
		// (start fresh with new forces every frame)
		acceleration = Vector3.zero;
	}

	/// <summary>
	/// Set the transform component to reflect the local position vector
	/// </summary>
	void SetTransform()
	{
		gameObject.transform.forward = direction;
	}

	protected void ApplyForce(Vector3 force)
	{
		acceleration += force / mass;
	}

	protected Vector3 Seek (Vector3 targetPos)
	{
		// Step 1: Calculate desired velocity
		// this is the vector pointing form myself to the target
		Vector3 desiredVelocity = targetPos - position;

		// Step 2: Scale desired to the maximum speed
		// so that I move as quickly as possible
		desiredVelocity = Vector3.ClampMagnitude (desiredVelocity, maxSpeed);

		desiredVelocity.Normalize ();
		desiredVelocity *= maxSpeed;

		// Step 3: Calculate the steering force for seeking
		Vector3 steeringForce = desiredVelocity - velocity;

		return steeringForce;
	}

	protected Vector3 Flee (Vector3 targetPos)
	{
		// Step 1: Calculate desired velocity
		// this is the vector pointing form myself to the target
		Vector3 desiredVelocity = position - targetPos;
		
		// Step 2: Scale desired to the maximum speed
		// so that I move as quickly as possible
		desiredVelocity = Vector3.ClampMagnitude (desiredVelocity, maxSpeed);
		
		desiredVelocity.Normalize ();
		desiredVelocity *= maxSpeed;
		
		// Step 3: Calculate the steering force for seeking
		Vector3 steeringForce = desiredVelocity - velocity;
		
		return steeringForce;
	}

	/// <summary>
	/// Keeps the objects inside of the plane boundaries by seeking the center
	/// </summary>
	protected Vector3 StayInBounds ()
	{
		Vector3 center = new Vector3 (0f, .5f, 0f);

		if(gameObject.transform.position.x >= 25f)
		{
			return Seek (center);
		}
		if(gameObject.transform.position.x <= -25f)
		{
			return Seek (center);
		}
		if(gameObject.transform.position.z >= 25f)
		{
			return Seek (center);
		}
		if(gameObject.transform.position.z <= -25f)
		{
			return Seek (center);
		}

		return Vector3.zero;
	}

	/// <summary>
	/// Makes the objects in the scene avoid obstacles
	/// </summary>
	protected Vector3 AvoidObstacle(GameObject obst, float safeDistance) 
	{
		Vector3 steer = Vector3.zero;

		// Create vecToCenter-a vector from the character to the center of the obstacle
		Vector3 obstCenter = position - obst.transform.position;

		// Find the distance to the obstacle
		float distToObst = obstCenter.magnitude;

		// Return a zero vector if the obstacle is too far to concern us
		// Use safe distance to determine how large the “safe zone” is 
		if (distToObst > safeDistance) 
		{
			return steer;
		}

		// Return a zero vector if the obstacle is behind us 
		// (dot product of vecToCenterand forward is negative)
		if(Vector3.Dot (this.transform.forward, obst.transform.position) < 0)
		{
			return steer;
		}

		// Use the dot product of the vector to obstacle center (vecToCenter) and the unit vector 
		//   to the right (right) of the vehicle to find the projected distance between the centers
		//   of the vehicle and the obstacle
		// Compare this to the sum of the radii and return a zero vector if we can pass safely
		if (Mathf.Abs (Vector3.Dot(obstCenter, gameObject.transform.right)) > (radius + 1f))
		{
			return steer;
		}

		// If we get this far we are on a collision course and must steer away!
		// Use the sign of the dot product between the vector to center (vecToCenter) and the      
		//   vector to the right (right) to determine whether to steer left or right
		Vector3 desiredVelocity = Vector3.zero;

		if (Vector3.Dot (obstCenter, gameObject.transform.right) < 0) {
			desiredVelocity = gameObject.transform.right * -1;
		} 
		else 
		{
			desiredVelocity = gameObject.transform.right;
		}

		// Calculate desired velocity using the right vector or negated right vector and maxSpeed
		desiredVelocity *= maxSpeed;

		// Compute the steering force required to change current velocity to desired velocity
		steer = desiredVelocity - velocity;

		// Consider multiplying this force by safeDistance/distto increase the relative weight
		//   of the steering force when obstacles are closer.

		return steer * (safeDistance/distToObst);
	}

	/// <summary>
	/// Makes the objects in the scene wander around with no objective
	/// </summary>
	protected Vector3 Wander()
	{
		Vector3 wanderCenter = position + (transform.forward * wanderDistance) * Random.Range (1, 26);

		x += .9f;
		y += 3f;

		// gets an angle using perlin noise
		float angle = (Mathf.PerlinNoise (x, y) * 360) + offsetAngle;

		// converts the angle
		angle *= Mathf.Deg2Rad;

		// creates a vector of where the object will wander to
		Vector3 wanderTarget = new Vector3 (wanderCenter.x + (Mathf.Cos (angle) * wanderCircleRadius), 0f, wanderCenter.z + (Mathf.Cos (angle) * wanderCircleRadius));

		// creates the velocity vector
		Vector3 desiredVelocity = wanderTarget - position;

		desiredVelocity.Normalize ();
		desiredVelocity *= maxSpeed;

		// creates the wander force
		Vector3 wanderForce = desiredVelocity - velocity;

		return wanderForce;
	}

	/// <summary>
	/// Advanced seek method that looks at future position
	/// </summary>
	protected Vector3 Pursuit(GameObject target)
	{
		Vector3 distance = target.GetComponent<VehicleMovement>().position - position;	
		float X = distance.magnitude / maxSpeed;
		
		Vector3 futurePos = target.GetComponent<VehicleMovement>().position + target.GetComponent<VehicleMovement>().velocity * X;
		
		return Seek (futurePos);
	}

	/// <summary>
	/// Advanced flee method that looks at future position
	/// </summary>
	protected Vector3 Evade(GameObject target)
	{
		Vector3 distance = target.GetComponent<VehicleMovement>().position - position;	
		float updatesAhead = distance.magnitude / maxSpeed;
		
		Vector3 futurePos = target.GetComponent<VehicleMovement>().position + target.GetComponent<VehicleMovement>().velocity * updatesAhead * .5f;
		
		return Flee (futurePos);
	}
}
