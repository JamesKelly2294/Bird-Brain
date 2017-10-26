using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Spell {

	public GameObject laserPrefab;

	public override void Cast(Vector3 _dir) {

		_dir.y = 0;
		// Play sound if there is one
		//if (castSound != null) {
		//audioSource.clip = castSound;
		//audioSource.Play();
		AudioSource.PlayClipAtPoint(castSound, Camera.main.transform.position, volume);
		//}

		ProjectileBasic pBasic = Projectile.Create(laserPrefab, EntityType.Player, owner, owner.transform.position + Vector3.up * 3.195f) as ProjectileBasic;
		pBasic.Init(_dir, 14);

	}
}
