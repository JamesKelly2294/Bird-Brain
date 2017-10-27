using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class PlayerController : MonoBehaviour {

	public float speed = 0.4f;
	Transform m_SpriteTransform;
    Entity entity;

    private Plane groundPlane;

    private int currentSpell;
    private List<Spell> spells;

    private IceTileManager itm;
    private float iceAcceleration = 0.2f;
    private Vector3 moveVec = Vector3.zero;

	// Use this for initialization
	void Start () {

        entity = GetComponent<Entity>();
        groundPlane = new Plane(new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0));

        itm = GetComponent<IceTileManager>();

        // Gather spells
        spells = new List<Spell>();
        Transform spellsTransform = this.transform.Find("Spells");
        foreach (Spell s in spellsTransform.GetComponentsInChildren<Spell>()) {
            spells.Add(s);
        }
	}
	
	// Update is called once per frame
	void Update () {
		// switching weapons
        if (Input.GetKeyDown(KeyCode.E)) {
            currentSpell++;
        } else if (Input.GetKeyDown(KeyCode.Q)) {
            currentSpell--;
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

		currentSpell = currentSpell % spells.Count;
        while (currentSpell < 0) {
            currentSpell += spells.Count;
        }

        // Shooting
        if (Input.GetMouseButton(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			float rayDistance;
			if (groundPlane.Raycast (ray, out rayDistance)) {

				Vector3 projectileDirection = ray.origin + ray.direction * rayDistance - this.transform.position;
				projectileDirection.y = 0;
				projectileDirection.z -= 0.5f;
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

        //rbody.velocity = new Vector3 (horizontalInput * speed, 0.0f, verticalInput * speed);
        Vector3 goalDir = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;
        if (itm.onIce) {
            moveVec = moveVec + goalDir * iceAcceleration;
            if (moveVec.magnitude > speed) {
                moveVec = moveVec.normalized * speed;
            }
        } else {
            moveVec = goalDir * speed;
        }
		entity.Move(moveVec);
		transform.position = new Vector3 (transform.position.x, 0.0f, transform.position.z);

    }
}
