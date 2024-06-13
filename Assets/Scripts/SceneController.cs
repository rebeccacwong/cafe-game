using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SceneController : MonoBehaviour
{

    private void Awake()
    {
        // show warning, assert, errors, and exceptions
        Debug.unityLogger.filterLogType = LogType.Warning;

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
        Debug.Log("Switching to BetweenDays scene");
        GoToScene("BetweenDays");
    }

    public void StartNewDay()
    {
        Debug.Log("Switching to cafe scene.");
        SceneManager.LoadScene("Cafe");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
