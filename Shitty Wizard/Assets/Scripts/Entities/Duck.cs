using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : Entity {

    public GameObject grenadePrefab;

    private float shootRate = 5.0f;
    private float shootTimer = 0;
	
	// Update is called once per frame
	void Update () {

        shootTimer += Time.deltaTime;
        if (shootTimer > shootRate) {
            shootTimer = 0;
            LaunchGrenade();
        }

	}

    private void LaunchGrenade() {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        ProjectileGrenade grenade = ProjectileManager.CreateProjectile(grenadePrefab, EntityType.Enemy, this.gameObject, this.transform.position + new Vector3(0, 1f, 0)) as ProjectileGrenade;
        grenade.lifetime = 4.0f;
        grenade.Init(Vector2.zero, 0);


    }

}
