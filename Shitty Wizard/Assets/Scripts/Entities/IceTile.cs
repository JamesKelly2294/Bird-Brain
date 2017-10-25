using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTile : MonoBehaviour {

    public GameObject sprite;
    public float lifespan;

    private IceTileManager itm;
    private int hash;

    // Update is called once per frame
    void Update () {

        lifespan -= Time.deltaTime;
        if (lifespan < 0) {
            lifespan = 99999999;
            StartCoroutine(Melt());
        }

	}

    public void Init(IceTileManager _itm, int _hash) {
        itm = _itm;
        hash = _hash;
    }

    private IEnumerator Melt() {

        float shake = 0.08f;
        float meltTime = 1.0f;
        while (meltTime > 0) {
            meltTime -= Time.deltaTime;
            sprite.transform.localPosition = new Vector3(Random.Range(-shake, shake), 0, Random.Range(-shake, shake));
            yield return null;
        }

        itm.RemoveTile(hash);

    }

}
