using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEnemy : Entity {

    [Header("Enemy Settings")]
    public float expToGive = 10;
    public int damageOnContact;
	public AudioClip deathSound;

	private UI _ui;

	protected override void OnStart ()
	{
		base.OnStart ();
		_ui = GameObject.Find ("HUD").GetComponent<UI> ();
	}

    protected override void OnUpdate() {
        base.OnUpdate();
    }

    protected override void OnDeath() {
        base.OnDeath();
		AudioSource.PlayClipAtPoint (deathSound, transform.position);
		_ui.GiveEXP (expToGive);
        Destroy(this.gameObject);
    }

    protected override void OnDamage() {
        base.OnDamage();
        Flash(0.05f);
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            Entity player = other.gameObject.GetComponent<Entity>();
            player.Damage(damageOnContact);
        }
    }

}
