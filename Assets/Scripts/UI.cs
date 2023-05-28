using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{

    #region
    private SceneController cc_sceneController;
    private CameraController cc_cameraController;
    private GameController cc_gameController;
    private GameManager cc_gameManager;
    #endregion

    [SerializeField]
    [Tooltip("The chatBubble prefab")]
    public GameObject chatBubbleGameObj;

    private TextMeshProUGUI moneyTextMesh;
    private TextMeshProUGUI hintTextMesh;
    private Slider timeSlider;

    public void Awake()
    {
        GameObject sceneController = GameObject.Find("SceneController");
        cc_sceneController = sceneController.GetComponent<SceneController>();

        GameObject cameraController = GameObject.Find("CameraController");
        cc_cameraController = cameraController.GetComponent<CameraController>();

        //GameObject customerSpawner = GameObject.Find("CustomerSpawner");
        //cc_spawnController = customerSpawner.GetComponent<SpawnController>();

        GameObject gameController = GameObject.Find("GameController");
        cc_gameController = gameController.GetComponent<GameController>();

        GameObject gameManager = GameObject.Find("GameManager");
        cc_gameManager = gameManager.GetComponent<GameManager>();

        transform.Find("StartDayButton").GetComponent<Button>().onClick.AddListener(startDay);

        Transform topbar = transform.Find("topbar");
        this.moneyTextMesh = topbar.Find("MoneyUI").Find("MoneyTMP").GetComponent<TextMeshProUGUI>();
        Debug.Assert(this.moneyTextMesh != null, "Must find textmesh to represent money");

        this.timeSlider = topbar.Find("Slider").GetComponent<Slider>();
        Debug.Assert(this.timeSlider != null, "Must find timeSlider");

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
        GameObject.Find("Canvas/StartDayButton").GetComponent<Button>().gameObject.SetActive(false);

        this.cc_gameController.openCafe();
    }

    public void createChatBubble(Transform parent, Vector3 localPosition, Sprite img)
    {
        GameObject newChatBubble = Instantiate(chatBubbleGameObj, parent);
        newChatBubble.transform.localPosition = localPosition;
        newChatBubble.GetComponent<ChatBubble>().updateSprite(img);
    }

    public void clearChatBubble(Transform parent)
    {
        Transform chatBubble = parent.Find("ChatBubble(Clone)");
        Debug.Assert(chatBubble != null, "Could not find chat bubble in parent.");
        Destroy(chatBubble.gameObject);
    }

    public void updateMoneyUI()
    {
        this.moneyTextMesh.text = "$" + cc_gameManager.getPlayerMoneyAmount().ToString();
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
        this.timeSlider.value = Mathf.Min(1, sliderVal);
    }

}
