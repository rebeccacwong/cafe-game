using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// This class manages the UI interaction for when the customer
/// is picking the special of the day and the pricing for it.
///
/// This class is only used in the BetweenDays scene.
/// 
/// </summary>
public class SpecialsUI : MonoBehaviour
{

    private GameObject cc_uiParent;
    private GameObject cc_cafeWall;
    private Button cc_nextButton;
    private GameObject cc_specialImage;

    private FoodItem m_currentSpecial;
    private int m_specialPrice = 0;

    private void Awake()
    {
        cc_uiParent = GameObject.Find("Canvas/DailySpecialSelection");
        Debug.Assert(cc_uiParent != null);

        cc_cafeWall = cc_uiParent.transform.Find("cafe wall").gameObject;
        Debug.Assert(cc_cafeWall != null);

        // next button starts disabled until we have added appropriate input
        cc_nextButton = cc_uiParent.transform.Find("next").gameObject.GetComponent<Button>();
        Debug.Assert(cc_nextButton != null);
        cc_nextButton.interactable = false;

        cc_specialImage = cc_uiParent.transform.Find("special image").gameObject;
        Debug.Assert(cc_specialImage != null);
    }

    private void Start()
    {
        populateSpecialOptions();
    }

    private void populateSpecialOptions()
    {
        Debug.LogWarning("Populating special options.");

        List<FoodItem> options = Menu.Instance.getNUniqueRandomItemsFromMenu(6);
        // Populates the potential special options from the menu
        for (int i = 0; i < options.Count; i++)
        {
            FoodItem item = options[i];
            GameObject foodDisplayOnWall = cc_cafeWall.transform.Find("FoodItem" + (i + 1)).gameObject;
            Debug.Assert(foodDisplayOnWall != null);

            item.resetPopularity();

            foodDisplayOnWall.GetComponent<Image>().sprite = item.itemImage;

            Button btn = foodDisplayOnWall.GetComponent<Button>();
            Debug.Assert(btn != null);

            btn.onClick.AddListener(delegate { selectNewSpecial(item); });
        }
    }

    public void getChangedPriceInput(string price)
    {
        int priceInt = 0;
        int.TryParse(price, out priceInt);
        if (priceInt == 0)
        {
            Debug.LogWarning("Invalid price input of 0 received. Player should try another number between 1-9.");
            this.cc_nextButton.interactable = false;
            return;
        }

        Debug.LogWarningFormat("Updating special price to {0}", priceInt);
        m_specialPrice = priceInt;

        if (m_currentSpecial != null)
        {
            this.cc_nextButton.interactable = true;
        }
    }

    public void selectNewSpecial(FoodItem item)
    {
        Debug.Assert(item != null);

        Debug.LogWarningFormat("Selecting item {0} as special", item.itemName);

        Image img = cc_specialImage.GetComponent<Image>();
        Debug.Assert(img != null);
        img.sprite = item.itemImage;

        m_currentSpecial = item;

        if (m_specialPrice != 0)
        {
            this.cc_nextButton.interactable = true;
        }
    }

    public void saveSpecial()
    {
        Debug.Assert(m_currentSpecial != null);
        Debug.Assert(m_specialPrice != 0);

        Debug.LogWarningFormat("Saving special: {0} at ${1}", m_currentSpecial.itemName, m_specialPrice);
        Menu.Instance.changePrice(m_currentSpecial, m_specialPrice);
    }

    public void nextButtonAfterSpecialSelection()
    {
        SceneController sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
        Debug.Assert(sceneController != null);

        sceneController.StartNewDay();
    }
}
