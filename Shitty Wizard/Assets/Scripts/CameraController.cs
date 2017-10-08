﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform target;
	public Vector3 offset = new Vector3 (0.0f, 0.0f, 18.0f);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (
			target.transform.position.x, 
			transform.position.y,
			target.transform.position.z) + offset;
	}
}