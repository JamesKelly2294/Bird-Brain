using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : EntityEnemy {

    [Header("Duck Settings")]
    public GameObject grenadePrefab;
    public float shootRate;

    private float shootTimer = 0;

    protected override void OnStart() {
        base.OnStart();
        shootTimer = Random.Range(0.0f, shootRate);
    }

    protected override void OnUpdate() {

        base.OnUpdate();

        shootTimer += Time.deltaTime;
        if (shootTimer > shootRate) {
            shootTimer = 0;
            LaunchGrenade();
        }

    }

    private void LaunchGrenade() {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 dir3 = player.transform.position - this.transform.position;
        Vector2 dir = new Vector2(dir3.x, dir3.z);
        dir = dir.normalized;

        ProjectileGrenade grenade = Projectile.Create(grenadePrefab, EntityType.Enemy, this.gameObject, this.transform.position + new Vector3(0, 1f, 0)) as ProjectileGrenade;
        grenade.lifetime = 4.0f;
        grenade.Init(dir, 0);

    }

}
