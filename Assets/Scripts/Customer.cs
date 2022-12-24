using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : CharacterBase
{
    #region Static variables

    #endregion

    #region Instance variables
    private Vector3 m_frontOfLinePos;
    private bool m_dragging;
    private Vector3 m_screenPointOfCharacter;
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
        this.onUpdateMoveTowardsTarget();
        //this.onUpdateDragToFollowMousePos();
    }

    private void onUpdateDragToFollowMousePos()
    {
        Camera activeCamera = this.cc_CameraController.getActiveCamera();

        if (Utils.isObjectClicked(gameObject))
        {
            // drag character
            this.m_screenPointOfCharacter = activeCamera.WorldToScreenPoint(gameObject.transform.position);
            //Vector3 offset = Vector3.zero;
            Debug.LogWarning("Clicked character");
            this.m_dragging = true;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.m_screenPointOfCharacter.z);
            Vector3 curPosition = activeCamera.ScreenToWorldPoint(curScreenPoint); // + offset optional
            gameObject.transform.position = curPosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            this.m_dragging = false;
            // TODO: add behavior for when the user lets go of mouse
        }
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
}
