using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{

    #region
    private CameraController cc_cameraController;
    private GameController cc_gameController;
    #endregion

    [SerializeField]
    [Tooltip("The chatBubble prefab")]
    public GameObject chatBubbleGameObj;

    private TextMeshProUGUI moneyTextMesh;
    private TextMeshProUGUI hintTextMesh;
    private Slider timeSlider;
    private Slider satisfactionSlider;
    private Button startDayButton;

    private static Color redColor = new Color(0.547f, 0.167f, 0.167f);
    private static Color greenColor = new Color(0.167f, 0.443f, 0.102f);

    public void Awake()
    {

        GameObject cameraController = GameObject.Find("CameraController");
        cc_cameraController = cameraController.GetComponent<CameraController>();

        GameObject gameController = GameObject.Find("GameController");
        cc_gameController = gameController.GetComponent<GameController>();

        startDayButton = transform.Find("StartDayButton").GetComponent<Button>();
        startDayButton.onClick.AddListener(startDay);

        Transform topbar = transform.Find("topbar");
        this.moneyTextMesh = topbar.Find("MoneyUI").Find("MoneyTMP").GetComponent<TextMeshProUGUI>();
        Debug.Assert(this.moneyTextMesh != null, "Must find textmesh to represent money");

        this.timeSlider = topbar.Find("TimeSlider").GetComponent<Slider>();
        Debug.Assert(this.timeSlider != null, "Must find timeSlider");

        this.satisfactionSlider = topbar.Find("SatisfactionSlider").GetComponent<Slider>();
        Debug.Assert(this.satisfactionSlider != null, "Must find satisfactionSlider");

        this.hintTextMesh = transform.Find("HintText").GetComponent<TextMeshProUGUI>();
        Debug.Assert(this.hintTextMesh != null, "Must find textmesh to represent hint text");
        this.hintTextMesh.gameObject.SetActive(false);
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startDay()
    {
        Debug.Log("Starting day");
        cc_cameraController.changeActiveCamera("far camera");
        startDayButton.gameObject.SetActive(false);

        this.cc_gameController.openCafe();
    }

    public void updateDayText(int level)
    {
        TextMeshProUGUI startButtonTMP = startDayButton.transform.GetComponentInChildren<TextMeshProUGUI>();
        startButtonTMP.text = "start day " + level.ToString();
    }

    public void createChatBubble(Transform parent, Vector3 localPosition, Sprite img, float waitTimeInSeconds)
    {
        GameObject newChatBubble = Instantiate(chatBubbleGameObj, parent);
        newChatBubble.transform.localPosition = localPosition;
        newChatBubble.GetComponent<ChatBubble>().updateSprite(img);
        newChatBubble.GetComponent<ChatBubble>().StartVisualCountDown(waitTimeInSeconds);
    }

    public void clearChatBubble(Transform parent)
    {
        Transform chatBubble = parent.Find("ChatBubble(Clone)");
        Debug.Assert(chatBubble != null, "Could not find chat bubble in parent.");
        Destroy(chatBubble.gameObject);
    }

    public void updateMoneyUI()
    {
        this.moneyTextMesh.text = "$" + GameManager.Instance.getPlayerMoneyAmount().ToString();
    }

    public void flashHintText(string hintText)
    {
        if (hintText != "")
        {
            this.hintTextMesh.text = hintText;
            this.hintTextMesh.gameObject.SetActive(true);
            StartCoroutine(removeTextAfterXTime(this.hintTextMesh, 5f));
        }
    }

    private IEnumerator removeTextAfterXTime(TextMeshProUGUI TmpObj, float waitTimeInSeconds)
    {
        yield return new WaitForSeconds(waitTimeInSeconds);
        TmpObj.gameObject.SetActive(false);
    }

    /*
     * Slider val must be between 0 and 1. 1 Indicates
     * full slider, 0 is empty.
     */
    public void updateTimeSlider(float sliderVal)
    {
        Debug.Assert(sliderVal >= 0);
        this.timeSlider.value = Mathf.Min(0.9f, Mathf.Lerp(0, 0.9f, sliderVal));
    }

    public void showDayCompleteUI()
    {
        Debug.LogWarning("showing ui");
        Transform Window = transform.Find("DayCompleteWindow");
        Debug.Assert(Window != null);

        Window.gameObject.SetActive(true);

        Transform satisfactionSlide = Window.Find("Customer Satisfaction Slide");
        Debug.Assert(satisfactionSlide != null);

        this.showHidePopUpWindow(satisfactionSlide, true);

        AudioManager.Instance.PlaySoundEffectThenResumeBackgroundMusic("dayEndMusic");

        //Transform Button = satisfactionSlide.Find("NextButton");
        //Debug.Assert(Button != null);
        //Button.GetComponent<Button>().onClick.AddListener(this.changeDayCompleteUItoMoneySlide);

        // Change the customers served stats on UI
        Transform customersServed = satisfactionSlide.Find("CustomersDynamic");
        Debug.Assert(customersServed != null);
        updateTextOnTMP(customersServed, Stats.queryTodayCustomersServed().ToString());

        // Change the orders served stats on UI
        Transform ordersCompleted = satisfactionSlide.Find("OrdersDynamic");
        Debug.Assert(ordersCompleted != null);
        updateTextOnTMP(ordersCompleted, Stats.queryTodayItemsServed().ToString());

        Debug.LogWarning(Stats.queryTodayCustomerSatisfaction());
        StartCoroutine(customerSatisfactionHeartsAnimation(satisfactionSlide, Stats.queryTodayCustomerSatisfaction(), 2f));
    }

    private void updateTextOnTMP(Transform textTransform, string text)
    {
        TextMeshProUGUI TMP = textTransform.GetComponent<TextMeshProUGUI>();
        Debug.Assert(textTransform != null);
        TMP.text = text;
    }

    private void updateTextColorOnTMP(Transform textTransform, Color color)
    {
        TextMeshProUGUI TMP = textTransform.GetComponent<TextMeshProUGUI>();
        Debug.Assert(textTransform != null);
        TMP.color = color;
    }

    public void changeDayCompleteUItoMoneySlide()
    {
        AudioManager.Instance.PlaySoundEffect("softBeep");

        Debug.LogWarning("Changed to money slide");
        Transform Window = transform.Find("DayCompleteWindow");
        Debug.Assert(Window != null);

        Transform satisfactionSlide = Window.Find("Customer Satisfaction Slide");
        Debug.LogWarning("Changed to money slide");

        this.showHidePopUpWindow(satisfactionSlide, false);

        Transform moneySlide = Window.Find("Money Window");
        Debug.Assert(moneySlide != null);
        this.showHidePopUpWindow(moneySlide, true);

        Transform revenue = moneySlide.Find("RevenueDynamic");
        Debug.Assert(revenue != null);
        updateTextOnTMP(revenue, "$" + Stats.queryTodayMoneyMade().ToString());

        Transform costs = moneySlide.Find("CostDynamic");
        Debug.Assert(costs != null);
        updateTextOnTMP(costs, "$" + Stats.queryLostMoney().ToString());
        GameManager.Instance.subtractFromPlayerMoneyAmount(Stats.queryLostMoney());

        Transform profit = moneySlide.Find("ProfitDynamic");
        Debug.Assert(profit != null);
        int profitInt = Stats.queryTodayMoneyMade() - Stats.queryLostMoney();
        if (profitInt < 0)
        {
            updateTextColorOnTMP(profit, redColor);
            updateTextOnTMP(profit, "-$" + profitInt.ToString().TrimStart('-'));
        } else
        {
            updateTextColorOnTMP(profit, greenColor);
            updateTextOnTMP(profit, "$" + profitInt.ToString());
        }

        Transform totalMoney = moneySlide.Find("TotalMoneyDynamic");
        Debug.Assert(totalMoney != null);
        updateTextOnTMP(totalMoney, "$" + GameManager.Instance.getPlayerMoneyAmount());

        Button nextButton = moneySlide.Find("NextButton").GetComponent<Button>();
        Debug.Assert(nextButton != null);
        nextButton.onClick.AddListener(SceneController.Instance.GoToFeedback);
    }

    private IEnumerator customerSatisfactionHeartsAnimation(Transform Window, float customerSatisfaction, float timeInSecForWholeAnim)
    {
        Transform heartsMask = Window.Find("hearts mask");
        Debug.Assert(heartsMask != null);

        Transform gameFill = heartsMask.Find("satisfactionHeartsFill");
        Debug.Assert(gameFill != null);

        float tInterval = 0.1f;
        for (float t = 0; t < 1; t += tInterval)
        {
            gameFill.localScale = new Vector3(Mathf.Lerp(0, customerSatisfaction, t), gameFill.localScale.y, gameFill.localScale.z);
            yield return new WaitForSeconds((float)(timeInSecForWholeAnim * tInterval));
        }
    }

    private void showHidePopUpWindow(Transform window, bool show)
    {
        if (window)
        {
            Utils.SetThisAndAllDescendantsActiveRecursive(window.gameObject, show);
        }
        Transform overlay = transform.Find("darkoverlay");
        if (overlay)
        {
            overlay.gameObject.SetActive(show);
        }
    }

    public void updateSatisfactionSlider()
    {
        float satisfaction = Stats.queryRealTimeAvgCustomerSatisfaction();
        satisfaction = Mathf.Lerp(0, 0.9f, satisfaction);

        Debug.Assert(satisfaction >= 0 && satisfaction <= 1);

        Transform fill = satisfactionSlider.gameObject.transform.Find("Fill Area").Find("Fill");
        Image img = fill.GetComponent<Image>();

        if (satisfaction > 0.3)
        {
            img.color = new Color(0.5566038f, 0.4069509f, 0.4069509f);

        } else
        {
            img.color = Color.red;
        }
        satisfactionSlider.value = satisfaction;
    }

}
