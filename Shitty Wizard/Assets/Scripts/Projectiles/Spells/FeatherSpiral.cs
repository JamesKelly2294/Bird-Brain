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

        float offset = UnityEngine.Random.Range(0f, 120f);

        float angle = 0;
        EntityType type = owner.GetComponent<Entity>().type;
        while (angle < 360) {

            angle += 5;

            Vector3 dir = Quaternion.Euler(0, angle + offset, 0) * _dir;
            ProjectileBasic pBasic = Projectile.Create(featherPrefab, type, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
            pBasic.Init(dir, 5);

            Vector3 dir2 = Quaternion.Euler(0, angle + offset + 120, 0) * _dir;
            pBasic = Projectile.Create(featherPrefab, type, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
            pBasic.Init(dir2, 5);

            Vector3 dir3 = Quaternion.Euler(0, angle + offset + 240, 0) * _dir;
            pBasic = Projectile.Create(featherPrefab, type, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
            pBasic.Init(dir3, 5);

            yield return new WaitForSeconds(0.07f);

        }

    }

}
