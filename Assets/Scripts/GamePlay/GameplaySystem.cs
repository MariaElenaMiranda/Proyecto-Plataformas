using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplaySystem : MonoBehaviour
{
    [Header("Visual Settings")]
    public CanvasGroup blackScreen;
    public float fadeSpeed = 2.0f;

    [Header("Audio Settings")]
    public AudioSource mainThemeMusic;
    public float pausedVolume = 0.3f;
    private float defaultVolume = 1f;

    private bool isTransitioning = false; // Flag to prevent double actions during transitions

    void Start()
    {
        if(mainThemeMusic != null)
        {
            mainThemeMusic.loop = true;
            mainThemeMusic.volume = 0; // Start silent for Fade In
            defaultVolume = 1f;

            if(!mainThemeMusic.isPlaying) mainThemeMusic.Play();
        }

        if(blackScreen != null)
        {
            blackScreen.alpha = 1; // Set screen to black initially
            blackScreen.blocksRaycasts = false; // Allow clicks to pass through
            StartCoroutine(FadeInSequence()); // Start fading in
        }
    }

    // To change the background music for the Boss
    public void SwitchMusic(AudioClip newMusicClip)
    {
        if(mainThemeMusic.clip != newMusicClip)
        {
            StartCoroutine(SwapMusic(newMusicClip));
        }
    }

    // Pausing / Unpausing
    public void NotifyPause(bool isPaused)
    {
        // Determine target volume based on pause state
        float targetVolume = isPaused ? pausedVolume : defaultVolume;
        // Start volume fade coroutine independent of screen fade
        StartCoroutine(FadeAudioTo(targetVolume));
    }

    // Call this to exit the level (Win, Lose)
    public void ChangeScene(string sceneName)
    {
        if(!isTransitioning) StartCoroutine(ChangeSceneSequence(sceneName));
    }


//-----------------------------------------------------------------------------------
//COROUTINES:

    IEnumerator FadeInSequence()
    {
        // Ensure music starts playing
        if(mainThemeMusic != null && !mainThemeMusic.isPlaying) mainThemeMusic.Play();

        float timer = 1;
        while(timer > 0)
        {
            // Reduces alpha from 1 (black) to 0 (transparent)
            timer -= Time.deltaTime * fadeSpeed;
            blackScreen.alpha = timer;

            // Volume goes up as Alpha goes down
            if(mainThemeMusic != null) mainThemeMusic.volume = 1 - timer;

            yield return null; // Wait for next frame
        }

        blackScreen.alpha = 0; // Ensure alpha is exactly 0
        blackScreen.blocksRaycasts = false;

        if(mainThemeMusic != null) mainThemeMusic.volume = 1f; // Max volume
    }

    IEnumerator FadeOutSequence()
    {
        // Block clicks during fade out
        if(blackScreen != null) blackScreen.blocksRaycasts = true;

        float timer = 0;
        while(timer < 1)
        {
            // Increases alpha from 0 (transparent) to 1 (black)
            timer += Time.deltaTime * fadeSpeed;
            blackScreen.alpha = timer;

            // Volume goes down as Alpha goes up
            if(mainThemeMusic != null) mainThemeMusic.volume = 1 - timer;

            yield return null;
        }
        if(mainThemeMusic != null) mainThemeMusic.volume = 0f; // Silence
    }

    IEnumerator ChangeSceneSequence(string sceneName)
    {
        isTransitioning = true;
        Time.timeScale = 1f; // Important: Unpause time for animations/fades

        yield return StartCoroutine(FadeOutSequence()); // Wait for FadeOut to finish

        SceneManager.LoadScene(sceneName);
    }

    // Used for Pause Menu volume changes
    IEnumerator FadeAudioTo(float endVolume)
    {
        if(mainThemeMusic == null) yield break;
        float startVolume = mainThemeMusic.volume;
        float time = 0;

        while(time < 1)
        {
            // Use unscaledDeltaTime to work even if paused
            time += Time.unscaledDeltaTime * fadeSpeed;
            mainThemeMusic.volume = Mathf.Lerp(startVolume, endVolume, time);
            yield return null;
        }
        mainThemeMusic.volume = endVolume;
    }

    // Handles the sequence: Fade Out -> Change Clip -> Fade In
    IEnumerator SwapMusic(AudioClip newMusicClip)
    {
        yield return StartCoroutine(FadeAudioTo(0)); // Fade out current music
        mainThemeMusic.clip = newMusicClip;
        mainThemeMusic.Play();
        yield return StartCoroutine(FadeAudioTo(defaultVolume)); // Fade in new music
    }
}
