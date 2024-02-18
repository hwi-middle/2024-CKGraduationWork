using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : Singleton<SceneLoader>
{
    public bool IsLoaded { get; private set; } = false;

    [SerializeField] private Image _progressBar;
    [SerializeField] private TMP_Text _progressText;
    private string _loadSceneName;

    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        _progressBar.fillAmount = 0f;
        SceneManager.sceneLoaded += LoadSceneEnd;
        _loadSceneName = sceneName;
        _progressText.text = "0%";
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress >= 0.9f)
        {
            _progressBar.fillAmount = op.progress / 0.9f;
            _progressText.text = $"{(int)(_progressBar.fillAmount * 100)}%";
            yield return null;
        }

        _progressBar.fillAmount = 1.0f;
        _progressText.text = "로드 완료!";

        SceneManagerBase sceneManager = SceneManagerBase.Instance;
        sceneManager.FadeOut(SceneManagerBase.DEFAULT_FADE_DURATION);
        while (sceneManager.IsFading)
        {
            yield return null;
        }
        
        op.allowSceneActivation = true;
    }

    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Assert(scene.name == _loadSceneName);
        Debug.Assert(loadSceneMode == LoadSceneMode.Single);
        SceneManager.sceneLoaded -= LoadSceneEnd;
        IsLoaded = true;
    }
}
