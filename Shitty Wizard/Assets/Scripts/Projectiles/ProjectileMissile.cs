using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMissile : Projectile {

	public GameObject sprite;

	private Vector3 direction;
	private float speed;

	public float frequency = 10.0f;  // Speed of sine movement
	public float magnitude = 0.1f;   // Size of sine movement
	private Vector3 axis;            // Sin wave axis

	protected override void OnMove() {
		axis = this.transform.forward;
		this.transform.position += direction * Time.deltaTime * speed;
		this.transform.position = this.transform.position + axis * Mathf.Sin (Time.time * frequency) * magnitude;
	}

	public void Init(Vector3 direction, float speed) {
		this.direction = direction;
		this.speed = speed;
		this.lifetime = 10.0f;
		this.transform.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + 90);
	}
}
