using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 3.0f;
	float verticalSpeed;


	Rigidbody m_Rigidbody;
	Transform m_SpriteTransform;

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> ();
		transform.eulerAngles = Camera.main.transform.eulerAngles;

		verticalSpeed = speed * ( 1.0f / Mathf.Sin (Mathf.Deg2Rad * 45.0f));
	}
	
	// Update is called once per frame
	void Update () {


	}

	void FixedUpdate() {
		float verticalInput = Input.GetAxis ("Vertical");
		float horizontalInput = Input.GetAxis ("Horizontal");

		m_Rigidbody.AddForce (Vector2.up * verticalInput * verticalSpeed );
		m_Rigidbody.AddForce (Vector2.right * horizontalInput * speed);

//		Vector3 direction = Camera.main.WorldToScreenPoint(transform.position) - 
//			Input.mousePosition;
//		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
//		m_SpriteTransform.rotation = Quaternion.AngleAxis(angle + 90f, Vector3.forward);

		transform.position = new Vector3 (transform.position.x, transform.position.y, 0.0f);
	}
}
