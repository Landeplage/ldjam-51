using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBouncer : MonoBehaviour
{
    public float bounceTime = 0.5f, waitTime = 1f;
    public Vector2 movement;
    public EasingFunction.Ease easeUp, easeDown;

    SpriteRenderer rt;
    Vector2 initialPosition;
    float t, wait;
    bool doBounce;
    EasingFunction.Function easeUpFunc, easeDownFunc;

    private void Awake() {
        rt = GetComponent<SpriteRenderer>();
        initialPosition = rt.transform.localPosition;
        wait = waitTime;
        t = 0.0f;
        easeUpFunc = EasingFunction.GetEasingFunction(easeUp);
        easeDownFunc = EasingFunction.GetEasingFunction(easeDown);
    }

    private void Update() {
        if (doBounce) {
            if (t < 1.0f) {
                t += Time.unscaledDeltaTime / (bounceTime / 2f);

                if (t >= 1.0f) {
                    t = 1.0f;
                    doBounce = false;
                }
            }
        } else {
            if (t > 0.0f) {
                t -= Time.unscaledDeltaTime / (bounceTime / 2f);

                if (t <= 0f) {
                    t = 0.0f;
                }
            } else {
                if (wait > 0.0f) {
                    wait -= Time.unscaledDeltaTime / waitTime;
                }
                if (wait <= 0.0f) {
                    doBounce = true;
                    wait = waitTime;
                }
            }
        }

        float easeMove = easeUpFunc(0f, 1.0f, t);
        if (!doBounce) {
            easeMove = easeDownFunc(0f, 1.0f, t);
        }
        rt.transform.localPosition = new Vector2(initialPosition.x + (easeMove * movement.x), initialPosition.y + (easeMove * movement.y));
    }

    public void ResetPosition() {
        if (rt != null) {
            rt.transform.localPosition = initialPosition;
        }
    }
}
