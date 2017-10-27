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
		PlayMusic ();
    }

	void PlayMusic() {
		int i = Random.Range (0, music.Length);
		musicSource.Stop ();

		musicSource.clip = music [i];
		musicSource.Play ();

		Invoke ("PlayMusic", musicSource.clip.length + 0.5f);
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = new Vector3 (
			target.transform.position.x, 
			transform.position.y,
			target.transform.position.z) + offset;

    }
}
