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
    public static GameObject returnObjectMouseIsOn(int layerMask = 0)
    {
        Vector3 target = Input.mousePosition;
        CameraController camController = getCameraController();

        Ray ray = camController.getActiveCamera().ScreenPointToRay(target);
        bool raycastOutput;
        RaycastHit hitData;

        if (layerMask == 0)
        {
            raycastOutput = Physics.Raycast(ray, out hitData, 100);
        } else
        {
            raycastOutput = Physics.Raycast(ray, out hitData, 100, layerMask);
        }

        if (raycastOutput)
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