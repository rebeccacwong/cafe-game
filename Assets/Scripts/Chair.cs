using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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

    private bool m_inUse;
        
    private void Awake()
    {
        this.facingDirection =
            Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.y, Vector3.up) * this.facingDirection;
    }

    // Returns the table that the chair is at
    public Table getTable()
    {
        if (Physics.Raycast(
                gameObject.transform.position,
                this.facingDirection,
                out RaycastHit hitData,
                5f,
                LayerMask.GetMask("Tables")))
        {
            GameObject hitObj = hitData.collider.gameObject;
            return hitObj.GetComponent<Table>();
        }
        return null;
    }

    public void useChair()
    {
        this.m_inUse = true;
    }

    public bool inUse()
    {
        return this.m_inUse;
    }
}
