using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The prefab of the item on the menu. It must have a foodItem script on it.")]
    GameObject[] menuGameObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FoodItem returnRandomItemFromMenu()
    {
        return menuGameObjects[Random.Range(0, menuGameObjects.Length)].GetComponent<FoodItem>();
    }
}
