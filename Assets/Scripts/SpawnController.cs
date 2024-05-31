using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class SpawnController : MonoBehaviour
{

	#region Class Variables
	private float m_respawnTimer;
	private float m_numCustomersToSpawn;
	private float m_remainingCustomers;
	private List<GameObject> allCustomerObjs = new List<GameObject>();
    #endregion

    #region Cached components
	private MainCharacter cc_mainCharacter;
    #endregion

    #region Variabes used by other classes
    private bool m_moreCustomers;
	public bool moreCustomers
	{
		get { return m_moreCustomers; }
		set { m_moreCustomers = value; }
	}

	private int m_activeCustomers = 0;
	public int activeCustomers
    {
		get { return this.m_activeCustomers; }
		set { m_activeCustomers = value;  }
    }

	private float m_minSpawnInterval;
	public float minSpawnInterval
	{
		set { m_minSpawnInterval = value; }
	}

	private float m_maxSpawnInterval;
	public float maxSpawnInterval
	{
		set { m_maxSpawnInterval = value; }
	}

	private int m_minNumCustomers;
	public int minNumCustomers
	{
		set { m_minNumCustomers = value; }
	}

	private int m_maxNumCustomers;
	public int maxNumCustomers
	{
		set { m_maxNumCustomers = value; }
	}
	#endregion

	public UnityEvent noMoreCustomersEvent;

	#region Editor Variables
	[SerializeField]
	[Tooltip("NPC Prefabs")]
	private GameObject[] NPCPrefabs;

	[SerializeField]
	[Tooltip("3d coordinates where the customers should spawn from.")]
	private Vector3 startPos;
	public Vector3 spawnPosition
    {
		get { return this.startPos; }
    }
	#endregion

	#region Initialization
	private void Awake()
	{
		this.cc_mainCharacter = GameObject.Find("MainCharacter").GetComponent<MainCharacter>();
		Debug.Assert(this.cc_mainCharacter != null, "SpawnController must find a main character object!");

		if (this.noMoreCustomersEvent == null)
        {
			this.noMoreCustomersEvent = new UnityEvent();
        }
	}
	#endregion

	#region Main Functions
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (moreCustomers && m_remainingCustomers > 0)
		{
			m_respawnTimer -= Time.deltaTime;
			if (this.shouldSpawn())
			{
				// reset timer
				m_respawnTimer = Random.Range(m_minSpawnInterval, m_maxSpawnInterval);
				m_remainingCustomers -= 1;

				// spawn new customers
				this.NextCustomer();
			}
		}
	}

    private void LateUpdate()
    {
		Stats.pushRealTimeAvgCustomerSatisfaction(calculateRealtimeAvgCustomerSatisfaction());
	}

    private bool shouldSpawn()
    {
		return ((Vector3.Distance(this.cc_mainCharacter.transform.position, this.spawnPosition) > 3f)
			 && m_respawnTimer <= 0);
	}

	public void StartSpawningCustomers()
	{
		if (m_minSpawnInterval <= 0 || m_maxSpawnInterval <= 0)
        {
			throw new System.Exception("Invalid spawn interval");
        }
		if (m_minNumCustomers <= 0 || m_maxNumCustomers <= 0)
        {
			throw new System.Exception("Invalid input to num customers");
        }
		//GameObject.Find("/Canvas/Start Selling Button").active = false;
		m_respawnTimer = Random.Range(m_minSpawnInterval, m_maxSpawnInterval); //Respawns the enemy after this many seconds
		m_numCustomersToSpawn = Random.Range(m_minNumCustomers, m_maxNumCustomers);
		m_remainingCustomers = m_numCustomersToSpawn;

		Debug.Log(string.Format("Spawning {0} total customers in {1} second intervals", m_numCustomersToSpawn, m_respawnTimer));
		moreCustomers = true;
	}

	/*
	 * Temporarily stops spawning customers.
	 * Will maintain the knowledge of the
	 * customers remaining for the day.
	 */
	public void PauseCustomerSpawning()
    {
		moreCustomers = false;
	}

	public void ResumeCustomerSpawning()
    {
		moreCustomers = true;
	}

	/*
	 * Will stop the spawning entirely. 
	 * Will need to run StartSpawningCustomers
	 * to spawn more.
	 */
	public void StopSpawningCustomers()
    {
		moreCustomers = false;
		m_numCustomersToSpawn = 0;
		m_remainingCustomers = 0;
    }


	public void RemoveAllCustomers()
	{
		foreach (GameObject customer in allCustomerObjs)
        {
			if (customer != null)
            {
				Customer customerObj = customer.GetComponent<Customer>();
				if (customerObj != null)
                {
					customerObj.pushCustomerStats();
                }

				Destroy(customer.gameObject);
			}
        }
		allCustomerObjs.Clear();
	}

	public void RemoveCustomer(GameObject obj)
    {
		this.allCustomerObjs.Remove(obj);
	}

	public void NextCustomer()
	{
		GameObject npc = Instantiate(NPCPrefabs[Random.Range(0, NPCPrefabs.Length)], startPos, Quaternion.identity);
        Debug.Assert(npc != null);
        Customer CustomerObj = npc.GetComponent<Customer>();
		Debug.Assert(CustomerObj != null);
        Debug.LogWarningFormat("New customer {0:X} arrived in cafe.", npc.GetInstanceID());

        //CustomerObj.destroyEvent.AddListener(RemoveCustomer);
        this.allCustomerObjs.Add(npc);
		this.m_activeCustomers++;
	}

	public float calculateRealtimeAvgCustomerSatisfaction()
	{
		float sum = 0;
		if (this.allCustomerObjs.Count == 0)
        {
			return 1f;
        }
		foreach (GameObject obj in this.allCustomerObjs)
        {
			Customer customer = obj.GetComponent<Customer>();
			Debug.Assert(customer != null);
			sum += customer.getCustomerSatisfaction();
        }
		return sum / this.allCustomerObjs.Count;
	}

	#endregion

}