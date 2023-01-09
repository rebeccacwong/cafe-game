using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDraggableObject
{
    public bool isBeingDragged
    {
        get;
        set;
    }

     /*
     * Enables dragging of the character. Returns true
     * if character can be dragged. Returns false if
     * the character cannot be dragged.
     */
    public bool startDraggingObject();

    /*
     * Stops dragging character and enables appropriate
     * follow through action. 
     */
    public void stopDraggingObject();

    /*
     * Updates the position of the object to match
     * the mouse position if dragging is enabled
     * on this object
     */
    public void onUpdateDragObject();
}
