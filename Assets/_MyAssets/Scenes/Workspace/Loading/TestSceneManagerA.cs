using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneManagerA : SceneManagerBase
{
    private bool _isLoadStarted = false;
    
    protected override void Start()
    {
        base.Start();
        FadeIn(DEFAULT_FADE_DURATION);
    }

    protected override void Update()
    {
        base.Update();
        if (!Input.GetKeyDown(KeyCode.Space) || _isLoadStarted)
        {
            return;
        }
        
        _isLoadStarted = true;
        LoadSceneWithLoadingUI("B");
    }
}
