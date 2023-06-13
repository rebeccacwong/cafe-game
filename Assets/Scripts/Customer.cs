using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Customer : CharacterBase, IDraggableObject, IInteractable
{

    #region Instance variables
    private Vector3 m_frontOfLinePos;
    private Vector3 m_screenPointOfCharacter;
    private bool m_isBeingDragged;
    private Vector3 m_lastPositionBeforeDragging;
    private Chair m_chairSeatedIn;
    #endregion

    #region Customer experience variables
    private FoodItem foodItemOrdered;
    private FoodItem foodItemConsuming;
    private float timeInstantiated;
    private int totalItemsOrdered = 0;

    // the longest time the customer will wait before leaving. when this reaches
    // 0, the customer will leave.
    private float timeUntilLeavingCafe;

    // the max time that customer will wait for any singular order, this is a const.
    private float maxWaitTimeSecondsForOrder = 2.5f * 60;

    // the max time that customer will "eat" before potentially ordering another 
    // item, this is a const.
    private float waitBetweenOrders = 2 * 60;

    // this timer represents a countdown until the customer can order another item.
    // when it's 0, the customer potentially order another item.
    private float waitBetweenOrdersTimer = 0;
    #endregion

    #region Cached components
    private SpawnController cc_spawnController;
    private Menu cc_menu;
    private UI cc_uiController;
    private GameManager cc_gameManager;
    private coffeeMachine cc_coffeeMachine;
    #endregion

    [SerializeField]
    [Tooltip("The number of frames for the character's sit animation")]
    private int m_numFramesForSitAnim;

    [SerializeField]
    [Tooltip("The explosion that occurs when customer gameobject is destroyed")]
    private ParticleSystem m_destroyExplosion;

    #region Overrides
    protected override void Awake()
    {
        base.Awake();

        cc_spawnController = GameObject.Find("CustomerSpawner").GetComponent<SpawnController>();
        cc_uiController = GameObject.Find("Canvas").GetComponent<UI>();
        cc_menu = GameObject.Find("menu").GetComponent<Menu>();
        cc_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cc_coffeeMachine = GameObject.Find("coffee machine").GetComponent<coffeeMachine>();

        this.cc_animator = gameObject.GetComponent<Animator>();
        this.timeUntilLeavingCafe = maxWaitTimeSecondsForOrder;
        this.timeInstantiated = Time.realtimeSinceStartup; // real time in seconds since game started
    }

    protected override void Start()
    {
        this.m_frontOfLinePos = cc_spawnController.spawnPosition;
        this.m_frontOfLinePos.z = 13f;

        this.m_direction = new Vector3(0, 0, 1);
        this.m_forwards = new Vector3(0, 0, 1);

        // Move to the counter
        this.setNewTarget(this.m_frontOfLinePos, true, true);
    }

    protected override void Update()
    {
        waitBetweenOrdersTimer = Mathf.Max(waitBetweenOrdersTimer - Time.deltaTime, 0);

        if (Time.realtimeSinceStartup - this.timeInstantiated > this.timeUntilLeavingCafe)
        {
            Debug.LogWarningFormat("Customer {0:X} exited the cafe", gameObject.GetInstanceID());
            this.exitCafe();
        }

        if (this.isPaused)
        {
            return;
        }

        if (this.m_chairSeatedIn)
        {
            if (this.shouldOrderItem())
            {
                orderItem();
            }
        } else {
            this.onUpdateMoveTowardsTarget();

            /* 
             * Once the character is no longer moving (sitting), 
             * we disable dragging. The character can no longer
             * be moved by the player and must leave at its
             * own choice.
             */
            this.onUpdateDragObject();
        }
    }
    #endregion


    #region Customer methods
    private MainCharacter getMainCharacter()
    {
        return GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
    }

    /*
     * Make a request to order an item
     */
    public void orderItem()
    {
        this.totalItemsOrdered++;
        this.foodItemOrdered = this.cc_menu.returnRandomItemFromMenu();

        // if we're already eating something, destroy it
        if (this.foodItemConsuming)
        {
            Destroy(this.foodItemConsuming.gameObject);
        }
        if (this.foodItemOrdered)
        {
            cc_uiController.createChatBubble(gameObject.transform, new Vector3(0, 4.2f, 0), this.foodItemOrdered.itemImage);
        }

        if (this.foodItemOrdered.prepLocation == "coffeeMachine")
        {
            this.cc_coffeeMachine.updateCoffeeMachinePrefabList(this.foodItemOrdered, true);
        }

        // when we create a new order, we are willing to wait for it up to maxWaitTimeSecondsForOrder time.
        this.timeUntilLeavingCafe += this.maxWaitTimeSecondsForOrder;

        Debug.LogWarningFormat("Customer {0:X} Ordered item {1} and will wait up to {2} seconds for it before leaving", gameObject.GetInstanceID(), this.foodItemOrdered, this.timeUntilLeavingCafe);
    }

    /*
     * This is called after the main character
     * serves the ordered item. Returns true
     * if the customer accepted the item.
     */
    public bool acceptItem(FoodItem servedItem)
    {
        FoodItem newFoodItem = null;

        Debug.Assert(this.foodItemOrdered != null);
        Debug.Assert(servedItem != null);

        Debug.LogWarningFormat(
            "Customer {0:X} is expecting to receive {1} and got served {2}",
            gameObject.GetInstanceID(),
            servedItem.itemName,
            this.foodItemOrdered.itemName);
        if (servedItem.itemName == this.foodItemOrdered.itemName)
        {
            this.cc_gameManager.addToPlayerMoneyAmount(this.foodItemOrdered.itemPrice);
            cc_uiController.clearChatBubble(gameObject.transform);

            if (this.foodItemOrdered.prepLocation == "coffeeMachine")
            {
                this.cc_coffeeMachine.updateCoffeeMachinePrefabList(this.foodItemOrdered, false);
            }
            this.foodItemOrdered = null;

            if (newFoodItem = servedItem.InstantiateFoodItem(this.m_chairSeatedIn))
            {
                // reset the time to "eat"
                this.waitBetweenOrdersTimer = this.waitBetweenOrders;

                // extend the time that the customer will stay in the cafe
                this.timeUntilLeavingCafe += this.waitBetweenOrdersTimer;

                this.foodItemConsuming = newFoodItem;
                this.cc_audioManager.PlaySoundEffect("cashRegister");
            }
        }
        return (newFoodItem != null);
    }

    /*
     * Makes customer sit down in the given chair, with the assumption that
     * it's a valid sit action.
     */
    public void sitDown(GameObject chairGameObject, Chair chairData)
    {
        this.m_chairSeatedIn = chairData;
        chairData.useChair();

        Vector3 characterPosition = new Vector3(chairGameObject.transform.position.x, chairData.heightOfSeat, chairGameObject.transform.position.z + (chairData.offset_z * chairData.facingDirection.z));
        float angle = Vector3.SignedAngle(this.m_direction, chairData.facingDirection, Vector3.up);

        Debug.LogFormat("Customer {4:X} Sitting down, originally had position {0} and rotation{1}, now will sit at position {2} and rotate by {3} degrees",
            gameObject.transform.position, gameObject.transform.rotation.eulerAngles, characterPosition, angle, gameObject.GetInstanceID());

        this.setState(AnimationState.SITTING);
        gameObject.transform.Rotate(0, angle, 0);
        gameObject.transform.position = characterPosition;
        //this.orderItem();
        //StartCoroutine(sitCoroutine(characterPosition, angle));
    }

    public bool shouldExitCafe()
    {
        // TODO: implement
        return false;
    }

    private float calculateCustomerSatisfaction()
    {
        float avgTimeRemainingSpentOnEachOrder = (this.timeUntilLeavingCafe / this.totalItemsOrdered);
        float bestTime = maxWaitTimeSecondsForOrder * this.totalItemsOrdered + ((this.totalItemsOrdered - 1) * this.maxWaitTimeSecondsForOrder);

        return Mathf.Lerp(0, 1, (avgTimeRemainingSpentOnEachOrder / (bestTime * 0.65f)));
    }

    public void exitCafe()
    {
        if (this.foodItemConsuming)
        {
            Destroy(this.foodItemConsuming.gameObject);
        }

        this.pushCustomerStats();

        if (m_destroyExplosion)
        {
            // TODO: Finish this particle sim
            Instantiate(this.m_destroyExplosion, transform.position, Quaternion.identity);
        }

        if (this.cc_spawnController.activeCustomers > 0)
        {
            this.cc_spawnController.activeCustomers--;
        } else
        {
            Debug.LogError("Should never get here! This means that the customer counters are wrong.");
        }
        Destroy(this.gameObject);
    }

    public void pushCustomerStats()
    {
        Stats.addCustomerStats(new CustomerStats(calculateCustomerSatisfaction(), this.totalItemsOrdered));
    }

    private bool shouldOrderItem()
    {
        if (this.foodItemOrdered || waitBetweenOrdersTimer > 0)
        {
            // only order if we haven't already ordered and aren't waiting between orders
            return false;
        }

        // order a new item with 33% probability
        bool[] probabilityList = { true, false, false };
        return probabilityList[UnityEngine.Random.Range(0, probabilityList.Length)];
    }
    #endregion


    #region IDraggableObject override
    public bool isBeingDragged
    {
        get { return m_isBeingDragged; }
        set { m_isBeingDragged = value; }
    }

    public bool startDraggingObject()
    {
        // Only drag characters that are not seated
        if (this.m_chairSeatedIn)
        {
            return false;
        }

        this.pauseAnimation();
        this.isBeingDragged = true;
        this.m_lastPositionBeforeDragging = gameObject.transform.position;
        Debug.LogFormat("Start dragging customer {0} with id {1:X}", gameObject, gameObject.GetInstanceID());

        this.m_screenPointOfCharacter = this.cc_CameraController.getActiveCamera().WorldToScreenPoint(gameObject.transform.position);
        return true;
    }

    public void stopDraggingObject()
    {
        this.unpauseAnimation();

        // do nothing if character is already seated
        if (this.m_chairSeatedIn)
        {
            return;
        }

        this.isBeingDragged = false;
        Debug.LogFormat("Stop dragging customer {0} with id {1:X}", gameObject, gameObject.GetInstanceID());

        // check if we're intersecting with a chair or table and we should it there
        GameObject collisionObj = Utils.returnObjectMouseIsOn(LayerMask.GetMask("Chairs"));
        if (collisionObj != null)
        {
            if (collisionObj.tag == "Chair")
            {
                Debug.LogFormat("Collided with a chair while dragging customer {0:X}", gameObject.GetInstanceID());
                Chair chairData = collisionObj.GetComponent<Chair>();
                Debug.Assert(chairData != null, "All chair gameObjects must have a Chair script");

                if (chairData.inUse())
                {
                    gameObject.transform.position = this.m_lastPositionBeforeDragging;
                } else
                {
                    this.sitDown(collisionObj, chairData);
                }
            }
        } else
        {
            // we didn't collide with a chair. abort the drag.
            gameObject.transform.position = this.m_lastPositionBeforeDragging;
        }
    }

    public void onUpdateDragObject()
    {
        if (this.isBeingDragged)
        {
            if (Input.GetMouseButton(0))
            {
                // TODO: edit so that we get more range of motion along z axis
                // update the position of the character
                //Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.m_screenPointOfCharacter.z);
                //Vector3 curPosition = this.cc_CameraController.getActiveCamera().ScreenToWorldPoint(curScreenPoint);
                //curPosition.y -= gameObject.GetComponent<BoxCollider>().size.y / 2;
                //curPosition.y = Math.Max(curPosition.y, 0); // don't let character go through the floor
                //gameObject.transform.position = curPosition;

                Vector3 curPosition;
                Plane plane = new Plane(Vector3.up, 0);
                Ray ray = this.cc_CameraController.getActiveCamera().ScreenPointToRay(Input.mousePosition);
                float distance;
                if (plane.Raycast(ray, out distance))
                {
                    curPosition = ray.GetPoint(distance);
                    gameObject.transform.position = curPosition;
                    curPosition.y = Math.Max(curPosition.y, 0); // don't let character go through the floor
                }
            }
        }
    }
    #endregion

    #region IInteractable methods
    public void interactWithObject(GameObject optionalParam = null)
    {
        FoodItem foodItem = optionalParam.GetComponent<FoodItem>();
        if (foodItem == null)
        {
            throw new Exception("Customer interactWithObject received a parameter that is not a FoodItem.");
        }
        Debug.LogWarningFormat("Attempting to serve customer the food item:", foodItem);
        if (this.acceptItem(foodItem))
        {
            this.getMainCharacter().dropItem();
        }
    }

    public bool canInteract(out string errorString)
    {
        errorString = "";
        if (!this.getMainCharacter().isCarryingItem())
        {
            errorString = "Pick up some food/coffee to serve customers!";
        } else if (this.foodItemOrdered == null)
        {
            errorString = "Cannot serve a customer that has not ordered yet.";
        } else if (this.foodItemOrdered.itemName != this.getMainCharacter().itemBeingCarried().itemName)
        {
            errorString = "Wrong item! Customer has ordered a " + foodItemOrdered.itemName + ". Throw it away or serve to another customer.";
        } else
        {
            return true;
        }
        return false;
        //return (this.foodItemOrdered != null && this.getMainCharacter().isCarryingItem());
    }
    #endregion
}
