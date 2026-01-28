using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseSystem : MonoBehaviour
{
    [Header("Interface")]
    public GameObject pauseMenu;
    public GameplaySystem gameplaySystem; // Reference to the central system

    [Header("Sounds")]
    public AudioSource soundEffect;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private bool isPaused = false; // Controls if the game logic is stopped

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
            gameplaySystem.ChangeScene(SceneManager.GetActiveScene().name);
        }
    }

    //MainMenu Button
    public void MainMenu()
    {
        PlayClickSound();

        if(gameplaySystem != null)
        {
            // Starts the coroutine to change scene to MainMenu
            gameplaySystem.ChangeScene("MainMenu");
        }
    }

    public void PlayHoverSound()
    {
        // Plays the UI hover sound effect
        if(hoverSound != null && soundEffect != null)
        {
            soundEffect.PlayOneShot(hoverSound);
        }
    }
    private void PlayClickSound()
    {
        Time.timeScale = 1f; // Important: Unpause time so animations/fades can run

        // Plays the UI click sound effect
        if(clickSound != null && soundEffect != null)
        {
            soundEffect.PlayOneShot(clickSound);
        }
    }
}
