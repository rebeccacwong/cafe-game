using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pastryCase : MonoBehaviour, IInteractable
{
    #region Cached components
    private CameraController cc_CameraController;
    private MainCharacter cc_mainCharacter;
    #endregion

    private Camera m_oldCamera = null;
    private bool m_CurrentlyInteracting;

    // Start is called before the first frame update
    void Start()
    {
        this.cc_CameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        this.cc_mainCharacter = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.m_CurrentlyInteracting)
        {
            // do nothing if we're not actively interacting with the pastry case
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            GameObject foodItem = Utils.returnObjectMouseIsOn(LayerMask.GetMask("FoodItems"));
            if (foodItem)
            {
                Debug.LogWarning(foodItem);
                this.cc_mainCharacter.carryItem(foodItem);
                closeCase();
            }
        }
    }

    #region IInteractable Methods
    public void interactWithObject()
    {
        openCase();
        this.m_CurrentlyInteracting = true;
    }

    public void stopInteractingWithObject()
    {
        Debug.LogWarning("Stop interacting");
        this.m_CurrentlyInteracting = false;
        closeCase();
    }

    public bool canInteract()
    {
        // we cannot pick up a new item if we're already carrying something
        return !this.cc_mainCharacter.isCarryingItem();
    }
    #endregion

    private void openCase()
    {
        // TODO: Make put the camera to view the pastry case
        Debug.LogWarning("Opened pastry case");
        this.m_oldCamera = cc_CameraController.getActiveCamera();
        cc_CameraController.changeActiveCamera("pastryCamera");
    }

    private void closeCase()
    {
        // TODO: change the camera back to the old view
        if (this.m_oldCamera)
        {
            Debug.LogWarning(this.m_oldCamera.name);
            cc_CameraController.changeActiveCamera(this.m_oldCamera);
            this.m_oldCamera = null;
        }
    }
}