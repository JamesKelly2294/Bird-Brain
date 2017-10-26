using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShittyWizard.Controller.Game;

public class Staircase : MonoBehaviour
{

	public GameObject player;
	public WorldController worldController;

	private void OnTriggerEnter (Collider other)
	{
		GameObject oGo = other.gameObject;

		if (oGo != player) {
			return;
		}

		worldController.AdvanceLevel ();

	}
}
