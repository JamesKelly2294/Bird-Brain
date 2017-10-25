using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShittyWizard.Controller.Game;

public enum EntityType {
    Player,
    Enemy,
    None
}

public class Entity : MonoBehaviour {

    [Header("Basic Settings")]
    public EntityType type;

    [Header("State Settings")]
    public bool inControl = true;
    public float health;
    public float maxHealth = 0;
    public bool invulnerable = false;

    [Header("Bounce Settings")]
    public float bounceSpeedMultiplier = 3;
    public float bounceHeight = 0.3f;

    public GameObject sprite;

    private float blinkRate = 0.2f;
    private bool visible = true;
    private bool flashing = false;

    private Rigidbody rb;
    private float bounceCycle = 0;
    private Vector3 spriteStartPos;

	protected GUIController _ui;

    private IceTileManager itm;
    private bool collidingWithWall = false;

    public void Start() {
        rb = GetComponent<Rigidbody>();
        spriteStartPos = sprite.transform.localPosition;
        maxHealth = health;
		_ui = GameObject.Find ("GUIController").GetComponent<GUIController> ();
        itm = GetComponent<IceTileManager>();
        OnStart();

    }
    protected virtual void OnStart() { }

    public void Update() {

        OnUpdate();

        float bounceSpeed = rb.velocity.magnitude * bounceSpeedMultiplier;
        if (bounceSpeed == 0) {
            bounceCycle = 0;
        }
        bounceCycle += bounceSpeed * Time.deltaTime;
        sprite.transform.localPosition = spriteStartPos + Vector3.up * bounceHeight * Mathf.Abs(Mathf.Sin(bounceCycle));

    }
    protected virtual void OnUpdate() { }

    public void Move(Vector3 speed) {
        if (!inControl) return;
        MoveOverride(speed);
    }

    private void MoveOverride(Vector3 speed) {
        rb.velocity = speed;
    }

	public void Damage(float _amount) {

        if (invulnerable) return;

        health -= _amount;

        if (health <= 0) {
            OnDeath();
        }

        OnDamage();

    }

    protected virtual void OnDeath() { }
    protected virtual void OnDamage() { }

    protected void MakeInvulnerable(float _length) {
        StartCoroutine(InvulnerableCR(_length));
    }

    private IEnumerator InvulnerableCR(float _length) {

        invulnerable = true;
        SetVisible(false);

        float blinkTimer = 0;
        while (_length > 0 || !visible) {

            _length -= Time.deltaTime;

            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkRate) {
                blinkTimer -= blinkRate;
                ToggleVisibile();
                if (visible && _length < blinkRate) {
                    _length = 0;
                }
            }

            yield return null;

        }

        SetVisible(true);
        invulnerable = false;

    }

    private void ToggleVisibile() {
        SetVisible(!visible);
    }

    private void SetVisible(bool _visible) {
        visible = _visible;
        MeshRenderer meshRenderer = sprite.GetComponent<MeshRenderer>();
        meshRenderer.enabled = _visible;
    }

    protected void Flash(float _length) {
        if (!flashing) {
            StartCoroutine(FlashCR(_length));
        }
    }

    private IEnumerator FlashCR(float _length) {

        flashing = true;

        Renderer r = GetComponentInChildren<Renderer>();
        r.material.SetColor("_EmissionColor", Color.white);

        while (_length > 0) {
            _length -= Time.deltaTime;
            yield return null;
        }

        r.material.SetColor("_EmissionColor", Color.black);

        flashing = false;

    }

    public void Knockback(Vector3 _dir, float _distance) {
        StartCoroutine(KnockbackCR(_dir, _distance));
    }

    private IEnumerator KnockbackCR(Vector3 _dir, float _distance) {

        inControl = false;
        Vector3 startPos = this.transform.position;
        float baseSpeed = 5;

        collidingWithWall = false;

        float breakTime = 0.5f;
        float breakTimer = 0;
        float distanceTraveled = Vector3.Distance(startPos, this.transform.position);
        while (_distance - distanceTraveled > 0.5f && !inControl && breakTimer < breakTime) {

            if (collidingWithWall) {
                _distance /= 2;
                collidingWithWall = false;
            }

            if (itm.onIce) {
                breakTime = 1f;
                _distance += Vector3.Dot(rb.velocity, _dir) * Time.deltaTime;
            } else {
                breakTime = 0.5f;
            }

            breakTimer += Time.deltaTime;
            distanceTraveled = Vector3.Distance(startPos, this.transform.position);
            this.MoveOverride(_dir * (_distance - distanceTraveled) * baseSpeed);

            yield return null;

        }

        inControl = true;

    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall")) {
            collidingWithWall = true;
        }
    }

}
