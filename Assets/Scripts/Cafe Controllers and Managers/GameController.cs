using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 
/// The GameController manages the game behavior within the scope
/// of one cafe day.
///
/// The GameController will start the day (open the cafe), manage
/// aggregate satisfaction, time, and other day specific data,
/// and ensure that at the end of the day the proper UI
/// summarizing the day's data is presented.
/// 
/// </summary>
[DisallowMultipleComponent]
public class GameController : MonoBehaviour
{
    // the object that the mouse is currently carrying
    private IDraggableObject m_currentlyCarrying = null;

    #region Cached components
    private SpawnController cc_spawnController;
    private UI cc_uiController;
    #endregion

    [SerializeField, Range(0, 24)]
    public float timeOfDay;

    private static float startDayTime = 5f;
    private static float lastCustomerTime = 20f;
    private static float endDayTime = 24f;
    private bool m_timePaused = true;
    private bool m_isCafeOpen = false;
    private bool lastCallOccurred = false;

    private void Awake()
    {
        this.timeOfDay = startDayTime;
        cc_spawnController = GameObject.Find("CustomerSpawner").GetComponent<SpawnController>();
        cc_uiController = GameObject.Find("Canvas").GetComponent<UI>();
    }

    private void Start()
    {
        int level = GameManager.Instance.InitializeNewLevel();
        cc_uiController.updateDayText(level);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isCafeOpen)
        {
            return;
        }

        if (!m_timePaused)
        {
            timeOfDay += Time.deltaTime * 0.08f;
            cc_uiController.updateTimeSlider((timeOfDay - startDayTime) / (endDayTime - startDayTime));
        }

        if (!this.lastCallOccurred && timeOfDay >= lastCustomerTime)
        {
            cc_spawnController.StopSpawningCustomers();
            this.lastCallOccurred = true;
        }

        if (timeOfDay >= endDayTime)
        {
            closeCafe();
            // change the sun UI to moon image
            // TODO: wait until all customers have left before adding the close cafe UI
        }

        cc_uiController.updateSatisfactionSlider();

        // Poll for clicks
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Utils.returnObjectMouseIsOn(LayerMask.GetMask("IInteractables"));
            if (obj != null)
            {
                IDraggableObject draggableObject = obj.GetComponent<IDraggableObject>();
                m_currentlyCarrying = draggableObject;

                if (draggableObject != null)
                {
                    bool isDraggable = draggableObject.startDraggingObject();
                    if (isDraggable)
                    {
                        if (obj.tag == "Customer")
                        {
                            Debug.Log("Pausing all pausable objects");
                            this.m_timePaused = true;

                            foreach (IPausable pausableObj in getAllPausableObjects())
                            {
                                // Pause every object except for the customer
                                if (pausableObj.GetPausableGameObject() != obj)
                                {
                                    pausableObj.Pause();
                                }
                            }

                            cc_spawnController.PauseCustomerSpawning();
                        }
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) && m_currentlyCarrying != null)
        {
            m_currentlyCarrying.stopDraggingObject();
            m_currentlyCarrying = null;

            cc_spawnController.ResumeCustomerSpawning();

            Debug.Log("Unpausing all pausable objects");
            this.m_timePaused = false;
            foreach (IPausable pausableObj in getAllPausableObjects())
            {
                if (pausableObj.isPaused)
                {
                    pausableObj.Unpause();
                }
            }

        }
    }

    private IPausable[] getAllPausableObjects()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IPausable>().ToArray();
    }

    public void openCafe()
    {
        Debug.Log("Starting day");
        cc_spawnController.StartSpawningCustomers();
        this.m_timePaused = false;
        this.m_isCafeOpen = true;

        Stats.clearStatsForDay();
        cc_uiController.updateMoneyUI();
    }

    public void closeCafe()
    {
        Debug.Log("Ending day");

        this.m_isCafeOpen = false;
        this.m_timePaused = true;

        // Pause everything
        foreach (IPausable pausableObj in getAllPausableObjects())
        {
            pausableObj.Pause();
        }

        cc_spawnController.RemoveAllCustomers();

        cc_uiController.showDayCompleteUI();
    }
}
