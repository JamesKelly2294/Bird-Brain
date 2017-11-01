using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {

	public float HealAmount = 10.0f;
	public AudioClip pickupSound;

	private GameObject sprite;
	private float randomAnimationOffset = 0.0f; 

	// Use this for initialization
	void Start () {
		sprite = this.transform.Find ("Sprite").gameObject;
		randomAnimationOffset = Random.Range (0.0f, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		sprite.transform.position = new Vector3 (sprite.transform.position.x, (Mathf.Sin (Time.time + randomAnimationOffset) / 5.0f) + 0.5f, sprite.transform.position.z);
	}

	public void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Player") {
			Entity player = other.gameObject.GetComponent<Entity>();

			if (player.health == player.maxHealth) {
			} else {
				AudioSource.PlayClipAtPoint (pickupSound, other.gameObject.transform.position);
				player.Heal (HealAmount);
				Destroy (gameObject);
			}
		}
	}
}
