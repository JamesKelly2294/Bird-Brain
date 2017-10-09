using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType {
    Player,
    Enemy,
    None
}

public class Entity : MonoBehaviour {

    public EntityType type;
    public float health;
    public GameObject sprite;

    private float blinkRate = 0.2f;
    private bool visible = true;
    public bool invulnerable = false;

    private bool flashing = false;

	public void Damage(float _amount) {

        health -= _amount;

        if (health <= 0) {
            if (type == EntityType.Enemy) {
                Destroy(this.gameObject);
            }
        }

        if (type == EntityType.Player) {
            MakeInvulnerable(2);
        } else if (type == EntityType.Enemy) {
            Flash(0.05f);
        }

    }

    private void MakeInvulnerable(float _length) {
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

    public void Flash(float _length) {
        if (!flashing) {
            StartCoroutine(FlashCR(_length));
        }
    }

    private IEnumerator FlashCR(float _length) {

        flashing = true;

        Renderer r = GetComponentInChildren<Renderer>();
        Texture tex = r.material.GetTexture("_MainTex");
        r.material.color = Color.black;

        while (_length > 0) {
            _length -= Time.deltaTime;
            yield return null;
        }

        r.material.color = Color.white;

        flashing = false;

    }

}
