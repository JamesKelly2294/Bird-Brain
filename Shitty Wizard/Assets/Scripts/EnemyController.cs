using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AIState
{
	IDLE,
	AGGROVATED
}

public class EnemyController : MonoBehaviour
{

	public float speed = 3.0f;
	public Transform target;
	public float targetAquisitionThreshold = 10.0f;
	public float targetAbandonmentThreshold = 20.0f;

	private AIState m_AIState;
	private float m_TimeSinceLastAIUpdate = 0.0f;
	private float m_AIUpdateRate = 0.1f;

	private Rigidbody m_Rigidbody;

	// Use this for initialization
	void Start ()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();

		transform.eulerAngles = Camera.main.transform.eulerAngles;
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		UpdateAIState ();

		UpdateFixedAIBehavior ();

		transform.position = new Vector3 (transform.position.x, transform.position.y, 0.0f);
	}

	void Update ()
	{
		UpdateAIBehavior ();
	}

	void UpdateFixedAIBehavior ()
	{
		switch (m_AIState) {
		case AIState.AGGROVATED:
			Vector3 direction = transform.position - target.position;
//			float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
//			transform.rotation = Quaternion.AngleAxis (angle + 90f, Vector3.forward);

			m_Rigidbody.AddForce (-1 * direction.normalized * speed);
			break;
		case AIState.IDLE:
			break;
		default:
			break;
		}
	}

	void UpdateAIBehavior ()
	{

	}

	void UpdateAIState ()
	{
		m_TimeSinceLastAIUpdate += Time.fixedDeltaTime;

		// if it is not time to update the AI, just return
		if (m_TimeSinceLastAIUpdate < m_AIUpdateRate) {
			return;
		}

		// reset update timer
		m_TimeSinceLastAIUpdate = 0.0f;

		// perform AI Update
		switch (m_AIState) {
		case AIState.AGGROVATED:
			break;
		case AIState.IDLE:
			if (Vector2.Distance (
				    	new Vector2 (transform.position.x, transform.position.y), 
				    	new Vector2 (target.position.x, target.position.y)
			    	) <= targetAquisitionThreshold) {
				m_AIState = AIState.AGGROVATED;
			}
			break;
		default:
			break;
		}
	}
}
