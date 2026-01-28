using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [Header("Configuration")]
    public AudioClip musicBoss;
    public GameplaySystem gameplaySystem;

    private bool isTriggered = false; // Ensures the trigger only works once

    private void Start()
    {
        // If I didn't assign the system in the Inspector, find it automatically
        if(gameplaySystem == null)
        {
            gameplaySystem = FindObjectOfType<GameplaySystem>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for Player tag and strict boolean check
        if(other.CompareTag("Player") && !isTriggered)
        {
            ActivateMusic();
        }
    }

    private void ActivateMusic()
    {
        // Safety check before calling the system to avoid errors
        if(gameplaySystem != null && musicBoss != null)
        {
            isTriggered = true;
            gameplaySystem.SwitchMusic(musicBoss);
        }
    }
}
