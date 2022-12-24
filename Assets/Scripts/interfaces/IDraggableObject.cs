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

    public void startDraggingObject();

    public void stopDraggingObject();

    /*
     * Updates the position of the object to match
     * the mouse position if dragging is enabled
     * on this object
     */
    public void onUpdateDragObject();
}
