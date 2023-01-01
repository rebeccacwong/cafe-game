using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : CharacterBase, IDraggableObject
{
    #region Static variables

    #endregion

    #region Instance variables
    private Vector3 m_frontOfLinePos;
    private Vector3 m_screenPointOfCharacter;
    private bool m_isBeingDragged;
    private bool m_isMoving;
    #endregion

    #region Cached components
    private SpawnController cc_spawnController;
    #endregion

    public Customer(GameObject gameObject, float speed)
    {
        //npc.GetComponent<NPCMovementController>().NextMovementState();
    }

    protected override void Awake()
    {
        base.Awake();
        GameObject customerSpawner = GameObject.Find("CustomerSpawner");
        cc_spawnController = customerSpawner.GetComponent<SpawnController>();
        this.cc_rigidBody = gameObject.GetComponent<Rigidbody>();
        this.cc_animator = gameObject.GetComponent<Animator>();
        this.m_isMoving = true;
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
        if (base.isPaused)
        {
            return;
        }

        if (this.m_isMoving)
        {
            this.onUpdateMoveTowardsTarget();
        }
        this.onUpdateDragObject();
    }

    private void removeCustomer()
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        int index = this.cc_spawnController.allCustomerObjs.IndexOf(gameObject);
        if (index == -1)
        {
            throw new Exception("Could not find object in spawn controller");
        }
        this.cc_spawnController.allCustomerObjs.RemoveAt(index);
    }



    #region IDraggableObject override
    public bool isBeingDragged
    {
        get { return m_isBeingDragged; }
        set { m_isBeingDragged = value; }
    }

    public void startDraggingObject()
    {
        this.isBeingDragged = true;
        Debug.LogWarningFormat("Start dragging customer {0} with id {1}", gameObject, gameObject.GetInstanceID());

        this.cc_rigidBody.isKinematic = false;
        this.m_screenPointOfCharacter = this.cc_CameraController.getActiveCamera().WorldToScreenPoint(gameObject.transform.position);
    }

    public void stopDraggingObject()
    {
        this.isBeingDragged = false;
        this.cc_rigidBody.isKinematic = true;
        Debug.LogWarningFormat("Stop dragging customer {0} with id {1}", gameObject, gameObject.GetInstanceID());
    }

    public void sitDown()
    {
        this.m_isMoving = false;
    }

    public void onUpdateDragObject()
    {
        if (this.isBeingDragged)
        {
            if (Input.GetMouseButton(0))
            {
                // check if we're intersecting with a chair or table and we should drop
                GameObject collisionObj = Utils.returnObjectMouseIsOn(LayerMask.GetMask("Chairs"));
                if (collisionObj != null)
                {
                    Debug.LogWarning("collision detected with: " + collisionObj);
                    if (collisionObj.tag == "Chair")
                    {
                        Debug.LogWarning("collided with a chair");
                        this.stopDraggingObject();
                        this.sitDown();
                    }
                }

                // TODO: edit so that we get more range of motion along x,z axis
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
