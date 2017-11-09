using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBasic : Projectile {

    public GameObject sprite;

    private Vector3 direction;
    private float speed;

    protected override void OnMove() {
        this.transform.position = this.transform.position + direction * speed * Time.deltaTime;
    }

    public void Init(Vector3 direction, float speed) {
        this.direction = direction;
        this.speed = speed;
        this.lifetime = 10.0f;
        this.transform.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + 90);
    }

	protected override void OnTriggerEnter(Collider other) {
		GameObject oGo = other.gameObject;

		if (oGo != owner && (oGo.transform.parent == null || oGo.transform.parent.gameObject != owner)) {

			// Damage collided if entity and play hit sound
			Entity entity = oGo.GetComponent<Entity>();
			if (entity != null) {

				if (entity.type != this.type && !entity.invulnerable) {
					if (knockbackMagnitude > 0.001f && entity.knockedBackOnHit) {
						entity.Knockback ((gameObject.transform.position - (entity.transform.position - direction)).normalized, knockbackMagnitude);
					}
				}

			} 
		}


		base.OnTriggerEnter (other);
	}
}
