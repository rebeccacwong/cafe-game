using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MainCharacter : CharacterBase
{
    private float m_turnSmoothVelocity;
    private FoodItem m_currentlyCarrying = null;

    private static float interactRange = 3f;
    private static Vector3 handLocalPosition = new Vector3(-1.17f, 1.23f, 0.06f);

    #region Initialization
    protected override void Awake()
    {
        base.Awake();
        this.cc_animator = gameObject.GetComponent<Animator>();
    }
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        this.cc_rigidBody = GetComponent<Rigidbody>();
        this.m_forwards = new Vector3(0, 0, -1);
        this.m_direction = this.m_forwards;
        this.setState(AnimationState.IDLE);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!this.isPaused)
        {
            onUpdatefollowKeyDirections();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                // Find all nearby interactables
                Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange, LayerMask.GetMask("IInteractables"));
                foreach (Collider collider in colliderArray)
                {
                    var interactableObj = collider.gameObject.GetComponent<IInteractable>();
                    if (interactableObj != null && interactableObj.canInteract())
                    {
                        // Interact with the first IInteractable we find close to us
                        Debug.LogWarningFormat("Found interactable object: {0}", interactableObj);
                        if (this.itemBeingCarried())
                        {
                            interactableObj.interactWithObject(this.itemBeingCarried().gameObject);
                        } else
                        {
                            interactableObj.interactWithObject();
                        }
                        break;
                    }
                }
            }
        }
    }

    protected void onUpdatefollowKeyDirections()
    {
        float rotationSpeed = 80;

        if (this.cc_CameraController.followingCharacter)
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                float delta = Time.deltaTime * this.m_speed;
                Vector3 target = this.gameObject.transform.position + (delta * this.m_direction);
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target, delta);
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                gameObject.transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0, Space.World);
                this.m_direction = Quaternion.AngleAxis(-rotationSpeed * Time.deltaTime, Vector3.up) * this.m_direction;
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                gameObject.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
                this.m_direction = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up) * this.m_direction;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                this.setState(AnimationState.WALKING);
            }
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
            {
                this.setState(AnimationState.IDLE);
            }
        }
    }

    /*
     * Once the food item has been selected, instantiate
     * it so that it's being held in main character's
     * hands
     */
    public void carryItem(GameObject foodPrefab)
    {
        GameObject prefabToClone = null;

        Debug.LogWarning("Going to carry the item");

        // Map the pastry case item to the associated food item if necessary, otherwise just use the received one
        if (foodPrefab.GetComponent<pastryCaseItem>())
        {
            prefabToClone = foodPrefab.GetComponent<pastryCaseItem>().prefab;
        } else {
            prefabToClone = foodPrefab;
        }

        this.setState(AnimationState.CARRYING_POSE);
        GameObject newFoodItemObj = Instantiate(prefabToClone, gameObject.transform);

        // Reset the rotation of the main character
        Quaternion rotation = gameObject.transform.rotation;
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 1);

        FoodItem newFoodItem = newFoodItemObj.GetComponent<FoodItem>();

        Vector3 foodItemPos = handLocalPosition;
        foodItemPos.y += (newFoodItem.getHeight() / 2) - newFoodItem.getCenter().y;
        Debug.LogWarning(foodItemPos);

        newFoodItem.transform.localPosition = foodItemPos;
        newFoodItem.transform.localRotation = Quaternion.Euler(0, 180, 0);

        // retore the rotation of the main character with the food item added,
        // this will ensure that the food item is rotated properly 
        gameObject.transform.rotation = rotation;

        this.m_currentlyCarrying = newFoodItem;
    }

    public bool isCarryingItem()
    {
        return (this.m_currentlyCarrying != null);
    }

    public FoodItem itemBeingCarried()
    {
        return this.m_currentlyCarrying;
    }

    public void dropItem()
    {
        Destroy(this.m_currentlyCarrying.gameObject);
    }

    //protected void setNewTargetFromMousePosition(bool adjustForCollisions)
    //{
    //    Vector3 target = Input.mousePosition;
    //    Ray ray = this.cc_CameraController.getActiveCamera().ScreenPointToRay(target);

    //    if (Physics.Raycast(ray, out RaycastHit hitData, 100, cc_CameraController.floorLayerMask))
    //    {
    //        Debug.Log("Set new target from mouse");
    //        this.setNewTarget(hitData.point, adjustForCollisions, true);
    //    }
    //}

}
