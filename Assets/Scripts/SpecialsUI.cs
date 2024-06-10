using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialsUI : MonoBehaviour
{

    private Menu cc_menu;
    private GameObject cc_uiParent;
    private GameObject cc_cafeWall;
    private Button cc_nextButton;
    private GameObject cc_specialImage;

    private FoodItem m_currentSpecial;
    private int m_specialPrice = 0;

    void Awake()
    {
        cc_menu = GameObject.Find("menu").GetComponent<Menu>();
        Debug.Assert(cc_menu != null);

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

    // Update is called once per frame
    void Update()
    {
        
    }

    void populateSpecialOptions()
    {
        List<FoodItem> options = cc_menu.getNUniqueRandomItemsFromMenu(6);
        // Populates the potential special options from the menu
        for (int i = 0; i < options.Count; i++)
        {
            GameObject foodDisplayOnWall = cc_cafeWall.transform.Find("FoodItem" + (i + 1)).gameObject;
            Debug.Assert(foodDisplayOnWall != null);

            foodDisplayOnWall.GetComponent<Image>().sprite = options[i].itemImage;

            Button btn = foodDisplayOnWall.GetComponent<Button>();
            Debug.Assert(btn != null);

            btn.onClick.AddListener(delegate { selectNewSpecial(options[i]); });
        }
    }

    private void getChangedPriceInput(int price)
    {
        m_specialPrice = price;
    }

    void selectNewSpecial(FoodItem item)
    {
        Debug.Assert(item != null);

        Image img = cc_specialImage.GetComponent<Image>();
        Debug.Assert(img != null);
        img.sprite = item.itemImage;

        m_currentSpecial = item;
    }

    private void saveSpecial()
    {
        Debug.Assert(m_currentSpecial != null);
        Debug.Assert(m_specialPrice != 0);
        cc_menu.changePrice(m_currentSpecial, m_specialPrice);
    }
}
