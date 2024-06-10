using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SceneController : MonoBehaviour
{
    private GameManager cc_gameManager;

    private void Awake()
    {
        // show warning, assert, errors, and exceptions
        Debug.unityLogger.filterLogType = LogType.Warning;

        cc_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // display all logs
        //Debug.unityLogger.filterLogType = LogType.Log;
        DontDestroyOnLoad(this.gameObject);
    }

    public void GoToScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }

    public void GoToInBetweenDaysScene()
    {

        GoToScene("BetweenDays");
    }

    public void StartNewDay()
    {
        SceneManager.LoadScene("Cafe");
        cc_gameManager.InitializeNewLevel();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
