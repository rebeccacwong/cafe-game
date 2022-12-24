using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class Utils
{
    private static CameraController cc_cameraController;

    private static CameraController getCameraController()
    {
        GameObject cameraController = GameObject.Find("CameraController");
        return cameraController.GetComponent<CameraController>();
    }

    public static bool isObjectClicked(GameObject obj)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.LogWarning("Clicked");
            Vector3 target = Input.mousePosition;
            CameraController camController = getCameraController();

            Ray ray = camController.getActiveCamera().ScreenPointToRay(target);

            if (Physics.Raycast(ray, out RaycastHit hitData, 100))
            {
                Debug.LogWarning("Hit");
                if (hitData.transform != null)
                {
                    return (hitData.transform.gameObject == obj);
                }
            }
        }
        return false;
    }

    /*
     * Gets the world coordinates of mouse position
     */
    public static void worldPointOfMousePosition()
    {

    }
}