using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherFlurry : Spell {

    public GameObject featherPrefab;
    public int numProjectiles;
    public float timeBetweenShots;
    public float spread;

    public override void Cast(Vector3 _dir) {
        StartCoroutine(Flurry(_dir));
    }

    private IEnumerator Flurry (Vector3 _dir) {

        EntityType type = this.owner.GetComponent<Entity>().type;
        float projectilesLeft = numProjectiles;
        while (projectilesLeft > 0) {

            float offset = UnityEngine.Random.Range(-spread, spread);
            Vector3 dir = Quaternion.Euler(0, offset, 0) * _dir;

            ProjectileBasic pBasic = Projectile.Create(featherPrefab, type, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
            pBasic.Init(dir, 10);

            projectilesLeft--;
            yield return new WaitForSeconds(timeBetweenShots);

        }

        yield return null;

    }

}
