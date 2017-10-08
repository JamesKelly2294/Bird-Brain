using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 0.4f;

	Rigidbody m_Rigidbody;
	Transform m_SpriteTransform;

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {


	}

	void FixedUpdate() {
		float verticalInput = 0.0f; // = Input.GetAxis ("Vertical");
		float horizontalInput = 0.0f; // = Input.GetAxis ("Horizontal");

		if (Input.GetKey (KeyCode.W)) {
			verticalInput = 1.0f;
		}
		if (Input.GetKey (KeyCode.S)) {
			verticalInput = -1.0f;
		}
		if (Input.GetKey (KeyCode.A)) {
			horizontalInput = -1.0f;
		}
		if (Input.GetKey (KeyCode.D)) {
			horizontalInput = 1.0f;
		}

		m_Rigidbody.velocity = new Vector3 (horizontalInput * speed, 0.0f, verticalInput * speed);
		transform.position = new Vector3 (transform.position.x, 0.0f, transform.position.z);
	}
}
