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
    #endregion

    private Vector3 m_mainCharacterStartPos;
    private Quaternion m_mainCharacterStartRot;
    private Vector3 m_followCharacterCamStartPos;
    private Quaternion m_followCameraCamStartRot;
    private Camera m_ActiveCamera;

    public bool followingCharacter = false;

    GameObject cc_mainCharacter;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.enabled = true;
        m_ActiveCamera = Camera.main;

        cc_mainCharacter = GameObject.Find("MainCharacter");
        this.m_mainCharacterStartPos = cc_mainCharacter.transform.position;
        this.m_followCharacterCamStartPos = m_followCharacterCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // swap cameras
            this.swapCameras();
        }
        //this.updateFollowCamera();
    }

    /*
     * Updates the position of the camera that follows
     * the player
     */
    //private void updateFollowCamera()
    //{
    //    Vector3 displacement = cc_mainCharacter.transform.position - this.m_mainCharacterStartPos;
    //    m_followCharacterCamera.transform.position = this.m_followCharacterCamStartPos + displacement;

    //    m_followCharacterCamera.transform.LookAt(cc_mainCharacter.transform.position); ;
    //}

    /*
     * swap camera that follows character with the wide angle
     */ 
    private void swapCameras()
    {
        string activeCameraName = m_ActiveCamera.gameObject.name;
        if (activeCameraName == "MainCharacterCamera")
        {
            changeActiveCamera("far camera");
            followingCharacter = false;
        } else
        {
            changeActiveCamera(m_followCharacterCamera);
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
