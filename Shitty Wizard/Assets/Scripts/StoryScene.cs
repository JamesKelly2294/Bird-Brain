using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine ("LoadNext");
	}

	IEnumerator LoadNext() {
		yield return new WaitForSeconds (45);
		SceneManager.LoadScene ("LevelScene");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space")) {
			SceneManager.LoadScene ("LevelScene");
		}
	}
}
