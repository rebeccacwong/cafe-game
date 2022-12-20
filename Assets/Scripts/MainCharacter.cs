using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : CharacterBase
{

    #region Editor variables
    [SerializeField]
    [Tooltip("Enabled as true if the player should follow mouse")]
    private bool m_followMouse = false;
    #endregion

    #region Initialization
    protected override void Awake()
    {
        this.gameObj = GameObject.Find("MainCharacter");
        base.Awake();
    }
    #endregion

    public void startMainCharacterMovement()
    {
        this.m_followMouse = true;
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        this.cc_rigidBody = GetComponent<Rigidbody>();
        this.m_followMouse = false;
        this.m_forwards = new Vector3(0, 0, -1);
        this.m_direction = this.m_forwards;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (this.m_followMouse)
        {
            if (Input.GetMouseButtonDown(0))
            {
                base.setNewTargetFromMousePosition(false);
                //Camera activeCamera = cc_CameraController.getActiveCamera();
                //Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);

                //if (Physics.Raycast(ray, out RaycastHit hitData, 100, m_floorLayerMask))
                //{
                //    m_target = hitData.point;
                //    float distance = hitData.distance;

                //    m_direction = (m_target - transform.position).normalized;

                //    // adjust the target if there are collisions
                //    RaycastHit hit;
                //    if (cc_rigidBody.SweepTest(m_direction, out hit, distance))
                //    {
                //        Debug.Log("hit");
                //        m_target = hit.point - (2 * m_direction);
                //    }
                //    m_target.y = 1.79f;

                //    // change rotation
                //    Vector3 forwards = new Vector3(0, 0, -1);
                //    float angle = Vector3.SignedAngle(transform.TransformDirection(forwards), m_direction, Vector3.up);
                //    transform.Rotate(Vector3.up, angle);
            }
            this.onUpdateMoveTowardsTarget();
        } else
        {
            base.onUpdatefollowKeyDirections();
        }
        Debug.Log("After updating, direction is: " + m_direction);
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}
