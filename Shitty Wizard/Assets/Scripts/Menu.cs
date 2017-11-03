using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public void LoadScene(string _sceneName) {
        TransitionController.Instance().LoadScene(_sceneName);
	}

	public void QuitGame() {
		Application.Quit ();
	}

}
