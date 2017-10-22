using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : Spell {

	public GameObject missilePrefab;

	public override void Cast(Vector3 _dir) {
		ProjectileMissile pBasic = Projectile.Create(missilePrefab, EntityType.Player, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileMissile;
		pBasic.Init(_dir, 14);
	}
}
