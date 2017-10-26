using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Spell {

    public GameObject fireballPrefab;

    public override void Cast(Vector3 _dir) {


		// Play sound if there is one
		//if (castSound != null) {
			//audioSource.clip = castSound;
			//audioSource.Play();
			AudioSource.PlayClipAtPoint(castSound, Camera.main.transform.position, volume);
		//}

        ProjectileBasic pBasic = Projectile.Create(fireballPrefab, EntityType.Player, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileBasic;
        pBasic.Init(_dir, 14);
        
        owner.GetComponent<Entity>().Knockback(-_dir, 3);

    }
}
