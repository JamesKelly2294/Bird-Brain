using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 0.4f;
    public GameObject projectilePrefab;

	Rigidbody m_Rigidbody;
	Transform m_SpriteTransform;

    private Plane groundPlane;

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> ();
        groundPlane = new Plane(new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0));
	}
	
	// Update is called once per frame
	void Update () {
        
        // Shooting
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance)) {
                Vector3 projectileDirection = ray.origin + ray.direction * rayDistance - this.transform.position;
                projectileDirection.y = 0;
                projectileDirection = projectileDirection.normalized;
                ProjectileBasic pBasic = ProjectileManager.CreateProjectile(projectilePrefab, EntityType.Player, this.gameObject, this.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
                pBasic.Init(projectileDirection, 10.0f);
            }

        }

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
