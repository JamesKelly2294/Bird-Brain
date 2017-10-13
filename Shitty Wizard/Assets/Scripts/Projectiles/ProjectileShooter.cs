using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour {

    public GameObject projectilePrefab;

    private float timer;

	// Use this for initialization
	void Start () {
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;
        if (timer >= 1) {
            timer -= 1;
            ProjectileBasic pBasic = Projectile.Create(projectilePrefab, EntityType.Enemy, this.gameObject, this.transform.position) as ProjectileBasic;
            pBasic.Init(new Vector3(1, 0, 0), 3);
        }

	}
}
