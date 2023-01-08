using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : CharacterBase, IDraggableObject
{

    #region Instance variables
    private Vector3 m_frontOfLinePos;
    private Vector3 m_screenPointOfCharacter;
    private bool m_isBeingDragged;
    private bool m_isMoving;
    #endregion

    #region Cached components
    private SpawnController cc_spawnController;
    #endregion

    [SerializeField]
    [Tooltip("The number of frames for the character's sit animation")]
    private int m_numFramesForSitAnim;


    #region Overrides
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
    #endregion





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

    /*
     * Makes customer sit down in the given chair.
     */
    public void sitDown(GameObject chairGameObject)
    {

        this.m_isMoving = false;

        Chair chairData = chairGameObject.GetComponent<Chair>();
        Debug.Assert(chairData != null, "All chair gameObjects must have a Chair script");

        Vector3 characterPosition = new Vector3(chairGameObject.transform.position.x, chairData.heightOfSeat, chairGameObject.transform.position.z);
        float angle = Vector3.SignedAngle(this.m_direction, chairData.facingDirection, Vector3.up);

        Debug.LogWarningFormat("Sitting down, originally had position {0} and rotation{1}, now will sit at position {2} and rotate by {3} degrees",
            gameObject.transform.position, gameObject.transform.rotation.eulerAngles, characterPosition, angle);
        this.setState(AnimationState.SITTING);
        gameObject.transform.Rotate(0, angle, 0);
        gameObject.transform.position = characterPosition;
        //StartCoroutine(sitCoroutine(characterPosition, angle));
    }

    /*
     * Sit so that the character ultimately ends up in finalPosition 
     */
    //private IEnumerator sitCoroutine(Vector3 finalPosition, float degreesToRotate)
    //{
    //    Debug.Assert(this.m_numFramesForSitAnim > 0, "Must specify the number of frames for the sit animation");
    //    Debug.LogWarningFormat("Starting coroutine across {0} frames", this.m_numFramesForSitAnim);

    //    Vector3 startPosition = gameObject.transform.position;
    //    Vector3 startRotation = gameObject.transform.rotation.eulerAngles;

    //    for (int i = 0; i < this.m_numFramesForSitAnim; i++)
    //    {
    //        gameObject.transform.Rotate(0, startRotation.y + (degreesToRotate / this.m_numFramesForSitAnim), 0);
    //        gameObject.transform.position = Vector3.Lerp(startPosition, finalPosition, i / this.m_numFramesForSitAnim);
    //        yield return new WaitForSeconds(Time.deltaTime);
    //    }

    //    Vector3 finalEulerRotation = Quaternion.AngleAxis(degreesToRotate, Vector3.up) * this.m_direction;
    //    Debug.AssertFormat(
    //        finalPosition == gameObject.transform.position,
    //        "Coroutine ended, expected final position {0}, got {1}",
    //        finalPosition,
    //        gameObject.transform.position);
    //    Debug.AssertFormat(
    //        finalEulerRotation == gameObject.transform.rotation.eulerAngles,
    //        "Coroutine ended, expected final rotation {0}, got {1}",
    //        finalEulerRotation,
    //        gameObject.transform.rotation.eulerAngles);
    //    yield break;
    //}



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

        // check if we're intersecting with a chair or table and we should it there
        GameObject collisionObj = Utils.returnObjectMouseIsOn(LayerMask.GetMask("Chairs"));
        if (collisionObj != null)
        {
            if (collisionObj.tag == "Chair")
            {
                Debug.LogWarning("collided with a chair");
                this.sitDown(collisionObj);
            }
        }
    }

    public void onUpdateDragObject()
    {
        if (this.isBeingDragged)
        {
            if (Input.GetMouseButton(0))
            {
                // TODO: edit so that we get more range of motion along z axis
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
