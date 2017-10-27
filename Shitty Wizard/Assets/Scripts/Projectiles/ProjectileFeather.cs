using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFeather : Projectile {

	public GameObject sprite;

	private Vector3 direction;
	private float speed;

	int timer;

	void Start() {
		timer = 0;
	}

	protected override void OnMove() {
		timer++;

		if (timer == 20) {
			speed = speed / 6;
		} 
		else if(timer == 40){
			speed = speed * 6;
			timer = 0;
		}

		this.transform.position = this.transform.position + direction * speed * Time.deltaTime;
	}

	public void Init(Vector3 direction, float speed) {
		this.direction = direction;
		this.speed = speed;
		this.lifetime = 10.0f;
		this.sprite.transform.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + 90);
	}
}
