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
	public int expValue = 100;

    private float blinkRate = 0.2f;
    private bool visible = true;
    public bool invulnerable = false;
	public int expTotal = 0;
	public int levelUpAt = 1000;
	public int level = 1;

	public GameObject guiManager;
	GUIText GUITextEXP;

    private bool flashing = false;

	public void Start(){
		guiManager = GameObject.Find("GUI_EXP");
		GUITextEXP = guiManager.GetComponent<GUIText>();
	}

	public void Damage(float _amount) {

        health -= _amount;

        if (health <= 0) {
            if (type == EntityType.Enemy) {
				giveExp (expValue);
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
        r.material.SetColor("_EmissionColor", Color.white);

        while (_length > 0) {
            _length -= Time.deltaTime;
            yield return null;
        }

        r.material.SetColor("_EmissionColor", Color.black);

        flashing = false;

    }

	private void giveExp(int _added) {
		if (_added < 0) {
			Debug.Log ("Can't remove exp!");
			return;
		}

		expTotal += _added;
		if (expTotal >= levelUpAt) {
			levelUpAt += 1000;
			level += 1;
			Debug.Log (expTotal + "/" + levelUpAt + " | " + level);
			GUITextEXP.text = expTotal + "/" + levelUpAt + " | " + level;
		}
	}

}
