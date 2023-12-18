using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneManager : SceneManagerBase<IntroSceneManager>
{
    private SceneFadeManager _sceneFadeManager;
    
    private void Awake()
    {
        Instantiate(Resources.Load<GameObject>("FadeCanvas"));
        _sceneFadeManager = SceneFadeManager.Instance;
    }

    protected override void Start()
    {
        base.Start();
        _sceneFadeManager.FadeIn(1.0f, null, null);
    }

    protected override void Update()
    {
        base.Update();
    }
}
