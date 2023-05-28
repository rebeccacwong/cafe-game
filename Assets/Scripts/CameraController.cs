using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public GameObject[] m_walls;

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

    private bool m_updateWallRendering = true;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.enabled = true;
        m_ActiveCamera = Camera.main;

        cc_mainCharacter = GameObject.Find("MainCharacter");

        this.m_characterStartPos = cc_mainCharacter.transform.position;
        this.m_camStartPos = this.m_followCharacterCamera.transform.position;
    }

    private void Update()
    {
        if (ShouldUpdateRendering())
        {
            foreach (GameObject wall in m_walls)
            {
                // Set all the walls active by default, then update not to render if necessary
                Utils.SetActiveParentAndChildren(wall, true);
            }

            if (this.isActiveCameraStatic())
            {
                this.UpdateRenderingForStaticCams();

                // for static cameras, only need to update rendering once.
                this.m_updateWallRendering = false;
                return;
            }

            if (!Utils.isPointInCafe(this.m_ActiveCamera.transform.position))
            {
                RaycastHit hit;
                List<GameObject> wallsToUpdate = new List<GameObject>();

                foreach (GameObject wall in m_walls)
                {
                    Plane[] cameraFrustrum = GeometryUtility.CalculateFrustumPlanes(this.m_ActiveCamera);
                    Bounds bounds = wall.GetComponent<Collider>().bounds;
                    Collider wallCollider = wall.GetComponent<Collider>();

                    //Vector3 closestWallPoint = Physics.ClosestPoint(this.m_ActiveCamera.transform.position, wallCollider, wallCollider.transform.position, wallCollider.transform.rotation);
                    Vector3 camToWallVect = (wall.transform.position - this.m_ActiveCamera.transform.position).normalized;

                    if (GeometryUtility.TestPlanesAABB(cameraFrustrum, bounds))
                    {
                        if (Physics.SphereCast(this.m_ActiveCamera.transform.position, 1f, camToWallVect, out hit, 1f, LayerMask.GetMask("Walls"))
                            && hit.collider.gameObject == wall.gameObject)
                        {
                            wallsToUpdate.Add(wall);
                        }
                        else if (Physics.OverlapSphere(this.m_ActiveCamera.transform.position, 2f, LayerMask.GetMask("Walls")).Contains<Collider>(wallCollider))
                        {
                            wallsToUpdate.Add(wall);
                        }
                    }
                }
                // only update visibility after to ensure occlusions are handled properly
                foreach (GameObject wall in wallsToUpdate)
                {
                    Utils.SetActiveParentAndChildren(wall, false);
                }
            }
        }

    }

    // Update is called once per frame
    void LateUpdate()
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
        } else if (activeCameraName == "far camera")
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
        this.m_updateWallRendering = true;

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

    private bool ShouldUpdateRendering()
    {
        return this.m_updateWallRendering;
    }

    /*
     * Assuming the active camera is static, don't render the walls
     * that would be blocking the static cam.
     */
    private void UpdateRenderingForStaticCams()
    {
        foreach(GameObject wall in this.m_walls)
        {
            if (wall.name == "wallentrance" || wall.name == "windowwall")
            {
                Utils.SetActiveParentAndChildren(wall, false);
            }
            if (wall.name == "ceiling" && this.m_ActiveCamera.name == "far camera")
            {
                Utils.SetActiveParentAndChildren(wall, false);
            }
        }
    }

    /*
     * Returns true if the camera currently active is a static camera.
     */
    private bool isActiveCameraStatic()
    {
        return (this.m_ActiveCamera.name != "mainCharacterCamera");
    }
}
