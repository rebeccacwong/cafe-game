using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The layer mask of the floor")]
    public LayerMask floorLayerMask;

    #region Editor Variables
    [SerializeField]
    [Tooltip("Camera objects")]
    public Camera[] m_cameras;

    [SerializeField]
    [Tooltip("FollowCharacterCamera")]
    public Camera m_followCharacterCamera;
    #endregion

    private Camera m_ActiveCamera;
    private Vector3 m_characterStartPos;
    private Vector3 m_camStartPos;

    [HideInInspector]
    public bool followingCharacter = false;

    [HideInInspector]
    GameObject cc_mainCharacter;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.enabled = true;
        m_ActiveCamera = Camera.main;

        cc_mainCharacter = GameObject.Find("MainCharacter");

        this.m_characterStartPos = cc_mainCharacter.transform.position;
        this.m_camStartPos = this.m_followCharacterCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // swap cameras
            this.swapCameras();
        }
        //this.followCharacter();
    }

    private void followCharacter()
    {
        this.m_followCharacterCamera.transform.LookAt(new Vector3(cc_mainCharacter.transform.position.x, cc_mainCharacter.transform.position.y + 3, cc_mainCharacter.transform.position.z));

        Vector3 displacement = this.cc_mainCharacter.transform.position - this.m_characterStartPos;
        this.m_followCharacterCamera.transform.position = this.m_camStartPos + displacement;
    }

    /*
     * swap camera that follows character with the wide angle
     */ 
    private void swapCameras()
    {
        string activeCameraName = m_ActiveCamera.gameObject.name;
        if (activeCameraName == "mainCharacterCamera")
        {
            changeActiveCamera("far camera");
            followingCharacter = false;
        } else
        {
            changeActiveCamera(m_followCharacterCamera.name);
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
        Debug.LogFormat("Changed from camera {0} to camera {1}", this.m_ActiveCamera.gameObject.name, cameraName);
        this.m_ActiveCamera = newActiveCamera;
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
