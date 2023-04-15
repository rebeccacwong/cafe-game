using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Name of the food item")]
    public string itemName;

    [SerializeField]
    [Tooltip("The image corresponding to the item")]
    public Sprite itemImage;

    [SerializeField]
    [Tooltip("Price of the item")]
    public float itemPrice;

    [SerializeField]
    [Tooltip("The location that the main character must go to in order to prepare item")]
    public string prepLocation;

    /*
     * Spawns an instance of the food item.
     * Takes in the chair that the customer is seated in
     * to calculate where the food item should spawn.
     */
    public bool InstantiateFoodItem(Chair chairSeatedIn)
    {
        if (!chairSeatedIn)
        {
            return false;
        }

        Table table = chairSeatedIn.getTable();
        Vector3 pos = Vector3.zero;

        pos.y = table.getHeight() / 2;

        if (chairSeatedIn.facingDirection.x != 0)
        {
            // need to verify whether this is the right calculation
            pos.x = (table.getWidthOnXAxis() / 4f) - chairSeatedIn.facingDirection.x;
        } else if (chairSeatedIn.facingDirection.z != 0)
        {
            pos.z = (table.getWidthOnZAxis() / 4f) - chairSeatedIn.facingDirection.z;
        } else
        {
            Debug.LogWarning("Should never get here. Some chair needs to fix its facingDirection");
            return false;
        }

        // Instantiate a new food item as a child of the table
        Instantiate(this, table.transform);
        return true;
    }
}
