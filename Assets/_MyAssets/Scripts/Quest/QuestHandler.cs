using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestHandler : Singleton<QuestHandler>
{
    [SerializeField] private QuestData _questData;

    [SerializeField] private TMP_Text _questText;
    [SerializeField] private Image _tutorialImage;
    [SerializeField] private float _questFadeDuration;

    private readonly List<QuestZone> _questZones = new List<QuestZone>();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).GetComponent<QuestZone>();
            child.quest = _questData.quests[i];
            _questZones.Add(child);
        }
    }

    public void ShowQuestText(string text)
    {
        StartCoroutine(ShowQuestTextRoutine(text));
    }

    private IEnumerator ShowQuestTextRoutine(string text)
    {
        Color color = _questText.color;
        float t = 0f;

        // 페이드 아웃
        while (t < _questFadeDuration)
        {
            color.a = 1 - (t / _questFadeDuration);
            _questText.color = color;
            yield return null;
            t += Time.deltaTime;
        }

        color.a = 0f;
        _questText.color = color;
        _questText.text = text;
        t = 0f;

        // 페이드 인
        while (t < _questFadeDuration)
        {
            color.a = t / _questFadeDuration;
            _questText.color = color;
            yield return null;
            t += Time.deltaTime;
        }

        color.a = 1f;
        _questText.color = color;
    }

    public void ShowQuestImage(Sprite spriteOrNull)
    {
        if (_tutorialImage.sprite == null)
        {
            return;
        }

        StartCoroutine(ShowQuestImageRoutine(spriteOrNull));
    }

    private IEnumerator ShowQuestImageRoutine(Sprite spriteOrNull)
    {
        Color color = _tutorialImage.color;
        float t = 0f;

        // 페이드 아웃
        while (t < _questFadeDuration)
        {
            color.a = 1 - (t / _questFadeDuration);
            _tutorialImage.color = color;
            yield return null;
            t += Time.deltaTime;
        }
        
        color.a = 0f;
        _tutorialImage.color = color;
        _tutorialImage.sprite = spriteOrNull;
        t = 0f;

        if (_tutorialImage.sprite == null)
        {
            yield break;
        }
        
        // 페이드 인
        while (t < _questFadeDuration)
        {
            color.a = t / _questFadeDuration;
            _tutorialImage.color = color;
            yield return null;
            t += Time.deltaTime;
        }

        color.a = 1f;
        _tutorialImage.color = color;
    }
}
