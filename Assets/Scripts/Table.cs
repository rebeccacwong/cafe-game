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

    public float getHeight()
    {
        return this.cc_boxCollider.size.y;
    }
}
