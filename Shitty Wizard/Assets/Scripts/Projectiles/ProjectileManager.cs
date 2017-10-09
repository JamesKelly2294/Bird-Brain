using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour {

    public static Projectile CreateProjectile(GameObject projectilePrefab, EntityType type, GameObject owner, Vector3 initialPosition) {

        GameObject pObj = Instantiate(projectilePrefab, initialPosition, Quaternion.identity);
        Projectile proj = pObj.GetComponent<Projectile>();
        proj.owner = owner;
        proj.type = type;

        return proj;

    }
	
}
