using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    /*
     * This interface is defined for all objects
     * within the scene that the main character 
     * can interact with.
     */

    /* 
     * Allow the main character to interact with
     * this IInteractable object
     */
    public void interactWithObject();

    /*
     * Returns whether or not the main character
     * is allowed to interact with this
     * IInteractable object at this time.
     * */
    public bool canInteract();
}
