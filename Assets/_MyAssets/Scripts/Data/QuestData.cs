using System;
using System.Collections.Generic;
using UnityEngine;

// 확장 가능성을 고려하여 별도 클래스로 구현
[Serializable]
public class Quest
{
    [Tooltip("퀘스트 내용")] public string description;
}

[CreateAssetMenu(fileName = "NewQuestData", menuName = "Scriptable Object Asset/Quest Data")]
public class QuestData : ScriptableObject
{
    [Tooltip("퀘스트 정보 배열")] public Quest[] quests;
}
