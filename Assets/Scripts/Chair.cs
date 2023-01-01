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
    [Tooltip("The normalized direction vector that the chair is facing towards")]
    public Vector3 facingDirection;

    private void Awake()
    {
        this.facingDirection =
            Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.y, Vector3.up) * facingDirection;
    }
}
