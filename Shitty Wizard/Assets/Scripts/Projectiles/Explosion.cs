using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class Explosion : MonoBehaviour {

    private Animator anim;
    private Collider col;

    private List<GameObject> objs;

	void Start () {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider>();
        objs = new List<GameObject>();
	}

    void DamageArea() {

    }
	
	void Update () {

        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            Destroy(this.gameObject);
        }

	}

    private void OnTriggerEnter(Collider other) {

        GameObject go = other.gameObject;
        if (!objs.Contains(go)) {

            objs.Add(go);

            Entity entity = go.GetComponent<Entity>();
            if (entity != null) {
                entity.Damage(30);
            }

        }

    }

}
