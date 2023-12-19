using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainLobbySceneManager : SceneManagerBase<MainLobbySceneManager>
{
    [SerializeField] private TextMeshProUGUI versionText;
    [SerializeField] private GameObject warningText;
    private SceneFadeManager _sceneFadeManager;

    private void Awake()
    {
        Instantiate(Resources.Load<GameObject>("FadeCanvas"));
        _sceneFadeManager = SceneFadeManager.Instance;

#if __BUILD_ON_CLOUD
        warningText.SetActive(true);
        versionText.text = $"Version: {Application.version} (Automated Build)";
#else
        versionText.text = $"Version: {Application.version}";
#endif
    }

    protected override void Start()
    {
        base.Start();
        _sceneFadeManager.FadeIn();
    }

    protected override void Update()
    {
        base.Update();
    }
}
