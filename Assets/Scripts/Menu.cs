using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The prefab of the item on the menu. It must have a foodItem script on it.")]
    GameObject[] menuGameObjects;

    private List<FoodItem> m_foodItems;

    private void Awake()
    {
        updateFoodItemsList();
        DontDestroyOnLoad(this.gameObject);
    }

    private void updateFoodItemsList()
    {
        for (int i = 0; i < menuGameObjects.Length; i++)
        {
            FoodItem item = menuGameObjects[i].GetComponent<FoodItem>();
            Debug.Assert(item != null);
            m_foodItems.Add(item);
        }
    }

    public FoodItem returnRandomItemFromMenu()
    {
        return m_foodItems[Random.Range(0, m_foodItems.Count)].GetComponent<FoodItem>();
    }

    // Returns a list of length n, containing n unique random items from the menu.
    public List<FoodItem> getNUniqueRandomItemsFromMenu(int n)
    {
        List<int> indices = new List<int>();
        List<FoodItem> items = new List<FoodItem>();

        for (int i = 0; i < n; i++)
        {
            int index = Random.Range(0, m_foodItems.Count);
            if (!indices.Contains(index))
            {
                indices.Add(index);
                items.Add(m_foodItems[index]);
            }
        }
        return items;
    }

    public List<FoodItem> listMenuFoodItems()
    {
        return m_foodItems;
    }


    // Returns true if menu change was successful. False otherwise.
    public bool changePrice(FoodItem foodItem, int newPrice)
    {
        foreach(FoodItem item in m_foodItems)
        {
            if (item.itemName == foodItem.itemName)
            {
                item.setPrice(newPrice);
                return true;
            }
        }
        return false;
    }
}
