using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo7WeekSceneManager : SceneManagerBase
{
    protected override void Start()
    {
        base.Start();
        _player = Player.Instance;
        FadeIn(0.5f);
    }

    protected override void Update()
    {
        base.Update();
        ExecuteDeadSequence();

        if (IsDebugMode)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                LoadSceneWithLoadingUI("Respawn2");
            }
        }
    }
}
