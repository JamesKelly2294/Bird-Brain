using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : MonoBehaviour {
    
    protected GameObject owner;

    [SerializeField]
    protected float reloadTime;
    protected float lastCastTime = -100000;

    [SerializeField]
	protected AudioClip castSound;
	//public AudioSource audioSource;

	[SerializeField]
    protected float volume = 0.5f;

    private void Start() {
        this.owner = this.transform.parent.parent.gameObject;
		//audioSource = GetComponent<AudioSource> ();
    }

    public void RequestCast(Vector3 _dir) {
        float currTime = Time.time;

        if (currTime - lastCastTime >= reloadTime) {
            lastCastTime = currTime;
            Cast(_dir);
        }
    }

    public abstract void Cast(Vector3 _dir);

}
