using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLightning : MonoBehaviour {

    [SerializeField]
    private Color flashColor;
    private Color startColor;

    private Camera cam;
    private float timer = 0;
    private float flashTime = 0;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        startColor = cam.backgroundColor;
        flashTime = GetFlashTime();
	}
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;

        if (timer > flashTime) {
            timer = 0;
            flashTime = GetFlashTime();
            StartCoroutine(SequencedFlash());
        }

	}

    private float GetFlashTime() {
        return 5f + Random.Range(0f, 4f);
    }

    private void Flash(float _inTime, float _outTime) {
        StartCoroutine(FlashCR(_inTime, _outTime));
    }

    private IEnumerator FlashCR(float _inTime, float _outTime) {
        
        // Fade in
        float timer = 0;
        while (timer < _inTime) {

            float ratio = timer / _inTime;
            Color currentColor = Color.Lerp(startColor, flashColor, ratio);
            cam.backgroundColor = currentColor;

            timer += Time.deltaTime;
            yield return null;

        }

        cam.backgroundColor = flashColor;
        yield return null;

        // Fade out
        timer = 0;
        while (timer < _outTime) {

            float ratio = timer / _outTime;
            Color currentColor = Color.Lerp(flashColor, startColor, ratio);
            cam.backgroundColor = currentColor;

            timer += Time.deltaTime;
            yield return null;

        }

        cam.backgroundColor = startColor;

    }

    private IEnumerator SequencedFlash() {

        Flash(0.05f, 0.08f);
        yield return new WaitForSeconds(0.14f);
        Flash(0.05f, 0.08f);
        yield return new WaitForSeconds(0.14f);
        Flash(0.05f, 2f);

    }

}
