using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneManagerB : SceneManagerBase
{
    protected override void Start()
    {
        base.Start(); 
        FadeIn(DEFAULT_FADE_DURATION);
    }

    protected override void Update()
    {
        base.Update();
    }
}
