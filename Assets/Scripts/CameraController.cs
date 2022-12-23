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

    [SerializeField]
    [Tooltip("FollowCharacterCamera")]
    public Camera m_followCharacterCamera;

    [SerializeField]
    [Tooltip("The Cinemachine virtualCamera to follow the character")]
    public GameObject m_cinemachineCam;
    #endregion

    private Camera m_ActiveCamera;

    public bool followingCharacter = false;

    GameObject cc_mainCharacter;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.enabled = true;
        m_ActiveCamera = Camera.main;

        cc_mainCharacter = GameObject.Find("MainCharacter");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // swap cameras
            this.swapCameras();
        }
    }

    /*
     * swap camera that follows character with the wide angle
     */ 
    private void swapCameras()
    {
        string activeCameraName = m_ActiveCamera.gameObject.name;
        if (activeCameraName == "MainCharacterCamera")
        {
            changeActiveCamera("far camera");
            //m_cinemachineCam.SetActive(false);
            followingCharacter = false;
        } else
        {
            changeActiveCamera(m_followCharacterCamera);
            //m_cinemachineCam.SetActive(true);
            followingCharacter = true;
        }
    }

    public void changeActiveCamera(Camera camera)
    {
        this.changeActiveCamera(camera.gameObject.name);
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
        this.m_ActiveCamera = newActiveCamera;
        Debug.Log("Changed to camera " + cameraName);
    }

    public Camera getActiveCamera()
    {
        if (this.m_ActiveCamera == null)
        {
            this.m_ActiveCamera = Camera.main;
            Camera.main.enabled = true;
        }
        return this.m_ActiveCamera;
    }
}
