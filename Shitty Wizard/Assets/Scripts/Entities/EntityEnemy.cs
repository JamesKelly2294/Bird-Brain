﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEnemy : Entity {

    [Header("Enemy Settings")]
    public int expToGive;
    public int damageOnContact;
	public AudioClip deathSound;

    protected override void OnUpdate() {
        base.OnUpdate();
    }

    protected override void OnDeath() {
        base.OnDeath();
		AudioSource.PlayClipAtPoint (deathSound, transform.position);
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
