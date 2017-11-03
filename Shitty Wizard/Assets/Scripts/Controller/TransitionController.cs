using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionController : MonoBehaviour {

    public Texture2D fadeTexture;

    [Range(0f, 1f)]
    public float fadeAmount = 0;

    public static float DEFAULT_FADE_SPEED = 1f;

    private bool fading = false;

    public static TransitionController Instance() {
        GameObject tcgo = GameObject.Find("TransitionManager");
        if (tcgo == null) {

            tcgo = new GameObject("TransitionManager");
            tcgo.transform.position = Vector3.zero;
            tcgo.AddComponent<TransitionController>();

            Texture2D blackTexture = new Texture2D(1, 1);
            blackTexture.SetPixel(0, 0, Color.black);
            blackTexture.Apply();

            tcgo.GetComponent<TransitionController>().fadeTexture = blackTexture;

        }
        return tcgo.GetComponent<TransitionController>();
    }

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnGUI() {

        if (fadeAmount >= 0) {
            GUI.color = new Color(0, 0, 0, fadeAmount);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        }

    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        FadeIn(DEFAULT_FADE_SPEED);
    }

    public bool IsBlack() {
        return fadeAmount >= 1f;
    }

    public bool IsClear() {
        return fadeAmount <= 0f;
    }

    public void FadeOut(float _fadeTime) {
        fadeAmount = 0.99f;
        StartCoroutine(FadeOutCR(_fadeTime));
    }

    private IEnumerator FadeOutCR(float _fadeTime) {

        fading = true;

        float dTime = 0;
        while (dTime < _fadeTime) {
            dTime += Time.deltaTime;
            fadeAmount = Mathf.Clamp01(dTime / _fadeTime);
            yield return null;
        }

        fadeAmount = 1f;
        fading = false;

    }

    public void FadeIn(float _fadeTime) {
        fadeAmount = 0.01f;
        StartCoroutine(FadeInCR(_fadeTime));
    }

    private IEnumerator FadeInCR(float _fadeTime) {

        fading = true;

        float dTime = 0;
        while (dTime < _fadeTime) {
            dTime += Time.deltaTime;
            fadeAmount = 1f - Mathf.Clamp01(dTime / _fadeTime);
            yield return null;
        }

        fadeAmount = 0f;
        fading = false;

    }

    public void LoadScene(string _sceneName) {
        StartCoroutine(LoadSceneCR(_sceneName));
    }

    private IEnumerator LoadSceneCR(string _sceneName) {
        StartCoroutine(FadeOutCR(DEFAULT_FADE_SPEED));
        while (!IsBlack()) {
            yield return null;
        }
        SceneManager.LoadScene(_sceneName);
    }

}
