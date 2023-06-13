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

    /*
     * If isVisible is true, renders the mesh for this parent item and all of its 
     * children. Does not render parent or any of its children otherwise.
     */
    public static void SetVisibleParentAndChildren(GameObject parent, bool isVisible)
    {
        MeshRenderer parentRenderer = parent.GetComponent<MeshRenderer>();

        if (parentRenderer)
        {
            parentRenderer.enabled = isVisible;
        }
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            MeshRenderer childRenderer = parent.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>();
            if (childRenderer)
            {
                childRenderer.enabled = isVisible;
            }
        }
    }

    /*
     * Sets the gameObject obj and all of its descendants recurisvely 
     * as as active/notactive depending on isActive.
     */
    public static void SetThisAndAllDescendantsActiveRecursive(GameObject obj, bool isActive)
    {
        if (obj)
        {
            obj.SetActive(isActive);
        }
        foreach (Transform child in obj.transform)
        {
            SetThisAndAllDescendantsActiveRecursive(child.gameObject, isActive);
        }
    }

    public static bool isPointInCafe(Vector3 point)
    {
        Vector3 pointShifted = point;
        pointShifted.y += 2;
        return Physics.Raycast(pointShifted, Vector3.down, Mathf.Infinity, LayerMask.GetMask("Floor"));
    }
}