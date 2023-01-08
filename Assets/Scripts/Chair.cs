using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    /*
     * Class only for the purpose of adding additional metadata
     * for the chair objects
     */

    [SerializeField]
    [Tooltip("The distance from the bottom of the chair legs to the seat")]
    public float heightOfSeat;

    [SerializeField]
    [Tooltip("Input the default facing direction for the chair prefab")]
    public Vector3 facingDirection;

    [SerializeField]
    [Tooltip("The offset z position from the center of the chair, relative to the facing direction")]
    public float offset_z;

    public bool inUse = false;
        
    private void Awake()
    {
        this.facingDirection =
            Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.y, Vector3.up) * this.facingDirection;
    }
}
