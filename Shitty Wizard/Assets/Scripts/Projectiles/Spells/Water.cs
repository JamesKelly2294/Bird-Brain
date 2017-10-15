using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Spell {

	public GameObject waterPrefab;

    public float wallWidth;
    public int numProjectiles;

    public override void Cast(Vector3 _dir) {

        float offset = wallWidth / (float)(numProjectiles - 1);
        float startRot = -wallWidth / 2.0f;

        for (int i = 0; i < numProjectiles; i++) {

            float rot = startRot + offset * i;
            Vector3 streamDir = Quaternion.Euler(0, rot, 0) * _dir;
            
            ProjectileBasic pBasic = Projectile.Create(waterPrefab, EntityType.Player, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
            pBasic.Init(streamDir, 12);
            pBasic.lifetime = 2.0f;

        }

    }
}
