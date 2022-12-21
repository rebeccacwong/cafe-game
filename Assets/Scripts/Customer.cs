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
