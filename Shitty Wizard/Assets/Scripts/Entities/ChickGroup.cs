using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickGroup : MonoBehaviour {

    public GameObject chickPrefab;
    public int chickBaseCount;
    public int chickCountVariance;

    private void Start() {

		GameObject player = GameObject.FindGameObjectWithTag("Player");
		GameObject entities = GameObject.FindGameObjectWithTag("Entities");

        int numChicks = chickBaseCount + Random.Range(0, chickCountVariance);
        for (int i = 0; i < numChicks; i++) {
            GameObject chick = Instantiate(chickPrefab, this.transform.position, Quaternion.identity);
			chick.transform.parent = transform;
            EnemyController ec = chick.GetComponent<EnemyController>();
            ec.target = player.transform;
        }

    }

}
