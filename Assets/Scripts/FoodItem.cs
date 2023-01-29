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
}
