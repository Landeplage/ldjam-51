using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuController : MonoBehaviour
{
	public float transitionTimeOut = 0.4f;
	public float transitionTimeIn = 0.4f;
    public float transitionWaitTime = 0f;

    [Header("In")]
    public Vector2 movementIn;
    public Vector2 scaleIn;
    public EasingFunction.Ease easeIn;

    [Header("Out")]
    public Vector2 movementOut;
    public Vector2 scaleOut;
    public EasingFunction.Ease easeOut;

    [Header("General")]
    public bool showOnStart = true;
	public float initialDelay = 0f;
	public CanvasGroup[] Menus;

	CanvasGroup currentMenu = null, queuedMenu = null;
	float alpha = 0f;
    float waitTimer = 0f;

	CanvasGroup[] history = new CanvasGroup[5];

    [HideInInspector]
    public UnityEvent<string> onGoTo, onEnteredMenu, onExitedMenu;

    private EasingFunction.Function easingFunctionIn, easingFunctionOut;

    void Awake()
    {
		// Loop through each menu
		foreach(CanvasGroup cg in Menus)
		{
			cg.gameObject.SetActive(false);
			cg.alpha = 0.0f;
		}

        // Initialize history array
        for (int i = 0; i < history.Length; i++) {
			history[i] = null;
		}

        // Get easing functions
        easingFunctionIn = EasingFunction.GetEasingFunction(easeIn);
        easingFunctionOut = EasingFunction.GetEasingFunction(easeOut);
    }

    private void Start() {
        // Show initial menu
        if (showOnStart) {
            GoToDelayed(Menus[0].gameObject.name, initialDelay);
        }
    }

    void Update()
	{
		// Handle transition
		if (queuedMenu != null) {
			if (alpha >= 0.0f) {
				alpha -= Time.unscaledDeltaTime / transitionTimeOut;

				if (alpha < 0.0f) {
					alpha = 0.0f;

                    if (waitTimer > 0f) {
                        waitTimer -= Time.unscaledDeltaTime;
                    } else {
                        EnterQueuedMenu();
                        waitTimer = transitionWaitTime;
                    }
				}
			}
		} else {
			if (currentMenu != null) {
				if (alpha < 1.0f) {
					alpha += Time.unscaledDeltaTime / transitionTimeIn;

					if (alpha > 1.0f) {
						alpha = 1.0f;
					}
				}
			}
		}

		// Apply animation to canvas group
		if (currentMenu != null) {
            Vector3 pos, scale;

            float moveT, scaleT;

            if (queuedMenu == null) {
                // In
                moveT = easingFunctionIn(1f, 0f, alpha);
                scaleT = easingFunctionIn(1f, 0f, alpha);

                pos = new Vector3(movementIn.x * moveT, movementIn.y * moveT);
                scale = new Vector3(1.0f + scaleIn.x * scaleT, 1.0f + scaleIn.y * scaleT, 1.0f);
            } else {
                // Out
                moveT = easingFunctionOut(1f, 0f, alpha);
                scaleT = easingFunctionOut(1f, 0f, alpha);

                pos = new Vector3(movementOut.x * moveT, movementOut.y * moveT);
                scale = new Vector3(1.0f + scaleOut.x * scaleT, 1.0f + scaleOut.y * scaleT, 1.0f);
            }

            // Apply
            currentMenu.GetComponent<RectTransform>().anchoredPosition = pos;
			currentMenu.alpha = Mathf.Clamp(alpha * 1.5f, 0.0f, 1.0f);
            currentMenu.transform.localScale = scale;
        }
	}

	// Delay goto by x seconds
	public void GoToDelayed(string menuName, float delay)
	{
        StartCoroutine(GoToDelayedRoutine(menuName, delay));
	}

	IEnumerator GoToDelayedRoutine(string menuName, float delay)
	{
		yield return new WaitForSecondsRealtime(delay);

        // Initial menu
        GoTo(menuName);
	}

	public void GoTo(string menuName)
	{
		GoTo(menuName, true);
	}

	public void GoTo(string menuName, bool addToHistory)
	{
		// Add current menu to history
		if (addToHistory) {
			AddCurrentToHistory();
		}

        // Queue next menu
        Transform menu = transform.Find(menuName);
        if (menu != null) {
            queuedMenu = menu.GetComponent<CanvasGroup>();
        } else {
            Debug.LogWarning("MenuController: Tried to go to a menu that doesn't exist.");
        }

		// Block raycasts on all menus
		foreach(CanvasGroup cg in Menus) {
			cg.blocksRaycasts = false;
		}

        onGoTo.Invoke(menuName);
	}

	public void GoToPrevious()
	{
		if (history[0] == null) {
			return;
		}

		// Go to first index in history
		GoTo(history[0].gameObject.name, false);

		// Move existing elements one index forward
		for(int i = 0; i < history.Length - 1; i++) {
			history[i] = history[i + 1];
		}

		// Make sure last index is null
		history[history.Length - 1] = null;
	}

    public string GetCurrentMenuName() {
        if (currentMenu != null) {
            return currentMenu.name;
        }

        return "";
    }

    public void SetAlpha(float alpha) {
        this.alpha = alpha;
    }

    public float GetAlpha() {
        return alpha;
    }

	void AddCurrentToHistory()
	{
		// Move existing elements one index back
		for(int i = history.Length - 1; i > 0; i--) {
			history[i] = history[i - 1];
		}

		// Add current menu to history
		history[0] = currentMenu;
	}

    void EnterQueuedMenu() {

        // Make sure current menu is completely transparent and deactivated
        if (currentMenu != null) {
            currentMenu.alpha = 0.0f;
            currentMenu.gameObject.SetActive(false);
            onExitedMenu.Invoke(currentMenu.name);
        }

        // Switch menu
        currentMenu = queuedMenu;
        currentMenu.blocksRaycasts = true;
        queuedMenu = null;

        // Activate current menu
        currentMenu.gameObject.SetActive(true);

        onEnteredMenu.Invoke(currentMenu.name);
    }

    public void ForceMenu(string menuName) {
        CanvasGroup newMenu = transform.Find(menuName).GetComponent<CanvasGroup>();
        if (newMenu == null) {
            return;
        }

        if (currentMenu != null) {
            currentMenu.gameObject.SetActive(false);
            currentMenu.alpha = 0.0f;
        }

        if (queuedMenu != null) {
            queuedMenu = null;
        }

        currentMenu = newMenu;
        currentMenu.blocksRaycasts = true;
        currentMenu.gameObject.SetActive(true);
    }
}
