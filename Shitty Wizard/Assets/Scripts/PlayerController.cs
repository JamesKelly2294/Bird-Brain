using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 0.4f;
	float verticalSpeed;


	Rigidbody m_Rigidbody;
	Transform m_SpriteTransform;

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> ();
		transform.eulerAngles = Camera.main.transform.eulerAngles;

	}
	
	// Update is called once per frame
	void Update () {


	}

	void FixedUpdate() {
		float verticalInput = 0.0f; // = Input.GetAxis ("Vertical");
		float horizontalInput = 0.0f; // = Input.GetAxis ("Horizontal");

		// this let to some floating controls
//		m_Rigidbody.AddForce (Vector2.up * verticalInput * verticalSpeed );
//		m_Rigidbody.AddForce (Vector2.right * horizontalInput * speed);
		//		m_Rigidbody.velocity = new Vector3 (horizontalInput * speed, verticalInput * verticalSpeed, 0.0f);

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

		m_Rigidbody.velocity = new Vector3 (horizontalInput * speed, 0.0f, verticalInput * verticalSpeed);

		transform.position = new Vector3 (transform.position.x, 0.0f, transform.position.z);
	}

	void OnValidate() {
		verticalSpeed = speed * ( 1.0f / Mathf.Sin (Mathf.Deg2Rad * 45.0f));
	}
}
