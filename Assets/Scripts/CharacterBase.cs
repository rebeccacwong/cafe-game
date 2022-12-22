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
    private IEnumerator m_coroutine = null;

    private float m_rotationAngle;
    public float getRotationAngle
    {
        get { return m_rotationAngle; }
        set { m_rotationAngle = value; }
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
        this.m_target = gameObject.transform.position;
        this.cc_CameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        if (this.cc_CameraController == null)
        {
            throw new Exception("Could not find CameraController object.");
        }
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
        this.m_target = target;
        float distance = (target - gameObject.transform.position).magnitude;

        RaycastHit hit;
        if (this.cc_rigidBody.SweepTest(this.m_direction, out hit, distance))
        {
            Vector3 modified = hit.point - (2 * this.m_direction);

            // if sufficiently close, don't move
            if (Vector3.Distance(gameObject.transform.position, modified) <= 1)
            {
                this.m_target = gameObject.transform.position;
                return;
            }
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
            this.setNewTarget(hitData.point, adjustForCollisions, true);
        }
    }

    protected void onUpdatefollowKeyDirections()
    {
        // check if we need to change directions
        Vector3 oldDirection = this.m_direction;
        Vector3 newDirection = this.m_direction;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            newDirection = this.m_forwards;
            if (!cc_CameraController.followingCharacter)
            {
                newDirection *= -1;
            }
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            newDirection = -this.m_forwards;
            if (!cc_CameraController.followingCharacter)
            {
                newDirection *= -1;
            }
            //this.m_direction = Quaternion.AngleAxis(180, Vector3.up) * this.m_direction;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            newDirection = Quaternion.AngleAxis(90, Vector3.up) * this.m_forwards;
            if (!cc_CameraController.followingCharacter)
            {
                newDirection *= -1;
            }
            //this.m_direction = Quaternion.AngleAxis(-90, Vector3.up) * this.m_direction;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            newDirection = Quaternion.AngleAxis(-90, Vector3.up) * this.m_forwards;
            if (!cc_CameraController.followingCharacter)
            {
                newDirection *= -1;
            }
            //this.m_direction = Quaternion.AngleAxis(90, Vector3.up) * this.m_direction;
        }

        if (newDirection != oldDirection)
        {
            Debug.Log(oldDirection + " " + newDirection);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            // One of the key buttons is down, so we should update the target
            this.setNewTarget((this.m_speed * newDirection) + gameObject.transform.position, true, (newDirection != oldDirection));
            this.onUpdateMoveTowardsTarget();
        } else
        {
            return;
        }
    }

    /*
     * Updates the internal TARGET for the position of the character.
     * If NEWANGLE is true, it will also set the rotation to change according
     * to the new target.
     */
    protected void setNewTarget(Vector3 target, bool adjustForCollisions, bool newAngle)
    {
        this.m_finalTarget = target;
        Vector3 oldDirection = this.m_direction;

        this.m_direction = (target - gameObject.transform.position).normalized;

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
        if (newAngle)
        {
            float angle = Vector3.SignedAngle(
                oldDirection,
                this.m_direction,
                Vector3.up);
            Debug.Log(angle);
            if (this.m_coroutine != null)
            {
                StopCoroutine(this.m_coroutine);
            }
            m_rotationAngle = angle;
            this.m_coroutine = updateRotation(angle);
            StartCoroutine(this.m_coroutine);
        }
    }

    protected void onUpdateMoveTowardsTarget()
    {

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

    private IEnumerator updateRotation(float finalAngle)
    {
        float timeInterval = 0.01f;
        float t = 6f;
        float i = 0f;

        if (cc_CameraController.followingCharacter)
        {
            t *= 5;
            Debug.Log("Increased time");
        }

        float angleRotation = finalAngle / t;

        while (i < t)
        {
            //angleRotation = Mathf.Lerp(0f, finalAngle, i / t);
            gameObject.transform.Rotate(Vector3.up, angleRotation);
            yield return new WaitForSeconds(timeInterval);
            i++;
        }
        yield break;
    }
}
