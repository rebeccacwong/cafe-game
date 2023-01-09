using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour
{
    // the object that the mouse is currently carrying
    private IDraggableObject m_currentlyCarrying = null;

    #region Cached components
    private SpawnController cc_spawnController;
    #endregion

    private void Awake()
    {
        cc_spawnController = GameObject.Find("CustomerSpawner").GetComponent<SpawnController>();
    }

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        // Poll for clicks
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Utils.returnObjectMouseIsOn();
            if (obj != null)
            {
                IDraggableObject draggableObject = obj.GetComponent<IDraggableObject>();
                m_currentlyCarrying = draggableObject;

                if (draggableObject != null)
                {
                    bool isDraggable = draggableObject.startDraggingObject();
                    if (isDraggable)
                    {
                        if (obj.tag == "Customer")
                        {
                            Debug.LogWarning("Pausing all pausable objects");

                            foreach (IPausable pausableObj in getAllPausableObjects())
                            {
                                // Pause every object except for the customer
                                if (pausableObj.GetPausableGameObject() != obj)
                                {
                                    pausableObj.Pause();
                                }
                            }

                            cc_spawnController.PauseCustomerSpawning();
                        }
                    }
                }
            }
        }
        //if (Input.GetMouseButtonUp(0) && m_currentlyCarrying != null)
        //{
        //    // TODO: Fix problem where you drag a customer in line
        //    // to an invalid location so it goes back to line.
        //    // Currently the customer next in line steps on top
        //    // of the customer that you let go of
        //    m_currentlyCarrying.stopDraggingObject();
        //    m_currentlyCarrying = null;

        //    cc_spawnController.ResumeCustomerSpawning();

        //    Debug.LogWarning("Unpausing all pausable objects");
        //    foreach (IPausable pausableObj in getAllPausableObjects())
        //    {
        //        if (pausableObj.isPaused)
        //        {
        //            pausableObj.Unpause();
        //        }
        //    }

        //}
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) && m_currentlyCarrying != null)
        {
            m_currentlyCarrying.stopDraggingObject();
            m_currentlyCarrying = null;

            cc_spawnController.ResumeCustomerSpawning();

            Debug.LogWarning("Unpausing all pausable objects");
            foreach (IPausable pausableObj in getAllPausableObjects())
            {
                if (pausableObj.isPaused)
                {
                    pausableObj.Unpause();
                }
            }

        }
    }

    private IPausable[] getAllPausableObjects()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IPausable>().ToArray();
    }
}
