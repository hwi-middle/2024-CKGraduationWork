using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditSceneManager : SceneManagerBase
{
    private float _originalYPos;
    [SerializeField] private float _targetYPos;
    [SerializeField]  private float _scrollDuration;
    [SerializeField] private RectTransform _creditText;
    private float _elapsedTime = 0f;

    protected override void Awake()
    {
        base.Awake();
        _originalYPos = _creditText.anchoredPosition.y;
    }

    protected override void Start()
    {
        base.Start();
        FadeIn(0.5f);
        AudioPlayManager.Instance.PlayOnceSfxAudio(ESfxAudioClipIndex.Credit_BGM);
    }

    protected override void Update()
    {
        base.Update();
        
        Vector2 textPos = _creditText.anchoredPosition;
        textPos.y = Mathf.Lerp(_originalYPos, _targetYPos, _elapsedTime / _scrollDuration);
        _creditText.anchoredPosition = textPos;
        _elapsedTime += Time.deltaTime;
    }
}
