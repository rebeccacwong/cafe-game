using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : CharacterBase, IDraggableObject
{

    #region Instance variables
    private Vector3 m_frontOfLinePos;
    private Vector3 m_screenPointOfCharacter;
    private bool m_isBeingDragged;
    private bool m_isSeated;
    private Vector3 m_lastPositionBeforeDragging;
    #endregion

    #region Customer experience variables
    private float m_Satisfaction = 1;
    private int itemsToOrder;
    private float runningBill = 0;
    private FoodItem foodItemOrdered;
    private float timeInstantiated;

    // the longest time the customer will wait before leaving
    private float maxWaitTime;
    #endregion

    #region Cached components
    private SpawnController cc_spawnController;
    private Menu cc_menu;
    #endregion

    [SerializeField]
    [Tooltip("The number of frames for the character's sit animation")]
    private int m_numFramesForSitAnim;


    #region Overrides
    protected override void Awake()
    {
        base.Awake();

        // get cached components
        cc_spawnController = GameObject.Find("CustomerSpawner").GetComponent<SpawnController>();
        cc_menu = GameObject.Find("menu").GetComponent<Menu>();

        // populate other instance variables
        this.cc_rigidBody = gameObject.GetComponent<Rigidbody>();
        this.cc_animator = gameObject.GetComponent<Animator>();
        this.m_isSeated = false;
        this.itemsToOrder = UnityEngine.Random.Range(1, 5);
        this.maxWaitTime = 5 * 60; // currently set at 5 min
        this.timeInstantiated = Time.realtimeSinceStartup; // real time in seconds since game started
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        this.m_frontOfLinePos = cc_spawnController.spawnPosition;
        this.m_frontOfLinePos.z = 13f;

        this.m_direction = new Vector3(0, 0, 1);
        this.m_forwards = new Vector3(0, 0, 1);

        // Move to the counter
        this.setNewTarget(this.m_frontOfLinePos, true, true);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (Time.realtimeSinceStartup - this.timeInstantiated > this.maxWaitTime)
        {
            this.exitCafe();
        }

        if (this.isPaused)
        {
            return;
        }

        if (!this.m_isSeated)
        {
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
    /*
     * Make a request to order an item
     */
    public void orderItem()
    {
        // Pick a random menu item
        if (this.itemsToOrder <= 0)
        {
            return;
        }

        this.foodItemOrdered = this.cc_menu.returnRandomItemFromMenu();
        if (this.foodItemOrdered)
        {
            // TODO: put the item in the speech bubble
            this.itemsToOrder -= 1;
        }
    }

    /*
     * This is called after the main character
     * serves the ordered item.
     */
    public void acceptItem(FoodItem servedItem)
    {
        if (servedItem.itemName == this.foodItemOrdered.itemName)
        {
            this.runningBill += this.foodItemOrdered.itemPrice;
            this.foodItemOrdered = null;

            // clear speech bubble
        }
    }

    /*
     * Makes customer sit down in the given chair, with the assumption that
     * it's a valid sit action.
     */
    public void sitDown(GameObject chairGameObject, Chair chairData)
    {
        this.m_isSeated = true;
        chairData.inUse = true;

        Vector3 characterPosition = new Vector3(chairGameObject.transform.position.x, chairData.heightOfSeat, chairGameObject.transform.position.z + (chairData.offset_z * chairData.facingDirection.z));
        float angle = Vector3.SignedAngle(this.m_direction, chairData.facingDirection, Vector3.up);

        Debug.LogWarningFormat("Sitting down, originally had position {0} and rotation{1}, now will sit at position {2} and rotate by {3} degrees",
            gameObject.transform.position, gameObject.transform.rotation.eulerAngles, characterPosition, angle);

        this.setState(AnimationState.SITTING);
        gameObject.transform.Rotate(0, angle, 0);
        gameObject.transform.position = characterPosition;
        //StartCoroutine(sitCoroutine(characterPosition, angle));
    }

    public void exitCafe()
    {
        // add the payment so far to the money bank
        Destroy(this);
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
        if (this.m_isSeated)
        {
            return false;
        }

        this.pauseAnimation();
        this.isBeingDragged = true;
        this.m_lastPositionBeforeDragging = gameObject.transform.position;
        Debug.LogWarningFormat("Start dragging customer {0} with id {1}", gameObject, gameObject.GetInstanceID());

        this.cc_rigidBody.isKinematic = false;
        this.m_screenPointOfCharacter = this.cc_CameraController.getActiveCamera().WorldToScreenPoint(gameObject.transform.position);
        return true;
    }

    public void stopDraggingObject()
    {
        this.unpauseAnimation();

        // do nothing if character is already seated
        if (this.m_isSeated)
        {
            return;
        }

        this.isBeingDragged = false;
        this.cc_rigidBody.isKinematic = true;
        Debug.LogWarningFormat("Stop dragging customer {0} with id {1}", gameObject, gameObject.GetInstanceID());

        // check if we're intersecting with a chair or table and we should it there
        GameObject collisionObj = Utils.returnObjectMouseIsOn(LayerMask.GetMask("Chairs"));
        if (collisionObj != null)
        {
            if (collisionObj.tag == "Chair")
            {
                Debug.LogWarning("collided with a chair");
                Chair chairData = collisionObj.GetComponent<Chair>();
                Debug.Assert(chairData != null, "All chair gameObjects must have a Chair script");

                if (chairData.inUse)
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
                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.m_screenPointOfCharacter.z);
                Vector3 curPosition = this.cc_CameraController.getActiveCamera().ScreenToWorldPoint(curScreenPoint);
                curPosition.y -= gameObject.GetComponent<BoxCollider>().size.y / 2;
                curPosition.y = Math.Max(curPosition.y, 0); // don't let character go through the floor
                gameObject.transform.position = curPosition;
            }
        }
    }
    #endregion
}
