using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public float speed = 100.0f;
	public Transform target;

	Rigidbody2D m_Rigidbody;

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update () {
		Vector3 direction = target.position - 
			transform.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

		m_Rigidbody.AddForce (transform.up * speed);
	}
}
