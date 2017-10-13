using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 0.4f;
	public Vector3 projectileDirection;
    public GameObject projectilePrefab;
	public GameObject projectile2Prefab;
	public GameObject projectile3Prefab;
	public GameObject projectile4Prefab;

	Rigidbody m_Rigidbody;
	Transform m_SpriteTransform;

    private Plane groundPlane;

    private int currentSpell;
    private Spell[] spells = new Spell[] { null };

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> ();
        groundPlane = new Plane(new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0));

        spells = new Spell[]
        {
            new Fireball(this.gameObject, projectilePrefab),
            new Fireball(this.gameObject, projectile2Prefab),
            new Fireball(this.gameObject, projectile3Prefab),
            new IceCone(this.gameObject, projectile4Prefab)
        };

	}
	
	// Update is called once per frame
	void Update () {
		// switching weapons
        if (Input.GetKey(KeyCode.E)) {
            currentSpell++;
            if (currentSpell >= spells.Length) {
                currentSpell -= spells.Length;
            }
        } else if (Input.GetKey(KeyCode.Q)) {
            currentSpell--;
            if (currentSpell < 0) {
                currentSpell += spells.Length;
            }
        }
		if(Input.GetKey(KeyCode.Alpha1)){
            currentSpell = 0;
		}
		else if(Input.GetKey(KeyCode.Alpha2)){
            currentSpell = 1;
        }
		else if(Input.GetKey(KeyCode.Alpha3)){
            currentSpell = 2;
        }
		else if(Input.GetKey(KeyCode.Alpha4)){
            currentSpell = 3;
        }

        // Shooting
        if (Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float rayDistance;
			if (groundPlane.Raycast (ray, out rayDistance)) {

				projectileDirection = ray.origin + ray.direction * rayDistance - this.transform.position;
				projectileDirection.y = 0;
				projectileDirection = projectileDirection.normalized;

                spells[currentSpell].RequestCast(projectileDirection);

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
