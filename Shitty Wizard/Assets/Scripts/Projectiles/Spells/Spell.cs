using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell {

    protected GameObject owner;

    protected float reloadTime;
    protected float lastCastTime;

    public Spell(GameObject _owner) {
        owner = _owner;
        lastCastTime = -100000;
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
