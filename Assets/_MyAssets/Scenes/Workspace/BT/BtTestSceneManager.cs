using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtTestSceneManager : SceneManagerBase
{
    protected override void Start()
    {
        base.Start();
        FadeIn(0.5f);
        Player.Instance.GetComponent<ItemThrowHandler>().GetItem();
    }

    protected override void Update()
    {
        base.Update();
    }
}
