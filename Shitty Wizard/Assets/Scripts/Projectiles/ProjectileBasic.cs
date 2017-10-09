using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBasic : Projectile {

    private Vector3 direction;
    private float speed;

    protected override void Move() {
        this.transform.position = this.transform.position + direction * speed * Time.deltaTime;
    }

    public void Init(Vector3 direction, float speed) {
        this.direction = direction;
        this.speed = speed;
        this.lifetime = 10.0f;
    }

}
