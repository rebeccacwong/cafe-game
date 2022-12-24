using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour
{
    // the object that the mouse is currently carrying
    private IDraggableObject m_currentlyCarrying = null;

    private IPausable[] m_pausableObjects;

    private void Awake()
    {
        m_pausableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IPausable>().ToArray();
        Debug.Assert(m_pausableObjects.Length > 0, "Found no pausable objects!");
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
                if (draggableObject != null)
                {
                    if (obj.tag == "Customer")
                    {
                        // Pause all movement except for the customer
                        foreach (IPausable pausableObj in m_pausableObjects)
                        {
                            if (pausableObj.GetPausableGameObject() != obj)
                            {
                                Debug.LogWarning("Pausing");
                                pausableObj.Pause();
                            }
                        }
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

        }
    }
}
