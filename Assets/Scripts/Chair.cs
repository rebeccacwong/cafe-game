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

    public Vector3 facingDirection;

    private void Awake()
    {
        this.facingDirection =
            Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.y, Vector3.up) * new Vector3(1, 0, 0);
    }
}
