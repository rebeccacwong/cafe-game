using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("Camera objects")]
    public Camera[] m_cameras;
    #endregion

    private Camera m_ActiveCamera;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.enabled = true;
        m_ActiveCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeActiveCamera(string cameraName)
    {
        bool validCamera = false;
        Camera newActiveCamera = null;
        foreach (Camera camera in m_cameras)
        {
            if (camera.gameObject.name == cameraName)
            {
                newActiveCamera = camera;
                validCamera = true;
                break;
            }
        }

        if (!validCamera)
        {
            throw new Exception(
                String.Format(
                    "Invalid input to changeActiveCamera. {0} is not a valid camera.", cameraName));
        }
        foreach (Camera camera in m_cameras)
        {
            camera.gameObject.SetActive(false);
        }
        newActiveCamera.gameObject.SetActive(true);
        Debug.Log("Changed to camera " + cameraName);
    }

    public Camera getActiveCamera()
    {
        if (m_ActiveCamera == null)
        {
            m_ActiveCamera = Camera.main;
            Camera.main.enabled = true;
        }
        return m_ActiveCamera;
    }
}
