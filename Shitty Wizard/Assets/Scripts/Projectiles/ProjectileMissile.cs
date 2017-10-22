using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMissile : Projectile {

	public GameObject sprite;

	private Vector3 direction;
	private float speed;

	// public float frequency = 20.0f;  // Speed of sine movement
	// public float magnitude = 0.5f;   // Size of sine movement
	// private Vector3 axis;

	// private Vector3 pos;
	// axis = transform.right;

	protected override void OnMove() {
		this.transform.position = this.transform.position + direction * speed * Time.deltaTime;

		// pos += transform.up * Time.deltaTime * MoveSpeed;
		// transform.position = pos + axis * Mathf.Sin (Time.deltaTime * frequency) * magnitude;
	}

	public void Init(Vector3 direction, float speed) {
		this.direction = direction;
		this.speed = speed;
		this.lifetime = 10.0f;
		this.sprite.transform.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + 90);
	}
}
