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

	public void Damage(float _amount) {
        health -= _amount;
        if (type == EntityType.Player) {
            MakeInvulnerable(2);
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

}
