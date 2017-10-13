using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public bool weapon1 = true;
	public bool weapon2 = false;
	public bool weapon3 = false;
	public bool weapon4 = false;
	public float speed = 0.4f;
	public Vector3 projectileDirection;
    public GameObject projectilePrefab;
	public GameObject projectile2Prefab;
	public GameObject projectile3Prefab;
	public GameObject projectile4Prefab;

	Rigidbody m_Rigidbody;
	Transform m_SpriteTransform;

    private Plane groundPlane;

    private Spell spell;
	private Spell spell2;
	private Spell spell3;
	private Spell spell4;

	// Use this for initialization
	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> ();
        groundPlane = new Plane(new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0));

        spell = new Fireball(this.gameObject, projectilePrefab);
		spell2 = new Fireball(this.gameObject, projectile2Prefab);
		spell3 = new Fireball(this.gameObject, projectile3Prefab);
		spell4 = new Fireball(this.gameObject, projectile4Prefab);
	}
	
	// Update is called once per frame
	void Update () {
		// switching weapons
		if(Input.GetKey(KeyCode.Alpha1)){
			weapon1 = true;
			weapon2 = false;
			weapon3 = false;
			weapon4 = false;
		}
		else if(Input.GetKey(KeyCode.Alpha2)){
			weapon1 = false;
			weapon2 = true;
			weapon3 = false;
			weapon4 = false;
		}
		else if(Input.GetKey(KeyCode.Alpha3)){
			weapon1 = false;
			weapon2 = false;
			weapon3 = true;
			weapon4 = false;
		}
		else if(Input.GetKey(KeyCode.Alpha4)){
			weapon1 = false;
			weapon2 = false;
			weapon3 = false;
			weapon4 = true;
		}

        // Shooting
        if (Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float rayDistance;
			if (groundPlane.Raycast (ray, out rayDistance)) {
				projectileDirection = ray.origin + ray.direction * rayDistance - this.transform.position;
				projectileDirection.y = 0;
				projectileDirection = projectileDirection.normalized;
			}

			if (weapon1 == true) {
				spell.RequestCast (projectileDirection);
				//ProjectileBasic pBasic = ProjectileManager.CreateProjectile(projectilePrefab, EntityType.Player, this.gameObject, this.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
				//pBasic.Init(projectileDirection, 10.0f);
			} 
			else if (weapon2 == true) {
				spell2.RequestCast (projectileDirection);
			}
			else if (weapon3 == true) {
				spell3.RequestCast (projectileDirection);
			}
			else if (weapon4 == true) {
				spell4.RequestCast (projectileDirection);
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
