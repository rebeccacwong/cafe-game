using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    private MainCharacter cc_mainCharacter;
    private Animator cc_animator;

    // Start is called before the first frame update
    void Start()
    {
        this.cc_mainCharacter = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
        Debug.Assert(this.cc_mainCharacter != null, "TrashCan must find a main character object!");

        this.cc_animator = GetComponent<Animator>();
        Debug.Assert(this.cc_animator != null, "Trash can must have an animator attribute");
    }

    #region IInteractable Methods
    public void interactWithObject(GameObject optionalParam = null)
    {
        _ = optionalParam;
        if (this.cc_mainCharacter.isCarryingItem())
        {
            Stats.addTrashStats(1, this.cc_mainCharacter.itemBeingCarried().itemPrice);

            this.cc_animator.SetTrigger("showAnimation");
            this.cc_mainCharacter.dropItem();
            AudioManager.Instance.PlaySoundEffect("trash");
        }
    }

    public bool canInteract(out string errorString)
    {
        errorString = "";
        return this.cc_mainCharacter.isCarryingItem();
    }
    #endregion
}
