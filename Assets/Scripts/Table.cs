using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    private BoxCollider cc_boxCollider;

    private void Awake()
    {
        this.cc_boxCollider = gameObject.GetComponent<BoxCollider>();
        Debug.Assert(this.cc_boxCollider != null, "All tables must have an associated collider.");
    }

    /*
     * Gets the width of the object along the z-axis measured in local space
     */
    public float getWidthOnZAxis()
    {
        return this.cc_boxCollider.size.z;
    }

    /*
     * Gets the width of the object along the x-axis measured in local space
     */
    public float getWidthOnXAxis()
    {
        return this.cc_boxCollider.size.x;
    }

    /*
     * Gets the height of the object measured in 
     * local space 
     */
    public float getHeight()
    {
        return this.cc_boxCollider.size.y;
    }
}
