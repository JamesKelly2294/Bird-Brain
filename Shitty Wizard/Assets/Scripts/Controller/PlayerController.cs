using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Entity))]
public class PlayerController : MonoBehaviour {

    public enum ControlMode {
        KeyboardAndMouse,
        Mobile
    }
    public ControlMode controlMode;

	public float speed = 0.4f;
	Transform m_SpriteTransform;
    Entity entity;

    private Plane groundPlane;

	public int CurrentSpell {
		get {
			return currentSpell;
		}
	}

    private int currentSpell;
    private List<Spell> spells;

    private IceTileManager itm;
    private float iceAcceleration = 0.2f;
    private Vector3 moveVec = Vector3.zero;

    public MeshRenderer spriteMeshRenderer;
    public Material[] playerMaterials;

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

        SwitchSpell(0);

	}
	
	// Update is called once per frame
	void Update () {
        Shoot();
    }

	void FixedUpdate() {
        Move();
    }

    private void Move() {

        float verticalInput = 0;
        float horizontalInput = 0;
        
        if (ControlMode.KeyboardAndMouse == controlMode) {

            if (Input.GetKey(KeyCode.W)) {
                verticalInput = 1.0f;
            }
            if (Input.GetKey(KeyCode.S)) {
                verticalInput = -1.0f;
            }
            if (Input.GetKey(KeyCode.A)) {
                horizontalInput = -1.0f;
            }
            if (Input.GetKey(KeyCode.D)) {
                horizontalInput = 1.0f;
            }

        } else if (ControlMode.Mobile == controlMode) {

            verticalInput = CrossPlatformInputManager.GetAxis("VerticalLeft");
            horizontalInput = CrossPlatformInputManager.GetAxis("HorizontalLeft");

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
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);

    }

    private void Shoot() {

        if (controlMode == ControlMode.KeyboardAndMouse) {

            // switching weapons
            if (Input.GetKeyDown(KeyCode.E)) {
                SwitchSpell(currentSpell + 1);
            } else if (Input.GetKeyDown(KeyCode.Q)) {
                SwitchSpell(currentSpell - 1);
            } else if (Input.GetKeyDown(KeyCode.Alpha1)) {
                SwitchSpell(0);
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                SwitchSpell(1);
            }

            currentSpell = currentSpell % spells.Count;
            while (currentSpell < 0) {
                currentSpell += spells.Count;
            }

            // Shooting
            if (Input.GetMouseButton(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float rayDistance;
                if (groundPlane.Raycast(ray, out rayDistance)) {

                    Vector3 projectileDirection = ray.origin + ray.direction * rayDistance - this.transform.position;
                    projectileDirection.y = 0;
                    projectileDirection.z -= 0.5f;
                    projectileDirection = projectileDirection.normalized;

                    spells[currentSpell].RequestCast(projectileDirection);
                }
            }

        } else if (controlMode == ControlMode.Mobile) {

            // Switching weapon
            if (CrossPlatformInputManager.GetButton("SwitchWeapon")) {
                currentSpell++;
                currentSpell = currentSpell % spells.Count;
            }

            // Aiming
            float hShoot = CrossPlatformInputManager.GetAxis("HorizontalRight");
            float vShoot = CrossPlatformInputManager.GetAxis("VerticalRight");

            Vector3 projectileDirection = new Vector3(hShoot, 0, vShoot);
            if (projectileDirection.magnitude > 0.5f) {
                projectileDirection.Normalize();
                spells[currentSpell].RequestCast(projectileDirection);
            }
            

        }

    }

    private void SwitchSpell(int _spellNum) {

        currentSpell = _spellNum;

        currentSpell = currentSpell % spells.Count;
        while (currentSpell < 0) {
            currentSpell += spells.Count;
        }

        if (currentSpell < playerMaterials.Length) {
            spriteMeshRenderer.material = playerMaterials[currentSpell];
        } else {
            spriteMeshRenderer.material = playerMaterials[0];
        }

    }

}
