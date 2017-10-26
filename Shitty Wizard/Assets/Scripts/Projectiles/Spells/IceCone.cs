using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCone : Spell {

    public GameObject icePrefab;

    public float coneWidth;
    public int numStreams;

    public override void Cast(Vector3 _dir) {

        float offset = coneWidth / (float)(numStreams - 1);
        float startRot = -coneWidth / 2.0f;


		// Play sound if there is one
		//if (castSound != null) {

			// If the sound is not already being played, play it
			//if(!audioSource.isPlaying)
			//{
			//	//audioSource.clip = castSound;
			//	audioSource.Play();
			//}
			AudioSource.PlayClipAtPoint(castSound, Camera.main.transform.position, volume);
		//}

        for (int i = 0; i < numStreams; i++) {

            float rot = startRot + offset * i;
            Vector3 streamDir = Quaternion.Euler(0, rot, 0) * _dir;
            
            ProjectileIce pIce = Projectile.Create(icePrefab, EntityType.Player, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileIce;
            pIce.Init(streamDir, 12);
            pIce.lifetime = 0.2f;

        }

    }

}
