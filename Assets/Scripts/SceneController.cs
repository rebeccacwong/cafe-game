using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SceneController : MonoBehaviour
{
    private static SceneController _instance;
    public static SceneController Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("SceneConntroller is null!");
            }
            return _instance;
        }
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }

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

    public void GoToSpecialSelection()
    {
        Debug.Log("Switching to SpecialSelection scene");
        GoToScene("SpecialSelection");
    }

    public void GoToFeedback()
    {
        Debug.Log("Switching to Feedback scene");
        GoToScene("Feedback");
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
