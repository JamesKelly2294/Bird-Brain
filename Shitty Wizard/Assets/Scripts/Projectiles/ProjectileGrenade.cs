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
        
        _dir = _dir.normalized * 5;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(_dir.x, 6, _dir.y);

    }

    private void OnCollisionEnter(Collision collision) {

        GameObject go = collision.gameObject;
        if (go.tag == "Player") {
            OnEndOfLife();
        }

    }

}
