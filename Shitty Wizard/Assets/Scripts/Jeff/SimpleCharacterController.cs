using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleCharacterController : MonoBehaviour {

    public bool useJoystick;
    public float moveSpeed;
    public GameObject sprite;

    private CharacterController cc;

	// Use this for initialization
	void Start () {

        cc = this.GetComponent<CharacterController>();

    }
	
	// Update is called once per frame
	void Update () {

        float inH = Input.GetAxis("Horizontal");
        float inV = Input.GetAxis("Vertical");

        if (useJoystick) {

        } else {
            
        }

        Vector3 moveVec = new Vector3(inH, 0, inV).normalized * moveSpeed;
        cc.SimpleMove(moveVec);

	}
}
