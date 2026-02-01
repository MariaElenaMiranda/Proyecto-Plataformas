using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseSystem : BaseSceneManager
{
    [Header("Pause Settings")]
    public GameObject pauseMenu; // The UI Panel with buttons
    public GameplaySystem gameplaySystem; // Reference to the main controller
    private bool isPaused = false; // Flag to check if game is frozen

    //-----------------------------------------------------------------------------------------
    //UNITY EVENTS

    void Awake()
    {
        BaseAwake(); // Run parent logic
    }

    void Start()
    {
        BaseStart(); // Run parent start

        // Safety Check: for to forgot to assign the system, try to find it
        if(gameplaySystem == null) gameplaySystem = FindObjectOfType<GameplaySystem>();
    }

    void Update()
    {
        // Check for Escape key press
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused) ResumeGame();
            else PauseGame();
        }
    }

    //-----------------------------------------------------------------------------------------
    // PUBLIC METHODS
    public void PauseGame()
    {
        isPaused = true;

        // Show the Menu
        if(pauseMenu != null) pauseMenu.SetActive(true);

        Time.timeScale = 0f; // Stop time completely

        // Notify system to lower music volume (Underwater effect)
        if(gameplaySystem != null) gameplaySystem.NotifyPause(true);
    }

    public void ResumeGame()
    {
        isPaused = false;

        // Hide the Menu
        if(pauseMenu != null) pauseMenu.SetActive(false);

        Time.timeScale = 1f; // Resume normal time

       // Restore music volume
        if(gameplaySystem != null) gameplaySystem.NotifyPause(false);
    }

    public void RestartGame()
    {
        PlayClickSound(); // Audio feedback

        if(gameplaySystem != null)
        {
            // Get current scene name dynamically
            string currentScene = SceneManager.GetActiveScene().name;

            // Ask GameplaySystem to handle the exit (Fade out, etc.)
            gameplaySystem.ExitScene(currentScene);
        }
    }

    public void MainMenu()
    {
        PlayClickSound(); // Audio feedback

        if(gameplaySystem != null)
        {
            // Return to main menu with transition
            gameplaySystem.ExitScene("MainMenu");
        }
    }
}
