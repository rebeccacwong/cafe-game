using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coffeeMachine : MonoBehaviour, IInteractable
{
    [Tooltip("Coffee prefabs")]
    public GameObject[] m_coffeePrefabs;

    #region Cached components
    private CameraController cc_CameraController;
    private MainCharacter cc_mainCharacter;
    private AudioManager cc_audioManager;
    #endregion

    private Camera m_oldCamera = null;
    private bool m_CurrentlyInteracting;
    private Vector3 m_spawnPos = new Vector3(11.97f, 3.112f, 24.216f);

    // Start is called before the first frame update
    void Start()
    {
        this.cc_CameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        this.cc_mainCharacter = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
        this.cc_audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    #region IInteractable Methods
    public void interactWithObject(GameObject optionalParam = null)
    {
        _ = optionalParam;
        FoodItem foodItem = this.m_coffeePrefabs[Random.Range(0, m_coffeePrefabs.Length)].GetComponent<FoodItem>();
        Debug.Assert(foodItem != null, "CoffeeMachine interactWithObject should receive an item that is a FoodItem.");

        makeCoffee(foodItem);
        //this.m_CurrentlyInteracting = true;
    }

    public bool canInteract(out string errorString)
    {
        errorString = "Already carrying an item! Serve it to a customer or throw it away.";
        return !this.cc_mainCharacter.isCarryingItem() && !this.m_CurrentlyInteracting;
    }
    #endregion

    private void makeCoffee(FoodItem coffeeItem)
    {
        this.cc_mainCharacter.gameObject.SetActive(false);
        Debug.Log("Making coffee");
        this.m_oldCamera = cc_CameraController.getActiveCamera();
        cc_CameraController.changeActiveCamera("coffeeCamera");

        FoodItem newCoffee = Instantiate(coffeeItem, m_spawnPos, Quaternion.identity);
        newCoffee.transform.Rotate(new Vector3(0, 180, 0));

        Animator animator = newCoffee.GetComponent<Animator>();
        if (animator)
        {
            animator.SetBool("animate", true);
        }

        StartCoroutine(waitOutAnimationThenExitCoffeeCam(5, newCoffee));
    }

    private IEnumerator waitOutAnimationThenExitCoffeeCam(float waitTimeInSeconds, FoodItem coffeeItem)
    {
        yield return new WaitForSeconds(waitTimeInSeconds);
        this.cc_mainCharacter.gameObject.SetActive(true);

        if (this.m_oldCamera)
        {
            cc_CameraController.changeActiveCamera(this.m_oldCamera);
            this.m_oldCamera = null;
        }

        // make character carry a new version of this
        FoodItem newCoffeeBeingCarried = this.cc_mainCharacter.carryItem(coffeeItem.gameObject);
        newCoffeeBeingCarried.transform.Find("stream").gameObject.SetActive(false);

        // destroy the one on the coffee machine
        Destroy(coffeeItem.gameObject);
    }
}
