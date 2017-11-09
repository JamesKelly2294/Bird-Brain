using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryScene : MonoBehaviour {

    TransitionController transitionController;

    // Use this for initialization
    void Start () {
        StartCoroutine("LoadNext");
	}

	IEnumerator LoadNext() {
		yield return new WaitForSeconds(45);
		TransitionController.Instance().LoadScene("LevelScene");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space")) {
            TransitionController.Instance().LoadScene("LevelScene");
		}
	}

}
