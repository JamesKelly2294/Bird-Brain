using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell {

    public GameObject fireballPrefab;

    public override void Cast(Vector3 _dir) {

        ProjectileBasic pBasic = Projectile.Create(fireballPrefab, EntityType.Player, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
        pBasic.Init(_dir, 14);

    }

}
