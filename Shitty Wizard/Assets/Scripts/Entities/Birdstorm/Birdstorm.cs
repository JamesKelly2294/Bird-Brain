using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Birdstorm : MonoBehaviour {

    public GameObject target;
    public GameObject birdPrefab;

    private float height = 5;

    private float birdRate = 1;
    private float birdTimer = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        birdTimer += Time.deltaTime;
        if (birdTimer > birdRate) {
            birdTimer -= birdRate;
            CreateBird();
        }

	}

    private void LateUpdate() {
        this.transform.position = target.transform.position;
    }

    private void CreateBird() {

        GameObject bird = Instantiate(birdPrefab, Vector3.zero, Quaternion.identity);
        bird.transform.parent = this.transform;
        bird.GetComponent<FlyingChicken>().Init(height, target);

    }

}
