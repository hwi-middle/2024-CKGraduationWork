using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainLobbySceneManager : SceneManagerBase
{
    [SerializeField] private TextMeshProUGUI _versionText;
    [SerializeField] private GameObject _warningText;

//    private void Awake()
//    {
//        Instantiate(Resources.Load<GameObject>("FadeCanvas"));
//#if __BUILD_ON_CLOUD
//        _warningText.SetActive(true);
//        _versionText.text = $"Version: {Application.version} (Automated Build)";
//#else
//        _versionText.text = $"Version: {Application.version}";
//#endif
//    }

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
