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
    protected void changeTargetForCollisions(Vector3 target)
    {
        this.m_target = target;
        float distance = (target - gameObject.transform.position).magnitude;

        RaycastHit hit;
        if (this.cc_rigidBody.SweepTest(this.m_direction, out hit, distance))
        {
            Debug.LogWarning("Detected collision");
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
        
        Debug.LogFormat("[gameObject id: {2}] Set new target to {0} and move in direction {1}",
                        this.m_target,
                        this.m_direction,
                        gameObject.GetInstanceID());

        // change rotation
        if (newAngle)
        {
            float angle = Vector3.SignedAngle(
                oldDirection,
                this.m_direction,
                Vector3.up);
            if (this.m_coroutine != null)
            {
                StopCoroutine(this.m_coroutine);
            }
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

        Debug.LogFormat("[gameObject id: {2}] Target is {0}, currently at {1}",
                        this.m_target,
                        gameObject.transform.position,
                        gameObject.GetInstanceID());

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
