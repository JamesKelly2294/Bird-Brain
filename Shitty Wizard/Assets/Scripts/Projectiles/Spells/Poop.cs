using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poop : Spell {

	public GameObject poopPrefab;

	Vector3 AddRandomness(float min, float max){
		float zRandom = Random.Range (min, max);

		Vector3 randomness = new Vector3(Mathf.Sin(2 * Mathf.PI * zRandom / 360), 0, 0);

		return randomness;
	}

	public override void Cast(Vector3 _dir) {

		_dir += AddRandomness (-45, 45);

		AudioSource.PlayClipAtPoint(castSound, Camera.main.transform.position, volume);

		ProjectilePoop pPoop = Projectile.Create(poopPrefab, EntityType.Player, owner, owner.transform.position + Vector3.up * 0.5f) as ProjectilePoop;
		pPoop.Init(_dir, 5);
	}
}
