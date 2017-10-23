using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform target;
	public Vector3 offset = new Vector3 (0.0f, 0.0f, 18.0f);

	public AudioClip[] music;
	public AudioSource musicSource;

    private Camera cam;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();

        cam.ResetProjectionMatrix();
        var m = cam.projectionMatrix;

        m.m11 *= 1.5f;
        cam.projectionMatrix = m;

		musicSource.volume = 0.25f;
		musicSource.loop = true;
		musicSource.clip = music [0];
		musicSource.PlayDelayed (1);
    }
	
	// Update is called once per frame
	void Update () {

		transform.position = new Vector3 (
			target.transform.position.x, 
			transform.position.y,
			target.transform.position.z) + offset;

    }
}
