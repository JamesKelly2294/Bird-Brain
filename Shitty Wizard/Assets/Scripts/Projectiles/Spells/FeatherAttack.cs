using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherAttack : Spell {

	public GameObject featherPrefab;

	public float circle;
	public int numFeathers;

	public override void Cast(Vector3 _dir) {

		float offset = circle / (float)(numFeathers - 1);
		float startRot = -circle / 2.0f;


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

		for (int i = 0; i < numFeathers; i++) {

			float rot = startRot + offset * i;
			Vector3 streamDir = Quaternion.Euler(0, rot, 0) * _dir;

			ProjectileFeather pFthr = Projectile.Create(featherPrefab, EntityType.Player, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectileFeather;
			pFthr.Init(streamDir, 6);

		}

	}
}
