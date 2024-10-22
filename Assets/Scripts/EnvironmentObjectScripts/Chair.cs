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
    [Tooltip("The offset z position from the center of the chair, relative to the facing direction")]
    public float offset_z;

    [SerializeField]
    [Tooltip("The default facing direction vector of the prefab.")]
    public Vector3 prefabFacingDirection;

    public Vector3Int facingDirection;

    private bool m_inUse;

    private Table m_table;
        
    private void Awake()
    {
        this.facingDirection =
            Vector3Int.RoundToInt(Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.y, Vector3.up) * prefabFacingDirection);
        this.m_table = getTable();
    }

    // Returns the table that the chair is at
    public Table getTable()
    {
        if (m_table)
        {
            return m_table;
        }

        // If we have not found the table yet, use raycast to find it
        if (Physics.Raycast(
                gameObject.transform.position,
                this.facingDirection,
                out RaycastHit hitData,
                3f,
                LayerMask.GetMask("Tables")))
        {
            GameObject hitObj = hitData.collider.gameObject;
            return hitObj.GetComponent<Table>();
        }
        return null;
    }

    public void useChair(bool isSocial)
    {
        this.m_inUse = true;
        this.m_table.IncreaseCustomerCount(isSocial);
    }

    public bool inUse()
    {
        return this.m_inUse;
    }

    public void leaveChair(bool isSocial)
    {
        this.m_inUse = false;
        this.m_table.DecreaseCustomerCount(isSocial);
    }
}
