using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType {
    Player,
    Enemy,
    None
}

public class Entity : MonoBehaviour {

    public EntityType type;
    public float health;

	public void Damage(float amount) {
        health -= amount;
    }

}
