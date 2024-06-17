using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coffeeMachine : MonoBehaviour, IInteractable
{
    // Contains a mapping of active coffee orders from customers so it will
    // only spawn the prefabs that customers ordered.
    private Dictionary<string, (GameObject, int)> m_coffeePrefabs = new Dictionary<string, (GameObject, int)>();

    [SerializeField]
    [Tooltip("The default prefab we will spawn if no customer has ordered coffee yet")]
    GameObject m_defaultCoffeePrefab;

    #region Cached components
    private CameraController cc_CameraController;
    private MainCharacter cc_mainCharacter;
    #endregion

    private Camera m_oldCamera = null;
    private bool m_CurrentlyInteracting;
    private Vector3 m_spawnPos = new Vector3(12.022f, 3.112f, 24.216f);

    // Start is called before the first frame update
    void Start()
    {
        this.cc_CameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        this.cc_mainCharacter = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
    }

    #region IInteractable Methods
    public void interactWithObject(GameObject optionalParam = null)
    {
        _ = optionalParam;
        FoodItem foodItem;
        foodItem = this.m_defaultCoffeePrefab.GetComponent<FoodItem>();
        if (this.m_coffeePrefabs.Count == 0)
        {
            foodItem = this.m_defaultCoffeePrefab.GetComponent<FoodItem>();
            Debug.LogWarning("Making default coffee");
        }
        else
        {
            foreach (string key in m_coffeePrefabs.Keys)
            {
                Debug.LogWarningFormat("Getting key {0} from coffee machine dictionary", key);
                foodItem = this.m_coffeePrefabs[key].Item1.GetComponent<FoodItem>();
                break;
            }
        }
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
        AudioManager.Instance.PlaySoundEffect("coffeePour");

        StartCoroutine(waitOutAnimationThenExitCoffeeCam(4.5f, newCoffee));
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

    /*
     * Allows the coffee machine to make coffee prefab. if ADDTOLIST is false, 
     * removes the prefab from the list of things to potentially make if no 
     * other customers will potentially order (the count value for that prefab
     * in the dictionary is 0).
     */
    public void updateCoffeeMachinePrefabList(FoodItem coffeePrefab, bool AddToList)
    {
        Debug.Assert(coffeePrefab.prepLocation == "coffeeMachine");

        if (AddToList)
        {
            Debug.LogWarningFormat("[Coffee Machine] Adding {0} to dictionary.", coffeePrefab.name);
            if (!m_coffeePrefabs.ContainsKey(coffeePrefab.name))
            {
                m_coffeePrefabs.Add(coffeePrefab.name, (coffeePrefab.gameObject, 1));
            } else
            {
                GameObject prefab = m_coffeePrefabs[coffeePrefab.name].Item1;
                int count = m_coffeePrefabs[coffeePrefab.name].Item2;
                m_coffeePrefabs[coffeePrefab.name] = (prefab, count + 1);
            }
            Debug.LogWarningFormat("[Coffee Machine] Dictionary has {0} of {1}.", m_coffeePrefabs[coffeePrefab.name].Item2, coffeePrefab.name);
        } else
        {
            Debug.Assert(m_coffeePrefabs.ContainsKey(coffeePrefab.name));
            int count = m_coffeePrefabs[coffeePrefab.name].Item2;
            Debug.LogWarningFormat("[Coffee Machine] Removing {0} from dictionary. Count before was: {1}", coffeePrefab.name, count);
            if (count - 1 == 0)
            {
                m_coffeePrefabs.Remove(coffeePrefab.name);
            } else
            {
                GameObject prefab = m_coffeePrefabs[coffeePrefab.name].Item1;
                m_coffeePrefabs[coffeePrefab.name] = (prefab, count - 1);
            }
        }

    }
}
