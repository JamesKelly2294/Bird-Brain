using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherBlast : Spell {

    public GameObject featherPrefab;

    public override void Cast(Vector3 _dir) {
        EntityType type = owner.GetComponent<Entity>().type;
        for (int i = 0; i < 360; i += 4) {
            if ((i / 30) % 2 == 0) {
                Vector3 dir = Quaternion.Euler(0, i, 0) * _dir;
                ProjectileBasic pBasic = Projectile.Create(featherPrefab, type, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
                pBasic.Init(dir, 5);
            }
        }

    }

}
