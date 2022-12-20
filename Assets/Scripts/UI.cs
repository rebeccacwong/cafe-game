using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    #region
    private SceneController cc_sceneController;
    private CameraController cc_cameraController;
    private SpawnController cc_spawnController;
    #endregion

    public void Awake()
    {
        GameObject sceneController = GameObject.Find("SceneController");
        cc_sceneController = sceneController.GetComponent<SceneController>();

        GameObject cameraController = GameObject.Find("CameraController");
        cc_cameraController = cameraController.GetComponent<CameraController>();

        GameObject customerSpawner = GameObject.Find("CustomerSpawner");
        cc_spawnController = customerSpawner.GetComponent<SpawnController>();

        GameObject.Find("Canvas/StartDayButton").GetComponent<Button>().onClick.AddListener(startDay);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startDay()
    {
        Debug.Log("Starting day");
        cc_cameraController.changeActiveCamera("far camera");
        GameObject.Find("Canvas/StartDayButton").GetComponent<Button>().gameObject.SetActive(false);

        cc_spawnController.minNumCustomers = 4;
        cc_spawnController.maxNumCustomers = 4;
        cc_spawnController.minSpawnInterval = 5f;
        cc_spawnController.maxSpawnInterval = 5f;
        cc_spawnController.StartSpawningCustomers();
    }


}
