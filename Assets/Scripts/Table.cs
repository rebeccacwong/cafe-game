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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getWidthOnZAxis()
    {
        return this.cc_boxCollider.size.z;
    }

    public float getWidthOnXAxis()
    {
        return this.cc_boxCollider.size.x;
    }

    private float getHeight()
    {
        return this.cc_boxCollider.size.y;
    }

    /*
     * Returns the world-space y value that the 
     * surface of the table is at
     */
    public float getSurfacePointY()
    {
        float height = this.getHeight();
        Vector3 localPos = gameObject.transform.localPosition;

        // assumes that the gameObject's local position is in the center of the obj
        localPos.y += height / 2;
        return (gameObject.transform.localToWorldMatrix * localPos).y;
    }
}
