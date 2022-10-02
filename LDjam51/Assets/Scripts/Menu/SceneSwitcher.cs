using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneSwitcher : MonoBehaviour
{
	const float FADE_IN_TIME = 2f, FADE_OUT_TIME = 1f, WAIT_TIME = 0.3f;

	static float fade = 1.0f;
	static string queuedScene = "";
	static GameObject fadeCanvas;
	static Material fadeCanvasImageMaterial;

    static public UnityEvent onUpdateFadeCanvas;

    static public UnityEvent onFadeOut = new UnityEvent(), onEnterScene = new UnityEvent(), onExitScene = new UnityEvent();

    static float initialWait;

    private void Awake() {
        if (onUpdateFadeCanvas == null) {
            onUpdateFadeCanvas = new UnityEvent();
        } else {
            onEnterScene.Invoke();
        }

        initialWait = 0.1f;
        UpdateFadeCanvas();
    }

    private void Start() {
        onEnterScene.Invoke();
    }

    void Update()
	{
		if (queuedScene == "") {
			if (fade > 0.0f && initialWait <= 0f) {
				fade -= Time.unscaledDeltaTime / FADE_IN_TIME;

				if (fade < 0.0f) {
					fade = 0.0f;
					fadeCanvas.SetActive(false);
				}

				UpdateFadeCanvas();
			} else {
                if (initialWait > 0f) {
                    initialWait -= Time.deltaTime; // <- Don't use unscaled time for this one
                }
            }
		} else {
			if (fade < 1.0f) {
				fade += Time.unscaledDeltaTime / FADE_OUT_TIME;

				if (fade >= 1.0f) {
					fade = 1.0f;
					StartCoroutine(LoadQueuedScene());
				}

				UpdateFadeCanvas();
			}
		}
	}

    static public float GetFade() {
        return fade;
    }

	static public void Restart()
	{
        GoTo(SceneManager.GetActiveScene().name);
    }

	static public void GoTo(string sceneName)
	{
		queuedScene = sceneName;
		fadeCanvas.SetActive(true);
        onFadeOut.Invoke();
    }

	static IEnumerator LoadQueuedScene()
	{
		yield return new WaitForSecondsRealtime(WAIT_TIME);

        onExitScene.Invoke();

        SceneManager.LoadScene(queuedScene);

        queuedScene = "";
	}

	static void UpdateFadeCanvas()
	{
		// Create a fadecanvas if it doesn't exist
		if (fadeCanvas == null) {
			fadeCanvas = Instantiate(Resources.Load("FadeCanvas") as GameObject);

			// Get material of fadecanvas image
			fadeCanvasImageMaterial = fadeCanvas.transform.Find("Image").GetComponent<Image>().material;
		}

		// Make fadecanvas block raycasts only when fading out
		fadeCanvas.transform.GetChild(0).GetComponent<Image>().raycastTarget = (queuedScene != "");

		// Send fade-value to shader
		fadeCanvasImageMaterial.SetFloat("_Fade", EasingFunction.EaseOutSine(0f, 1f, fade));

        onUpdateFadeCanvas.Invoke();
	}

    public static string GetCurrentSceneName() {
        return SceneManager.GetActiveScene().name;
    }
}
