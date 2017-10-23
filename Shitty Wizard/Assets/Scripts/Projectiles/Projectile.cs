using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    
    public EntityType type;

    public GameObject owner;
    public float damage;
    public float lifetime = 1000;

    [Header("Sound Options")]
    public AudioClip sound;
    public float volume = 0.5f;

    public static Projectile Create(GameObject _projectilePrefab, EntityType _type, GameObject _owner, Vector3 _initialPosition) {

        GameObject pObj = Instantiate(_projectilePrefab, _initialPosition, Quaternion.identity);
        Projectile proj = pObj.GetComponent<Projectile>();
        proj.owner = _owner;
        proj.type = _type;

        return proj;

    }

	// Use this for initialization
	void Start () {

        // Play sound if there is one
        if (sound != null) {
            AudioSource.PlayClipAtPoint(sound, Camera.main.transform.position, volume);
        }
        

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

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag == "Projectile") {
            return;
        }

        GameObject oGo = other.gameObject;

        if (oGo != owner) {

            // Damage collided if entity
            Entity entity = oGo.GetComponent<Entity>();
            if (entity != null) {

                if (entity.type != this.type && !entity.invulnerable) {

                    entity.Damage(damage);
                    Destroy(this.gameObject);

                }

            } else {

                Destroy(this.gameObject);

            }

        }

    }

}
