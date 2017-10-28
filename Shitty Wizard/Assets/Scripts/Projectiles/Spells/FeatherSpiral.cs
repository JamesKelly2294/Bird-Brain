using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherSpiral : Spell {

    public GameObject featherPrefab;

    public override void Cast(Vector3 _dir) {

        StartCoroutine(CastSpiral(_dir));

    }

    private IEnumerator CastSpiral(Vector3 _dir) {

        float angle = 0;
        EntityType type = owner.GetComponent<Entity>().type;
        while (angle < 360) {

            angle += 5;

            Vector3 dir = Quaternion.Euler(0, angle, 0) * _dir;

            ProjectileBasic pBasic = Projectile.Create(featherPrefab, type, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
            pBasic.Init(dir, 5);

            yield return new WaitForSeconds(0.05f);

        }

    }

}
