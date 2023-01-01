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
                IPausable[] pausableObjects = getAllPausableObjects();
                if (draggableObject != null)
                {
                    if (obj.tag == "Customer")
                    {
                        // TODO: ensure that pausing succeeds
                        // Pause all movement except for the customer
                        
                        Debug.LogWarning("Found " + pausableObjects.Length + " pausable objects");

                        foreach (IPausable pausableObj in pausableObjects)
                        {
                            if (pausableObj.GetPausableGameObject() != obj)
                            {
                                pausableObj.Pause();
                            }
                        }

                        obj.GetComponent<IPausable>().pauseAnimation();
                        cc_spawnController.PauseCustomerSpawning();
                    }

                    m_currentlyCarrying = draggableObject;
                    draggableObject.startDraggingObject();
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && m_currentlyCarrying != null)
        {
            m_currentlyCarrying.stopDraggingObject();
            m_currentlyCarrying = null;

            cc_spawnController.ResumeCustomerSpawning();

            Debug.LogWarning("Unpausing all pausable objects");
            foreach (IPausable pausableObj in getAllPausableObjects())
            {
                pausableObj.Unpause();
            }

        }
    }

    private IPausable[] getAllPausableObjects()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IPausable>().ToArray();
    }
}
