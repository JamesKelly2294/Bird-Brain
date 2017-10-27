using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drumstick : MonoBehaviour {

    private EntityPlayer playerScript;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerScript = collision.gameObject.GetComponent<EntityPlayer>();
            playerScript.currentHealth += 10f;
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
}
