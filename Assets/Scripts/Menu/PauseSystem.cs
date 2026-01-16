using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseSystem : MonoBehaviour
{
    [Header("Interface")]
    public GameObject pauseMenu;
    public CanvasGroup blackScreen;
    public float fadeSpeed = 2.0f;

    [Header("Sounds")]
    public AudioSource soundEffect;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private bool isPaused = false; // Controls if the game logic is stopped
    private bool isTransitioning = false; // Prevents button spamming during animations

    void Start()
    {
        // Ensure game runs when scene starts
        Time.timeScale = 1f;

        if(blackScreen != null)
        {
            blackScreen.alpha = 1; // Set screen to black initially
            blackScreen.blocksRaycasts = false; // Allow clicks to pass through
            StartCoroutine(FadeInSequence()); // Start fading in
        }
    }

    void Update()
    {
        if(isTransitioning) return; // Ignore input if changing scenes

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused) Resume();
            else Pause();
        }
    }

    //ResumeButton
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // Resume game time
        isPaused = false;
    }

    //Pause
    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Freeze game time
        isPaused = true;
    }

    //Restart button
    public void Restart()
    {
        if(!isTransitioning)
        {
            // Reload current scene
            StartCoroutine(ChangeSceneSequence(SceneManager.GetActiveScene().name));
        }
    }

    //MainMenu Button
    public void MainMenu()
    {
        if(!isTransitioning)
        {
            // Starts the coroutine to change scene to MainMenu
            StartCoroutine(ChangeSceneSequence("MainMenu"));
        }
    }

    //ExitButton
    public void Exit()
    {
        // Start the exit sequence with sound
        StartCoroutine(ExitSequence());
    }

    public void PlayHoverSound()
    {
        // Plays the UI hover sound effect
        if(hoverSound != null && soundEffect != null)
        {
            soundEffect.PlayOneShot(hoverSound);
        }
    }

//-----------------------------------------------------------------------------------
//COROUTINES:
    IEnumerator FadeInSequence()
    {
        float timer = 1;
        while(timer > 0)
        {
            // Reduces alpha from 1 (black) to 0 (transparent)
            timer -= Time.deltaTime * fadeSpeed;
            blackScreen.alpha = timer;
            yield return null; // Wait for next frame
        }
        blackScreen.alpha = 0; // Ensure alpha is exactly 0
        blackScreen.blocksRaycasts = false;
    }

    IEnumerator FadeOutSequence()
    {
        // Hide pause menu and block inputs
        pauseMenu.SetActive(false);
        blackScreen.blocksRaycasts = true;

        float timer = 0;
        while(timer < 1)
        {
            // Increases alpha from 0 (transparent) to 1 (black)
            timer += Time.deltaTime * fadeSpeed;
            blackScreen.alpha = timer;
            yield return null;
        }
    }

    IEnumerator ChangeSceneSequence(string sceneName)
    {
        isTransitioning = true;
        Time.timeScale = 1f; // Important: Unpause time for animations

        //Click sound:
        if(clickSound != null) soundEffect.PlayOneShot(clickSound);

        //FadeOut:
        yield return StartCoroutine(FadeOutSequence()); // Wait for FadeOut to finish

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator ExitSequence()
    {
        isTransitioning = true;
        // Ensure time runs so WaitForSeconds works (since game might be paused)
        Time.timeScale = 1f;

        if(clickSound != null && soundEffect != null)
        {
            soundEffect.PlayOneShot(clickSound);
             // Wait exactly the length of the audio
            yield return new WaitForSeconds(clickSound.length);
        }else
        {
            yield return new WaitForSeconds(0.5f); // Default wait if no sound
        }

        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
