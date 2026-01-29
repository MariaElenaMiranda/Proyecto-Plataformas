using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseSystem : BaseSceneManager
{
    [Header("Pause Settings")]
    public GameObject pauseMenu;
    public GameplaySystem gameplaySystem; // Reference to the central system
    private bool isPaused = false; // Controls if the game logic is stopped


    void Awake()
    {
        BaseAwake();
    }

    void Start()
    {
        BaseStart();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Freeze game time

        // Notify system to lower music volume
        if(gameplaySystem != null) gameplaySystem.NotifyPause(true);
    }

    //ResumeButton
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // Resume game time

        // Notify system to restore music volume
        if(gameplaySystem != null) gameplaySystem.NotifyPause(false);
    }


    //Restart button
    public void RestartGame()
    {
        PlayClickSound();

        if(gameplaySystem != null)
        {
            // Reload current scene dynamically
            string currentScene = SceneManager.GetActiveScene().name;
            gameplaySystem.ExitScene(currentScene);
        }
    }

    //MainMenu Button
    public void MainMenu()
    {
        PlayClickSound();

        if(gameplaySystem != null)
        {
            // Starts the coroutine to change scene to MainMenu
            gameplaySystem.ExitScene("MainMenu");
        }
    }
}
