using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    
    public EntityType type;

    public GameObject owner;
    public float damage;
    public float lifetime = 1000;
	public float knockbackMagnitude = 0.0f;

    [Header("Sound Options")]
	public AudioClip hitSound;
    public float volume = 1.0f;

	[Header("Particles")]
	public GameObject onHitParticle;

    private static Transform projectileHolder;

    public static Projectile Create(GameObject _projectilePrefab, EntityType _type, GameObject _owner, Vector3 _initialPosition) {

        if (projectileHolder == null) {
            projectileHolder = GameObject.Find("ProjectileHolder").transform;
        }

		GameObject pObj = (GameObject)Instantiate(_projectilePrefab, _initialPosition, Quaternion.identity);
        if (projectileHolder != null) pObj.transform.parent = projectileHolder;
        Projectile proj = pObj.GetComponent<Projectile>();
        proj.owner = _owner;
        proj.type = _type;

        return proj;

    }

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        OnMove();

        lifetime -= Time.deltaTime;
        if (lifetime <= 0) {
            OnEndOfLife();
            Destroy(this.gameObject);
        }

	}
    
    protected virtual void OnMove() { }
    protected virtual void OnEndOfLife() { }

    protected virtual void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag == "Projectile") {
            return;
        }

		if (onHitParticle != null && other.gameObject.layer == LayerMask.NameToLayer ("Wall")) {
			GameObject particle = Instantiate (onHitParticle);
			particle.transform.position = transform.position;
			particle.transform.parent = transform.parent;

			Destroy (particle, 0.3f);
		}

        GameObject oGo = other.gameObject;

        if (oGo != owner && (oGo.transform.parent == null || oGo.transform.parent.gameObject != owner)) {

            // Damage collided if entity and play hit sound
            Entity entity = oGo.GetComponent<Entity>();
            if (entity != null) {

                if (entity.type != this.type && !entity.invulnerable) {
					AudioSource.PlayClipAtPoint (hitSound, transform.position, volume);
                    entity.Damage(damage);
                    Destroy(this.gameObject);
                }

            } else {
                
                Destroy(this.gameObject);

            }

        }

    }

}
