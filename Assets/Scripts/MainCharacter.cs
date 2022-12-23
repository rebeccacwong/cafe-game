using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : CharacterBase
{
    private float m_turnSmoothVelocity;

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
        this.onUpdatefollowKeyDirections();
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    // does not use target
    private void onUpdatefollowKeyDirections()
    {
        float rotationSpeed = 80;

        if (this.cc_CameraController.followingCharacter)
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                float delta = Time.deltaTime * this.m_speed;
                Vector3 target = this.gameObject.transform.position + (delta * this.m_direction);
                this.changeTargetForCollisions(target);
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, this.m_target, delta);
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                gameObject.transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
                this.m_direction = Quaternion.AngleAxis(-rotationSpeed * Time.deltaTime, Vector3.up) * this.m_direction;
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                gameObject.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
                this.m_direction = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up) * this.m_direction;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                //this.cc_animator.SetInteger("State", (int)AnimationState.WALKING);
            }
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
            {
                //this.cc_animator.SetInteger("State", (int)AnimationState.IDLE);
            }
        }
    }

    //private void onUpdateFollowMouseClick()
    //{
    //    float turnSmoothTime = 0.1f;

    //    if (this.cc_CameraController.followingCharacter && Input.GetMouseButtonDown(0))
    //    {
    //        Debug.LogWarning("Clicked");
    //        float horizontalInput = Input.GetAxisRaw("Horizontal");
    //        float verticalInput = Input.GetAxisRaw("Vertical");

    //        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

    //        if (direction.magnitude >= 0.1f)
    //        {
    //            Debug.LogWarning("Rotating");
    //            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    //            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_turnSmoothVelocity, turnSmoothTime);
    //            gameObject.transform.rotation = Quaternion.Euler(0f, angle, 0f);
    //            Vector3 target = gameObject.transform.position + direction * m_speed * Time.deltaTime;
    //            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target, m_speed * Time.deltaTime);
    //        }
    //    }
    //}

    protected void setNewTargetFromMousePosition(bool adjustForCollisions)
    {
        Vector3 target = Input.mousePosition;
        Ray ray = this.cc_CameraController.getActiveCamera().ScreenPointToRay(target);

        if (Physics.Raycast(ray, out RaycastHit hitData, 100, m_floorLayerMask))
        {
            Debug.Log("Set new target from mouse");
            this.setNewTarget(hitData.point, adjustForCollisions, true);
        }
    }
}
