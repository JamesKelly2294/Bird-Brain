using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    
    public EntityType type;

    public GameObject owner;
    public float damage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Move();

	}

    protected abstract void Move();

    private void OnTriggerEnter(Collider other) {

        GameObject oGo = other.gameObject;

        if (oGo != owner) {

            // Damage collided if entity
            Entity entity = oGo.GetComponent<Entity>();
            if (entity != null) {

                if (entity.type != this.type) {

                    entity.Damage(damage);
                    Destroy(this.gameObject);

                }

            } else {

                Destroy(this.gameObject);

            }

            

        }

    }

}
