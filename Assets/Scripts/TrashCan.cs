using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    private MainCharacter cc_mainCharacter;

    // Start is called before the first frame update
    void Start()
    {
        this.cc_mainCharacter = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
        Debug.Assert(this.cc_mainCharacter != null, "TrashCan must find a main character object!");
    }

    #region IInteractable Methods
    public void interactWithObject(GameObject optionalParam = null)
    {
        _ = optionalParam;
        if (this.cc_mainCharacter.isCarryingItem())
        {
            this.cc_mainCharacter.dropItem();
        }
    }

    public bool canInteract()
    {
        return this.cc_mainCharacter.isCarryingItem();
    }
    #endregion
}
