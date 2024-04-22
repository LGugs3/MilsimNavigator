using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.InputSystem;

public class MousePosition : MonoBehaviour
{
    public Vector3 screenPosition;
    public Vector3 worldPosition;
    

    public Vector3 getWorldPosition()
    {
        screenPosition = Input.mousePosition;

        screenPosition.z = Camera.main.nearClipPlane + 4;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        return worldPosition;
    }
}
