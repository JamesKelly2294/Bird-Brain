using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 300.0f;

	Rigidbody2D m_Rigidbody;
	Camera m_Camera;

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody2D> ();
		m_Camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 direction = Input.mousePosition - 
			Camera.main.WorldToScreenPoint(transform.position);
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

		float verticalInput = Input.GetAxis ("Vertical");
		float horizontalInput = Input.GetAxis ("Horizontal");

		m_Rigidbody.AddForce (Vector2.up * verticalInput * speed);
		m_Rigidbody.AddForce (Vector2.right * horizontalInput * speed);
	}
}
