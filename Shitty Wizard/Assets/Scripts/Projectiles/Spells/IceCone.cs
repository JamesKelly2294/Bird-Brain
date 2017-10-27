using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCone : Spell {

    public GameObject icePrefab;

    public float coneWidth;
    public int numStreams;

	private bool isPlayingCastSound = false;

    public override void Cast(Vector3 _dir) {

        float offset = coneWidth / (float)(numStreams - 1);
        float startRot = -coneWidth / 2.0f;


		// Play sound if there is one
		if (!isPlayingCastSound) {
			isPlayingCastSound = true;
			audioSource.volume = volume;
			audioSource.clip = castSound;
			audioSource.Play();
			Invoke ("StopPlayingCastSound", castSound.length + 0.05f);
		}

		Vector3 projOffset = owner.transform.position + Vector3.up * 0.5f;
        for (int i = 0; i < numStreams; i++) {

            float rot = startRot + offset * i;
            Vector3 streamDir = Quaternion.Euler(0, rot, 0) * _dir;
			Vector3 projStart = projOffset + streamDir;
            
			ProjectileIce pIce = Projectile.Create(icePrefab, EntityType.Player, owner, projStart) as ProjectileIce;
            pIce.Init(streamDir, 12);
            pIce.lifetime = 0.2f;

        }

    }

	void StopPlayingCastSound() {
		isPlayingCastSound = false;
	}

}
