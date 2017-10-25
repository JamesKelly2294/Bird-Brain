using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileIce : Projectile {

    public GameObject sprite;

    private Vector3 direction;
    private float speed;

    protected override void OnMove() {

        this.transform.position = this.transform.position + direction * speed * Time.deltaTime;

        IceTileManager itm = owner.GetComponent<IceTileManager>();
        if (itm != null) {
            itm.AddTile(new Vector2(this.transform.position.x, this.transform.position.z));
        }

    }

    public void Init(Vector3 direction, float speed) {
        this.direction = direction;
        this.speed = speed;
        this.lifetime = 10.0f;
        this.sprite.transform.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + 90);
    }

}
