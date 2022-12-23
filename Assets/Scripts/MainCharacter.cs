using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : CharacterBase
{

    #region Initialization
    protected override void Awake()
    {
        base.Awake();
    }
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        this.cc_rigidBody = GetComponent<Rigidbody>();
        this.m_forwards = new Vector3(0, 0, -1);
        this.m_direction = this.m_forwards;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (this.cc_CameraController.followingCharacter)
        {
            this.onUpdatefollowKeyDirections();
        }
        else
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
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    // does not use target
    private void onUpdatefollowKeyDirections()
    {
        float rotationSpeed = 60;

        if (!cc_CameraController.followingCharacter)
        {
            throw new Exception("To follow key directions, need to be using POV camera.");
            return;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            float delta = Time.deltaTime * this.m_speed;
            Vector3 target = this.gameObject.transform.position + (delta * this.m_direction);
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target, delta); 
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            gameObject.transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            this.m_direction = Quaternion.AngleAxis(-rotationSpeed * Time.deltaTime, Vector3.up) * this.m_direction;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            gameObject.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            this.m_direction = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up) * this.m_direction;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //this.cc_animator.SetInteger("State", (int)AnimationState.WALKING);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            //this.cc_animator.SetInteger("State", (int)AnimationState.IDLE);
        }
    }
}
