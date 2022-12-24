using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPausable
{
    /*
     * All base class for all objects 
     * whose update functions can be
     * paused.
     */

    public bool isPaused
    {
        get;
    }

    public void Pause();

    public void Unpause();

    public GameObject GetPausableGameObject();
}
