using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : CharacterBase
{
    private float m_turnSmoothVelocity;
    private bool m_interacting;
    private GameObject m_currentlyCarrying;

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
                float interactRange = 2f;
                Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
                if (colliderArray.Length > 0)
                {
                    Debug.LogWarning("Found a collision with something nearby");
                }
                foreach (Collider collider in colliderArray)
                {
                    IInteractable interactableObj = collider.GetComponent<IInteractable>();
                    if (interactableObj != null && interactableObj.canInteract())
                    {
                        Debug.LogWarning("Found an interactable object");
                        if (this.m_interacting)
                        {
                            interactableObj.stopInteractingWithObject();
                            this.m_interacting = false;
                        } else
                        {
                            this.m_interacting = true;
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
        Debug.LogWarning("Going to carry the item");
        this.setState(AnimationState.CARRYING_POSE);
        // Add animation
        // place the object under the main character prefab
        return;
    }

    public bool isCarryingItem()
    {
        return (this.m_currentlyCarrying != null);
    }

    public void dropItem()
    {
        // if the 
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
