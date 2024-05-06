using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewPopupData", menuName = "Scriptable Object Asset/Popup Data")]
public class PopupData : ScriptableObject
{
    [Serializable]
    public class Popup
    {
        [Tooltip("Popup의 이름")] public string popupName;
        [Tooltip("Popup의 제목으로 가장 위에 위치 함")] public string title;
        [Tooltip("Popup의 설명으로 경고 문구 등 Popup에서 하고자 하는 말을 작성")] public string description;
        [Tooltip("동의 혹은 긍정의 버튼에 들어 갈 내용")] public string positiveButton;
        [Tooltip("거부 혹은 부정의 버튼에 들어 갈 내용")] public string negativeButton;
    }

    [Tooltip("Popup의 정보를 담고 있는 리스트")] public List<Popup> popupList;
}
