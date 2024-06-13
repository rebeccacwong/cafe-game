using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpecialItemCategory
{
    COZY = 0,
    BERRIES = 1,
    FANCY = 2,
    BREAKFAST = 3,
    SWEETS = 4
}

public class Feedback : MonoBehaviour
{
    //List<FeedbackItem> m_feedbackPool = new List<FeedbackItem>();
    Dictionary<SpecialItemCategory, string[]> hintStringsByCategory = new Dictionary<SpecialItemCategory, string[]>();

    private static int m_feedbackBubbleCount = 3;

    #region Feedback Strings
    private string[] cozyArray = {
        "It's cold out! I'd be nice to enjoy a warm drink somewhere.",
        "This place is so cute! I'm always looking for a place to cozy up with a coffee and a good book."
    };

    private string[] berriesArray = { "This cafe has such a good location! I'm going berry picking nearby now that they’re in season. I love berries!" };

    private string[] fancyArray = { "I would love a place with some fancy french sweets!" };

    private string[] breakfastArray = { "Dropped by Pandy’s Parlour near the end of the day, I’d love to come back tomorrow for breakfast!" };

    private string[] sweetsArray = { "I love that Pandy's Parlour has desserts! I have such a sweet tooth." };

    private string[] genericFeedback =
    {
        "I dropped by Pandy's Parlour yesterday. The coffee met my expectations.",
        "I love the variety that the cafe has! Lots of things to choose from.",
        "It's a nice place to sit with friends and chat. I will be bringing my friends next time I visit!",
        "Pandy's Parlour has such a great view! I listened to some music while sitting by the window.",
        "The owner, Pandy, seems nice! The cafe was a bit busy though so we didn't get to talk long.",
        "I wish the service was better. I hate waiting in lines!",
        "Some of the things on the menu were overpriced, but it was still decent.",
        "Pretty good spot for a solo visit. Plenty of places to sit."
    };
    #endregion

    private void Awake()
    {
        hintStringsByCategory.Add(SpecialItemCategory.COZY, cozyArray);
        hintStringsByCategory.Add(SpecialItemCategory.BERRIES, berriesArray);
        hintStringsByCategory.Add(SpecialItemCategory.FANCY, fancyArray);
        hintStringsByCategory.Add(SpecialItemCategory.BREAKFAST, breakfastArray);
        hintStringsByCategory.Add(SpecialItemCategory.SWEETS, sweetsArray);
    }

    // Start is called before the first frame update
    void Start()
    {
        int numCategories = Enum.GetNames(typeof(SpecialItemCategory)).Length;
        SpecialItemCategory popularCategory = (SpecialItemCategory) UnityEngine.Random.Range(0, numCategories);

        foreach (FoodItem foodItem in Menu.Instance.listMenuFoodItems())
        {
            foodItem.boostPopularityIfApplicable(popularCategory);
        }

        // TODO: Generate the feedback and show it in the UI
        int i = 0;
        foreach (string s in hintStringsByCategory[popularCategory])
        {
            AddFeedbackBubbleToScreen(s);
            i++;
        }
        if (i < m_feedbackBubbleCount)
        {
            // TODO: create some generic strings and populate the rest of the bubbles with that
        }
    }

    private void AddFeedbackBubbleToScreen(string s)
    {
        // TODO: Implement
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

//public struct FeedbackItem
//{
//    public readonly FoodItem[] popularItems;
//    public readonly string feedbackMessage;
//    public readonly SpecialItemCategory category;

//    public FeedbackItem(FoodItem[] items, string msg)
//    {
//        popularItems = items;
//        feedbackMessage = msg;
//    }
//}