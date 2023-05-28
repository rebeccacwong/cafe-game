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
    private Vector3 prefabFacingDirection;

    public Vector3Int facingDirection;

    private bool m_inUse;
        
    private void Awake()
    {
        this.facingDirection =
            Vector3Int.RoundToInt(Quaternion.AngleAxis(gameObject.transform.rotation.eulerAngles.y, Vector3.up) * prefabFacingDirection);
        if (this.heightOfSeat == 1.85f)
        {
            Debug.LogWarning(facingDirection);
        }
    }

    // Returns the table that the chair is at
    public Table getTable()
    {
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

    public void useChair()
    {
        this.m_inUse = true;
    }

    public bool inUse()
    {
        return this.m_inUse;
    }
}
