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

    /*
     * Pauses all character movement (transforms) and animations.
     */
    public void Pause();

    public void Unpause();

    
    /*
     * Only pauses the character animation cycle. The transforms can 
     * continue to change.
     */
    public void pauseAnimation();

    public void unpauseAnimation();



    public GameObject GetPausableGameObject();
}
