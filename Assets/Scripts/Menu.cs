using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The prefab of the item on the menu. It must have a foodItem script on it.")]
    GameObject[] menuGameObjects;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public FoodItem returnRandomItemFromMenu()
    {
        return menuGameObjects[Random.Range(0, menuGameObjects.Length)].GetComponent<FoodItem>();
    }
}
