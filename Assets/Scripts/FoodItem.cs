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
    public void InstantiateFoodItem(Chair chairSeatedIn)
    {

    }
}
