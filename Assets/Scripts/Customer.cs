using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    private float customerSatisfactionBonus = 0;

    // the longest time the customer will wait before leaving. when this reaches
    // 0, the customer will leave.
    private float timeUntilLeavingCafe;

    // the max time that customer will wait for any singular order, this is a const.
    static private float maxWaitTimeSecondsForOrder = 2.5f * 60f;

    // the max time that customer will "eat" before potentially ordering another 
    // item, this is a const.
    static private float waitBetweenOrders = 25f;

    // this timer represents a countdown until the customer can order another item.
    // when it's 0, the customer potentially order another item.
    private float waitBetweenOrdersTimer = 0;
    #endregion

    #region Customer preferences
    [SerializeField]
    [Tooltip("True if customer enjoys sitting with others, false if they prefer to sit alone.")]
    private bool isSocial;
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

    [SerializeField]
    [Tooltip("The particle system for when the customer is satisfied")]
    private ParticleSystem m_satisfactionParticleSystem;

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

        if (this.isPaused)
        {
            return;
        }

        waitBetweenOrdersTimer = Mathf.Max(waitBetweenOrdersTimer - Time.deltaTime, 0);
        timeUntilLeavingCafe -= Time.deltaTime;
        //Debug.LogWarningFormat("Customer {0:X} with gameObject {1} has {2} seconds until leaving.", gameObject.GetInstanceID(), gameObject, timeUntilLeavingCafe);

        if (this.timeUntilLeavingCafe <= 0)
        {
            Debug.LogWarningFormat("Customer {0:X} exited the cafe", gameObject.GetInstanceID());
            this.exitCafe();
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

    #region Static Methods
    static public void setCustomerWaitTime(float waitTime)
    {
        maxWaitTimeSecondsForOrder = waitTime;
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
            // when we create a new order, we are willing to wait for it up to maxWaitTimeSecondsForOrder time.
            this.timeUntilLeavingCafe += maxWaitTimeSecondsForOrder;

            Debug.LogWarningFormat("Customer {0:X} Ordered item {1} and will wait up to {2} seconds for it before leaving", gameObject.GetInstanceID(), this.foodItemOrdered, this.timeUntilLeavingCafe);
            cc_uiController.createChatBubble(gameObject.transform, new Vector3(0, 4.75f, 0), this.foodItemOrdered.itemImage, timeUntilLeavingCafe);
        }

        if (this.foodItemOrdered.prepLocation == "coffeeMachine")
        {
            this.cc_coffeeMachine.updateCoffeeMachinePrefabList(this.foodItemOrdered, true);
        }
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
                this.waitBetweenOrdersTimer = waitBetweenOrders;

                // extend the time that the customer will stay in the cafe
                this.timeUntilLeavingCafe += this.waitBetweenOrdersTimer;

                this.foodItemConsuming = newFoodItem;
                this.cc_audioManager.PlaySoundEffect("cashRegister");
            }

            // can we change this to only show happy reaction if they're served fast enough?
            showHappyReaction();
        }
        return (newFoodItem != null);
    }

    private void handleReactionToSeating()
    {
        // TODO: If other customer(s) seated at that table like social seating, should the other customer also have reactions?
        if (CustomerSatisfaction.isCustomerSatisfiedWithSeating(m_chairSeatedIn, isSocial))
        {
            showHappyReaction();
            this.customerSatisfactionBonus += 0.1f;
        }
    }

    private void showHappyReaction()
    {
        if (m_satisfactionParticleSystem)
        {
            Debug.LogWarning("Particle sim");
            ParticleSystem particleSim = Instantiate(this.m_satisfactionParticleSystem, transform.position + new Vector3(0, 4.2f, 0), Quaternion.identity);

            // Destroy after particle sim duration + some buffer time
            Destroy(particleSim.gameObject, particleSim.main.duration + 1f);
            Debug.LogWarning("Destroyed particle sim");

            this.cc_audioManager.PlaySoundEffect("bonus");
        }
    }

    /*
     * Makes customer sit down in the given chair, with the assumption that
     * it's a valid sit action.
     */
    public void sitDown(GameObject chairGameObject, Chair chairData)
    {
        this.m_chairSeatedIn = chairData;
        chairData.useChair(isSocial);

        Vector3 characterPosition = new Vector3(chairGameObject.transform.position.x, chairData.heightOfSeat, chairGameObject.transform.position.z + (chairData.offset_z * chairData.facingDirection.z));
        float angle = Vector3.SignedAngle(this.m_direction, chairData.facingDirection, Vector3.up);

        Debug.LogFormat("Customer {4:X} Sitting down, originally had position {0} and rotation{1}, now will sit at position {2} and rotate by {3} degrees",
            gameObject.transform.position, gameObject.transform.rotation.eulerAngles, characterPosition, angle, gameObject.GetInstanceID());

        this.setState(AnimationState.SITTING);
        gameObject.transform.Rotate(0, angle, 0);
        gameObject.transform.position = characterPosition;

        handleReactionToSeating();
    }

    public float getTimeWaited()
    {
        return (Time.realtimeSinceStartup - this.timeInstantiated);
    }

    public bool shouldExitCafe()
    {
        // TODO: implement
        return false;
    }

    public void exitCafe()
    {
        if (this.foodItemConsuming)
        {
            Destroy(this.foodItemConsuming.gameObject);
        }

        if (this.m_chairSeatedIn)
        {
            this.m_chairSeatedIn.leaveChair(isSocial);
        }

        this.pushCustomerStats();

        if (m_destroyExplosion)
        {
            ParticleSystem particleSim = Instantiate(this.m_destroyExplosion, transform.position, Quaternion.identity);

            // Destroy after particle sim duration + some buffer time
            Destroy(particleSim.gameObject, particleSim.main.duration + 1f);
            Debug.LogWarning("Destroyed smoke particle sim");
        }

        this.cc_spawnController.RemoveCustomer(gameObject);

        Destroy(this.gameObject);
    }

    public float getCustomerSatisfaction()
    {
        return CustomerSatisfaction.calculateCustomerSatisfaction(
            this.totalItemsOrdered, waitBetweenOrders, maxWaitTimeSecondsForOrder, getTimeWaited(), this.customerSatisfactionBonus);
    }

    public void pushCustomerStats()
    {
        int totalItemsServed = getTotalItemsServed();
        bool served = (totalItemsServed == 0) ? false : true;

        
        Stats.addCustomerStats(new CustomerStats(getCustomerSatisfaction(), totalItemsServed, served));
    }

    private int getTotalItemsServed()
    {
        // Calculate if the customer has been served
        int totalItemsServed = this.totalItemsOrdered;
        if (this.foodItemOrdered != null)
        {
            // we have ordered something that we have not received yet
            totalItemsServed = this.totalItemsOrdered - 1;
        }
        return totalItemsServed;
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
