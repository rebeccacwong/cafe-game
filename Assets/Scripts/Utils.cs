using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class Utils
{

    private static CameraController getCameraController()
    {
        GameObject cameraController = GameObject.Find("CameraController");
        return cameraController.GetComponent<CameraController>();
    }

    /*
     * Returns the GameObject that the mouse position is on
     * if the mouse is over an object. Otherwise returns null.
     */
    public static GameObject returnObjectMouseIsOn()
    {
        Debug.LogWarning("Clicked");
        Vector3 target = Input.mousePosition;
        CameraController camController = getCameraController();

        Ray ray = camController.getActiveCamera().ScreenPointToRay(target);

        if (Physics.Raycast(ray, out RaycastHit hitData, 100))
        {
            if (hitData.transform != null)
            {
                return hitData.transform.gameObject;
            }
        }
        return null;
    }

    /*
     * Gets the world coordinates of mouse position
     */
    public static void worldPointOfMousePosition()
    {

    }
}