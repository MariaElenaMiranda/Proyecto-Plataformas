using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictorySystem : BaseSceneManager
{
    void Awake()
    {
        BaseAwake();
    }

    void Start()
    {
        BaseStart();
    }

    public void NewGame()
    {
        ChangeScene("MapTest");
    }

    public void MainMenu()
    {
        ChangeScene("MainMenu");
    }
}
