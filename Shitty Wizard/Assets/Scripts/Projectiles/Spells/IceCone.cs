using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCone : Spell {

    GameObject projectilePrefab;

    public float coneWidth;
    public int numStreams;

	public IceCone(GameObject _owner, GameObject _projectilePrefab) : base(_owner) {
        reloadTime = 0.02f;
        projectilePrefab = _projectilePrefab;
        coneWidth = 45f;
        numStreams = 5;
    }

    public override void Cast(Vector3 _dir) {

        float offset = coneWidth / (float)(numStreams - 1);
        float startRot = -coneWidth / 2.0f;

        for (int i = 0; i < numStreams; i++) {

            float rot = startRot + offset * i;
            Vector3 streamDir = Quaternion.Euler(0, rot, 0) * _dir;
            
            ProjectileBasic pBasic = Projectile.Create(projectilePrefab, EntityType.Player, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
            pBasic.Init(streamDir, 12);
            pBasic.lifetime = 0.2f;

        }

    }

}
