using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameController : MonoBehaviour
{
    // the object that the mouse is currently carrying
    private IDraggableObject m_currentlyCarrying = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
