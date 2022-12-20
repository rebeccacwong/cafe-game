using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SceneController : MonoBehaviour
{
    public void GoToScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }

    public void GoToSelectionScreen()
    {

        GoToScene("SelectionScreen");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
