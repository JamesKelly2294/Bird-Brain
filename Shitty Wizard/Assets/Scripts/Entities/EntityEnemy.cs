using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEnemy : Entity {

    public int expToGive;

    protected override void OnDeath() {
        base.OnDeath();
        Destroy(this.gameObject);
    }

    protected override void OnDamage() {
        base.OnDamage();
        Flash(0.05f);
    }
}
