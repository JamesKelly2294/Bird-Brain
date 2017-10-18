using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType {
    Player,
    Enemy,
    None
}

public class Entity : MonoBehaviour {

    public bool inControl;

    public EntityType type;
    public float health;
    public GameObject sprite;

    private float blinkRate = 0.2f;
    private bool visible = true;
    public bool invulnerable = false;
    private bool flashing = false;

    public void Start() {
        OnStart();
    }
    protected virtual void OnStart() { }

    public void Update() {
        OnUpdate();
    }
    protected virtual void OnUpdate() { }

    public void Move(Vector3 speed) {
        if (!inControl) return;
        MoveOverride(speed);
    }

    private void MoveOverride(Vector3 speed) {
        GetComponent<Rigidbody>().velocity = speed;
    }

	public void Damage(float _amount) {

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

    public void Knockback(Vector3 force) {
        StartCoroutine(KnockbackCR(force));
    }

    private IEnumerator KnockbackCR(Vector3 force) {

        inControl = false;

        while (force.magnitude > 0.1f) {

            this.MoveOverride(force);
            force *= 0.9f;

            yield return null;

        }

        inControl = true;

    }

}
