using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// The Menu is a Singleton object that manages the FoodItems that can
/// be sold in the cafe. It persists across scenes.
/// 
/// </summary>
public class Menu : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The prefab of the item on the menu. It must have a foodItem script on it.")]
    GameObject[] menuGameObjects;

    private static Menu _instance;
    public static Menu Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Menu is null!");
            }
            return _instance;
        }
    }

    private List<FoodItem> m_foodItems = new List<FoodItem>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        updateFoodItemsList();
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
        // TODO: update so that it returns popular items with higher probability, weighted random with popularityIndex as the weight
        return m_foodItems[Random.Range(0, m_foodItems.Count)].GetComponent<FoodItem>();
    }

    // Returns a list of length n, containing n unique random items from the menu.
    public List<FoodItem> getNUniqueRandomItemsFromMenu(int n)
    {
        List<int> indices = new List<int>();
        List<FoodItem> items = new List<FoodItem>();

        while (indices.Count < Mathf.Min(n, m_foodItems.Count))
        {
            int index = Random.Range(0, m_foodItems.Count);
            if (!indices.Contains(index))
            {
                Debug.LogWarningFormat("Adding index {0}, which is {1}", index, m_foodItems[index].itemName);
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
