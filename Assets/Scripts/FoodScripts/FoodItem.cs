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

    [SerializeField]
    [Tooltip("Optional parameter, The scale that the food item should be at after being put on table")]
    public float tableScaleFactor;

    [SerializeField]
    [Tooltip("Optional parameter, the offset that the transform should be so it's in the center of model")]
    public Vector3 offsetPosition;

    private BoxCollider cc_boxCollider;

    private void Awake()
    {
        this.cc_boxCollider = gameObject.GetComponent<BoxCollider>();
    }

    /*
     * Spawns an instance of the food item.
     * Takes in the chair that the customer is seated in
     * to calculate where the food item should spawn.
     */
    public FoodItem InstantiateFoodItem(Chair chairSeatedIn)
    {
        if (!chairSeatedIn)
        {
            return null;
        }

        Table table = chairSeatedIn.getTable();
        Debug.Assert(table != null, "Need a table to instantiate the food item on, got null valued table obj");

        // start by placing food item exactly where chair is, but in respect to table local space
        Vector3 chairPosInLocalTableSpace = table.transform.InverseTransformPoint(chairSeatedIn.transform.position);
        Vector3 pos = chairPosInLocalTableSpace;

        Debug.LogFormat("Chair position in local table space is {0}", chairPosInLocalTableSpace);

        pos.y = (table.getHeight() / 2);

        Debug.LogFormat("Chair facing direction vector is: {0}", chairSeatedIn.facingDirection);

        Debug.Log("Table width on x axis: " + table.getWidthOnXAxis());

        if (chairSeatedIn.facingDirection.z != 0)
        {
            // chair is pointing in z direction
            pos.x = (table.getWidthOnXAxis() / 4f) * chairSeatedIn.facingDirection.z;
        }
        else if (chairSeatedIn.facingDirection.x != 0)
        {
            pos.x = (table.getWidthOnXAxis() / 4f) * -chairSeatedIn.facingDirection.x;
        }
        else
        {
            Debug.LogError("Cannot instantiate food item. Some chair needs to fix its facingDirection");
            return null;
        }

        // Instantiate a new food item as a child of the table
        FoodItem newFoodItem = Instantiate(this, table.transform);

        if (newFoodItem.tableScaleFactor != 0)
        {
            newFoodItem.transform.localScale = new Vector3(1, 1, 1) * newFoodItem.tableScaleFactor;
        }

        pos.y += (this.getHeight() / 2) - (this.getCenter().y); // offset so base of object is at bottom of table
        Debug.LogFormat("FoodItem position relative to table parent is: {0}", pos);
        newFoodItem.transform.localPosition = pos;

        return newFoodItem;
    }

    public float getHeight()
    {
        return this.cc_boxCollider.size.y;
    }

    public Vector3 getCenter()
    {
        return this.cc_boxCollider.center;
    }
}