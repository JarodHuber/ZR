using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvents : MonoBehaviour
{
    /// <summary>
    /// change the scene
    /// </summary>
    /// <param name="sceneToChangeTo">name of the scene to change to</param>
    public void ChangeScene(string sceneToChangeTo)
    {
        SceneManager.LoadScene(sceneToChangeTo);
    }

    /// <summary>
    /// starts the game
    /// </summary>
    /// <param name="manager">the general manager</param>
    public void StartGame(Manager manager)
    {
        manager.StartGame();
    }

    /// <summary>
    /// pause / un-pause the game
    /// </summary>
    /// <param name="manager"></param>
    public void PauseAlter(Manager manager)
    {
        manager.Paused = !manager.Paused;
    }
}
