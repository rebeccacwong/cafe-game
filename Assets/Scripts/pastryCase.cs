using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pastryCase : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Poll for clicks to find out which item we're picking
        // TODO: Once we found the item
            // closeCase();
            // carryItem();
    }

    #region IInteractable Methods
    public void interactWithObject()
    {
        openCase();
        // TODO: Enable polling for clicks to figure out which item we're picking
        return;
    }

    public bool canInteract()
    {
        // if the character is already holding an object, 
        return true;
    }
    #endregion

    private void openCase()
    {
        // TODO: Make put the camera to view the pastry case
    }

    private void closeCase()
    {
        // TODO: change the camera back to the old view
    }

    /*
     * Once the food item has been selected, instantiate
     * it so that it's being held in main character's
     * hands
     */
    private void carryItem()
    {
        // TODO: Implement
        return;
    }
}
