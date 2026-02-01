using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [Header("Configuration")]
    public AudioClip musicBoss; // The music clip to switch to
    public GameplaySystem gameplaySystem; // Reference to the audio manager
    [Header("Internal Settings")]
    private bool isTriggered = false; // Ensures the trigger only works once

    //-----------------------------------------------------------------------------------------
    //UNITY EVENTS
    private void Start()
    {
        // If I didn't assign the system in the Inspector, find it automatically
        if(gameplaySystem == null) gameplaySystem = FindObjectOfType<GameplaySystem>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for Player tag and strict boolean check
        if(other.CompareTag("Player") && !isTriggered)
        {
            ActivateMusic(); // Run the logic to switch the music
        }
    }

    //-----------------------------------------------------------------------------------------
    //PRIVATE METHODS
    private void ActivateMusic()
    {
        // Safety check before calling the system to avoid errors
        if(gameplaySystem != null && musicBoss != null)
        {
            isTriggered = true; // Lock the trigger

            // Call the crossfade logic in GameplaySystem
            gameplaySystem.SwitchMusic(musicBoss);
        }
    }
}
