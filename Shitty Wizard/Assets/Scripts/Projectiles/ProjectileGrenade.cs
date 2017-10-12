using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGrenade : Projectile {

    public GameObject explosionPrefab;

    Vector3 dir;

    protected override void OnEndOfLife() {

        Vector3 pos = this.transform.position;
        pos.y = 0.05f;

        Quaternion rot = Quaternion.Euler(90, 0, 0);

        Instantiate(explosionPrefab, pos, rot);

    }

    public void Init(Vector2 _dir, float _height) {

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(5, 6, 0);

    }

}
