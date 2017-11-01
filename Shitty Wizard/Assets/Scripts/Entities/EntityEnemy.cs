using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEnemy : Entity {

    [Header("Enemy Settings")]
    public float expToGive = 10;
    public int damageOnContact;
	public AudioClip deathSound;
	public GameObject[] loot;
	[Range(0.0f, 1.0f)]
	public float dropRate;

	protected override void OnStart ()
	{
		base.OnStart ();
	}

    protected override void OnUpdate() {
        base.OnUpdate();
    }

    protected override void OnDeath() {
		bool wasAlive = alive;

        base.OnDeath();

		if (wasAlive) {
			AudioSource.PlayClipAtPoint (deathSound, transform.position);
			_ui.GiveEXP (expToGive);

			if (loot.Length > 0 && Random.Range(0.0f, 1.0f) <= dropRate) {
				GameObject instLoot = Instantiate(loot [Random.Range (0, loot.Length)]);
				instLoot.transform.position = this.gameObject.transform.position;
			}

			Destroy(this.gameObject);
		}

    }

    protected override void OnDamage() {
        base.OnDamage();
        MakeFlash(0.05f);
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            Entity player = other.gameObject.GetComponent<Entity>();
            player.Damage(damageOnContact);
        }
    }

}
