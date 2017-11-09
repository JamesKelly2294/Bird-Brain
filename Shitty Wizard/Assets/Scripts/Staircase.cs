using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShittyWizard.Controller.Game;

public class Staircase : MonoBehaviour
{

	public GameObject player;
	public WorldController worldController;

	private void OnTriggerEnter (Collider other) {

		GameObject oGo = other.gameObject;

		if (oGo != player) {
			return;
		}

        StartCoroutine(FadeToNextLevel(oGo));

	}

    private IEnumerator FadeToNextLevel(GameObject _playerGO) {

        Rigidbody playerRB = _playerGO.GetComponent<Rigidbody>();
        playerRB.velocity = Vector3.zero;

        PlayerController playerController = _playerGO.GetComponent<PlayerController>();
        playerController.enabled = false;

        TransitionController.Instance().FadeOut(1f);
        while (!TransitionController.Instance().IsBlack()) {
            yield return null;
        }

        worldController.AdvanceLevel();

        playerController.enabled = true;
        TransitionController.Instance().FadeIn(1f);

    }

}
