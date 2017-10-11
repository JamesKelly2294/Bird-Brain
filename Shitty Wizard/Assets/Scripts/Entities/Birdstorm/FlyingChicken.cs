using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingChicken : MonoBehaviour {

    private Vector3 dir;
    private float speed = 7;
    private float lifespan = 5;

    private Vector3 offset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        this.transform.position += dir * speed * Time.deltaTime;

        lifespan -= Time.deltaTime;
        if (lifespan <= 0) {
            Destroy(this.gameObject);
        }

	}

    public void Init(float _height, GameObject _target) {

        float angle = Random.Range(0.0f, 360.0f);
        Vector3 startPos = _target.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * Vector3.one * 10 - Vector3.forward * _height;
        startPos.y = _height;

        this.transform.position = startPos;

        BoxCollider col = this.GetComponent<BoxCollider>();
        offset = new Vector3(0, 0.5f - _height, _height);
        col.center = offset;

        dir = _target.transform.position - (this.transform.position + offset);
        dir.y = 0;
        dir = dir.normalized;

    }

}
