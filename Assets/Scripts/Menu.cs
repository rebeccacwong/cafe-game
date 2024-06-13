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

    private List<FoodItem> m_foodItems = new List<FoodItem>();

    // Inverse cdf where x values represent cumulative probability
    // and y values represent the index of the item in the menu
    private AnimationCurve invCdf;

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
        refreshPopularities();
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

    /* 
     * Returns a random item from the menu, considering a weighted probability 
     * based off of popularity. This makes the likelihood of returning an item 
     * with a higher probability index greater.
    */
    public FoodItem returnRandomItemFromMenu()
    {
        return m_foodItems[(int) invCdf.Evaluate(Random.value)].GetComponent<FoodItem>();
    }

    // Returns a list of length n, containing n unique uniformly random items from the menu.
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

    public void refreshPopularities()
    {
        Debug.LogWarning("Refreshing popularities in menu.");
        invCdf = new AnimationCurve();

        float sumOfPopularities = 0;
        foreach (FoodItem item in m_foodItems)
        {
            sumOfPopularities += item.getPopularityIndex();
        }

        float cumulativeSum = 0;
        for (int i = 0; i < m_foodItems.Count; i++)
        {
            cumulativeSum += m_foodItems[i].getPopularityIndex();
            Debug.LogWarningFormat("Item {0}, cumulative sum {1}, sumOfPopularities {2}, cumulative prob {3}", m_foodItems[i].itemName, cumulativeSum, sumOfPopularities, cumulativeSum / sumOfPopularities);
            invCdf.AddKey(cumulativeSum / sumOfPopularities, i);
        }
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
