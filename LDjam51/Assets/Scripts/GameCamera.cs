using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    Vector3 lastMousePosition = Vector3.zero;

    void Update()
    {
        PanCameraWasd();
        PanCameraMiddleMouse();
    }

    void PanCameraWasd()
    {
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            movement.y += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.y -= 1.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement.x -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x += 1.0f;
        }
        if (movement.magnitude > 0.0) {
            transform.position +=  Time.deltaTime * 15.0f * movement.normalized;
        }
    }

    void PanCameraMiddleMouse()
    {
        if (Input.GetMouseButton(2))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            mouseDelta.z = 0.0f;
            transform.position -= mouseDelta * 0.02f;
        }
        lastMousePosition = Input.mousePosition;
    }
}
