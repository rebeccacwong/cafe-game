using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SpawnController : MonoBehaviour
{

	#region Class Variables
	private float m_respawnTimer;
	private float m_numCustomers;
	private float m_remainingCustomers;
	private List<GameObject> allCustomerObjs = new List<GameObject>();
	#endregion

	#region Variabes used by other classes
	private bool m_moreCustomers;
	public bool moreCustomers
	{
		get { return m_moreCustomers; }
		set { m_moreCustomers = value; }
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

	private int numCustomers;

	#region Initialization
	private void Awake()
	{
		//GameObject pm = GameObject.Find("PersistenceManager");
		//cr_audioManager = pm.GetComponent<AudioManager>();
		//GameObject.Find("/Canvas/Start Selling Button").GetComponent<Button>().onClick.AddListener(StartSell);
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
			if (m_respawnTimer <= 0)
			{
				// reset timer
				m_respawnTimer = Random.Range(m_minSpawnInterval, m_maxSpawnInterval);
				m_remainingCustomers -= 1;

				// spawn new customers
				this.NextCustomer();
			}
		}
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
		m_numCustomers = Random.Range(m_minNumCustomers, m_maxNumCustomers);
		m_remainingCustomers = m_numCustomers;

		Debug.Log(string.Format("Spawning {0} total customers in {1} second intervals", m_numCustomers, m_respawnTimer));
		moreCustomers = true;
	}

	/*
	 * Temporarily stos spawning customers.
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
		m_numCustomers = 0;
		m_remainingCustomers = 0;
    }


	public void RemoveAllCustomers()
	{
		foreach (GameObject customer in allCustomerObjs)
        {
			if (customer != null)
            {
				Destroy(customer.gameObject);
			}
        }
	}

	public void NextCustomer()
	{
		Debug.Log("Next customer");
		GameObject npc = Instantiate(NPCPrefabs[Random.Range(0, NPCPrefabs.Length)], startPos, Quaternion.identity);
		allCustomerObjs.Add(npc.gameObject);
	}

	#endregion

}