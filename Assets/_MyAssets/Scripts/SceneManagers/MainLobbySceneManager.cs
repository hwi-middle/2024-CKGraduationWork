using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainLobbySceneManager : SceneManagerBase<MainLobbySceneManager>
{
    [SerializeField] private TextMeshProUGUI versionText;
    private SceneFadeManager _sceneFadeManager;

    private void Awake()
    {
        Instantiate(Resources.Load<GameObject>("FadeCanvas"));
        _sceneFadeManager = SceneFadeManager.Instance;
        
#if __BUILD_ON_CLOUD
        string hash = PlayerPrefs.GetString(PlayerPrefsKeyName.GIT_COMMIT_HASH);
        string hashShort = PlayerPrefs.GetString(PlayerPrefsKeyName.GIT_COMMIT_HASH_SHORT);
        versionText.text = $"Automated build ({hashShort})";
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
