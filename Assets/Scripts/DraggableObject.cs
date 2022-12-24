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

    public void onUpdateDragObject();
}
