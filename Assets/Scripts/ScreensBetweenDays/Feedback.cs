using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum SpecialItemCategory
{
    COZY = 0,
    BERRIES = 1,
    FANCY = 2,
    BREAKFAST = 3,
    SWEETS = 4
}

/// <summary>
/// 
/// This class manages the generated customer feedback.
/// It will also influence the popularity of items for the next
/// day according to the generated feedback.
///
/// This class is only used in the Feedback scene.
/// 
/// </summary>
/// 
public class Feedback : MonoBehaviour
{
    //List<FeedbackItem> m_feedbackPool = new List<FeedbackItem>();
    Dictionary<SpecialItemCategory, string[]> hintStringsByCategory = new Dictionary<SpecialItemCategory, string[]>();

    private static int m_feedbackBubbleCount = 2;

    #region Cached Components
    private GameObject cc_firstFeedbackBubble;
    private GameObject cc_secondFeedbacKBubble;
    #endregion

    [SerializeField]
    public GameObject NextButtonGameObj;

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

        this.cc_firstFeedbackBubble = gameObject.transform.Find("FeedbackBubbleRight").gameObject;
        this.cc_secondFeedbacKBubble = gameObject.transform.Find("FeedbackBubbleLeft").gameObject;

        Debug.Assert(cc_firstFeedbackBubble);
        Debug.Assert(cc_secondFeedbacKBubble);
    }

    void Start()
    {
        int numCategories = Enum.GetNames(typeof(SpecialItemCategory)).Length;
        SpecialItemCategory popularCategory = (SpecialItemCategory) UnityEngine.Random.Range(0, numCategories);

        Debug.LogWarningFormat("Selected popular item category as {0}", Enum.GetName(typeof(SpecialItemCategory), popularCategory));

        foreach (FoodItem foodItem in Menu.Instance.listMenuFoodItems())
        {
            foodItem.boostPopularityIfApplicable(popularCategory);
        }

        int i = 0;
        foreach (string s in hintStringsByCategory[popularCategory])
        {
            AddFeedbackBubbleToScreen(s, i);
            i++;
        }
        if (i < m_feedbackBubbleCount)
        {
            AddNGenericFeedback(m_feedbackBubbleCount - i, i);
        }

        Button nextButton = NextButtonGameObj.GetComponent<Button>();
        Debug.Assert(nextButton != null);
        nextButton.onClick.AddListener(SceneController.Instance.GoToSpecialSelection);
    }

    /*
     * Adds count new generic feedback messages, starting by modifying the 
     * nthFeedbackBubble out of the m_feedbackBubbleCount available bubbles.
     * 
     * This method is designed to handle if we have more than 2 feedback bubbles.
     */
    private void AddNGenericFeedback(int count, int nthFeedbackBubble)
    {
        List<int> indices = new List<int>();

        while (indices.Count < count)
        {
            int index = UnityEngine.Random.Range(0, genericFeedback.Length);
            if (!indices.Contains(index))
            {
                indices.Add(index);
                AddFeedbackBubbleToScreen(genericFeedback[index], nthFeedbackBubble);
            }
            nthFeedbackBubble++;
        }
    }

    /*
     * Mutates the UI by adding the string S to the available UI feedback bubbles.
     * nthFeedbackBubble will modify the according bubble in order from top to bottom
     * of the screen, where there are at most m_feedbackBubbleCount UI elements
     * that we can modify.
     */
    private void AddFeedbackBubbleToScreen(string s, int nthFeedbackBubble)
    {
        if (nthFeedbackBubble >= m_feedbackBubbleCount)
        {
            Debug.LogWarningFormat("Only {0} feedback bubbles can be populated. Attempted to add a {1}th bubble.", m_feedbackBubbleCount, nthFeedbackBubble);
            return;
        }

        TextMeshProUGUI feedbackTMP = null;
        if (nthFeedbackBubble == 0)
        {
            feedbackTMP = cc_firstFeedbackBubble.transform.Find("feedback").GetComponent<TextMeshProUGUI>();
            Debug.Assert(feedbackTMP != null);
            feedbackTMP.text = s;
        }
        else if (nthFeedbackBubble == 1)
        {
            feedbackTMP = cc_secondFeedbacKBubble.transform.Find("feedback").GetComponent<TextMeshProUGUI>();
        }

        Debug.Assert(feedbackTMP != null);
        feedbackTMP.text = s;

        Debug.LogWarningFormat("Added feedback string {0} to {1}th bubble", s, nthFeedbackBubble);
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