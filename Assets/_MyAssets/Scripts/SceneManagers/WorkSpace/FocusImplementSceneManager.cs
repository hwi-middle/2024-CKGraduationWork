using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusImplementSceneManager : SceneManagerBase
{
    protected override void Start()
    {
        base.Start();
        FadeIn(0.5f);
    }

    protected override void Update()
    {
        base.Update();
    }
}
