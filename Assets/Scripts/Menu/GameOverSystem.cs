using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSystem : BaseSceneManager
{
    void Awake()
    {
        BaseAwake();
    }

    void Start()
    {
        BaseStart();
    }

    //MainMenu Button
    public void MainMenu()
    {
        ChangeScene("MainMenu");
    }

    //Restart Button
    public void RestartGame()
    {
        ChangeScene("MapTest");
    }
}
