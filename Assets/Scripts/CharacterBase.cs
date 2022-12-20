using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AnimationState
{
    WALKING = 0,
    IDLE = 1
}

[System.Serializable]
public class CharacterBase : MonoBehaviour
{
    #region Cached Components
    protected CameraController cc_CameraController;
    protected Rigidbody cc_rigidBody;
    protected Animator cc_animator;
    #endregion

    protected Vector3 m_target;
    protected Vector3 m_finalTarget;
    protected Vector3 m_direction;
    protected Vector3 m_forwards;

    #region Lerp variables
    private float m_a = 0;
    private float m_b = 50;
    private float m_rotationFrameCount = 0;
    private float m_rotationFramesTotal = 5;
    #endregion

    private GameObject m_gameObject;
    public GameObject gameObj
    {
        get { return m_gameObject; }
        set { m_gameObject = value; }
    }

    //[SerializeField]
    //[Tooltip("The layer mask for all tables")]
    //public LayerMask m_tables;

    [SerializeField]
    [Tooltip("The layer mask of the floor")]
    public LayerMask m_floorLayerMask;

    [SerializeField]
    [Tooltip("The speed the character should move at")]
    protected float m_speed;

    protected virtual void Awake()
    {
        Debug.Log("Character base awake");
        this.m_target = gameObject.transform.position;
        //this.cc_CameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        //if (this.cc_CameraController == null)
        //{
        //    throw new Exception("Could not find CameraController object.");
        //}
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Do nothing
    }

    //// Update is called once per frame
    protected virtual void Update()
    {
        // Do nothing
    }

    /*
     * Changes this.m_target to be before a potential
     * collision if there is a collision in DIR direction
     * before DISTANCE dist. 
     */
    private void changeTargetForCollisions(Vector3 target)
    {
        this.m_direction = (target - gameObject.transform.position).normalized;
        float distance = (target - gameObject.transform.position).magnitude;
        this.m_target = target;

        RaycastHit hit;
        if (this.cc_rigidBody.SweepTest(this.m_direction, out hit, distance))
        {
            this.m_target = hit.point - (2 * this.m_direction);
        }
        this.m_target.y = gameObject.transform.position.y;
    }

    protected void setNewTargetFromMousePosition(bool adjustForCollisions)
    {
        Vector3 target = Input.mousePosition;
        Ray ray = this.cc_CameraController.getActiveCamera().ScreenPointToRay(target);

        if (Physics.Raycast(ray, out RaycastHit hitData, 100, m_floorLayerMask))
        {
            this.setNewTarget(hitData.point, adjustForCollisions);
        }
    }

    protected void onUpdatefollowKeyDirections()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.m_direction = -this.m_direction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.m_direction = this.m_direction;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.m_direction = Quaternion.AngleAxis(90, Vector3.up) * this.m_direction;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.m_direction = Quaternion.AngleAxis(-90, Vector3.up) * this.m_direction;
        } else
        {
            return;
        }

        Debug.Log(String.Format("Key direction, changed vector to: {0}", this.m_direction));

        this.setNewTarget((this.m_speed * this.m_direction) + gameObject.transform.position, false);
        this.onUpdateMoveTowardsTarget();
    }

    protected void setNewTarget(Vector3 target, bool adjustForCollisions)
    {
        this.m_finalTarget = target;

        if (adjustForCollisions)
        {
            // set our temporary target until we can reach our final target
            this.changeTargetForCollisions(target);
        }
        //this.m_target.y = 1.79f;

        //Debug.Log(
        //    String.Format(
        //        "[gameObject id: {2}] Set new target to {0} and move in direction {1}",
        //        this.m_target,
        //        this.m_direction,
        //        gameObject.GetInstanceID()));

        // change rotation
        float angle = Vector3.SignedAngle(
            gameObject.transform.TransformDirection(this.m_forwards),
            this.m_direction,
            Vector3.up);
        //gameObject.transform.Rotate(Vector3.up, angle);
        this.m_a = 0;
        this.m_b = angle;
        this.m_rotationFrameCount = 0;
    }

    protected void onUpdateMoveTowardsTarget()
    {
        float angle_rotation = Mathf.Lerp(this.m_a, this.m_b, this.m_rotationFrameCount / this.m_rotationFramesTotal);
        this.m_rotationFrameCount += 1;

        // check if we can update our target
        if (m_finalTarget != m_target)
        {
            this.changeTargetForCollisions(this.m_finalTarget);
        }

        if (gameObject == null || this.m_target == null)
        {
            this.setState(AnimationState.IDLE);
            return;
        }
        if (Vector3.Distance(gameObject.transform.position, this.m_target) <= 0.5)
        {
            this.setState(AnimationState.IDLE);
            gameObject.transform.position = this.m_target;
            return;
        }

        //Debug.Log(
        //    String.Format("[gameObject id: {2}] Target is {0}, currently at {1}",
        //    this.m_target,
        //    gameObject.transform.position,
        //    gameObject.GetInstanceID()));

        var delta = this.m_speed * Time.deltaTime;
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, this.m_target, delta);
    }

    private void setState(AnimationState state)
    {
        if (this.cc_animator != null)
        {
            this.cc_animator.SetInteger("State", (int) AnimationState.IDLE);
        }
    }

}
