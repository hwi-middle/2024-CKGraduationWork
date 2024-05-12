using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupHandler : Singleton<PopupHandler>
{
    private enum EPopupType
    {
        None,
        Info,
        Confirm,
        Warning,
        Error,
        Tutorial
    }

    private GameObject _popupPrefab;
    private GameObject _tutorialVideoRawImage;

    private EPopupType _currentType = EPopupType.None;
    
    private TMP_Text _typeText;
    private TMP_Text _title;
    private TMP_Text _description;
    private TMP_Text _positiveText;
    private TMP_Text _negativeText;

    private Action<bool> buttonAction;
    
    public bool IsPopupActive => _popupPrefab.activeSelf;

    private void Awake()
    {
        _popupPrefab = Instantiate(Resources.Load<GameObject>("PopupCanvas"));
        Transform popupImage = _popupPrefab.transform.GetChild(0);
        _typeText = popupImage.GetChild(0).GetComponentInParent<TMP_Text>();

        // Title Object
        _title = popupImage.GetChild(1).GetComponent<TMP_Text>();
        
        // Description Object
        _description = popupImage.GetChild(2).GetComponent<TMP_Text>();
        
        // Index 3 is Video Player Raw Image
        _tutorialVideoRawImage = popupImage.GetChild(3).gameObject;
        
        // Positive Object
        _positiveText = popupImage.GetChild(4).GetComponentInChildren<TMP_Text>();

        // Negative Object
        _negativeText = popupImage.GetChild(5).GetComponentInChildren<TMP_Text>();
        
        _popupPrefab.SetActive(false);
    }

    /// <summary>
    /// 버튼이 하나만 있는 팝업을 띄웁니다. 알림 메시지를 띄울 때 사용 합니다.
    /// </summary>
    /// <param name="action">User의 버튼 입력 정보를 받을 Handler</param>
    /// <param name="title">팝업의 타이틀</param>
    /// <param name="description">팝업의 내용</param>
    /// <param name="positive">확인 버튼</param>
    public void DisplayInfoPopup(Action<bool> action, string title, string description, string positive)
    {
        buttonAction += action;
        _currentType = EPopupType.Info;
        SetPopupTextAndDisplayPopup(title, description, positive);
    }

    /// <summary>
    /// 버튼이 두 개인 팝업을 띄웁니다. User의 선택이 필요 할 경우 사용합니다.
    /// </summary>
    /// <param name="action">User의 버튼 입력 정보를 받을 Handler</param>
    /// <param name="title">팝업의 타이틀</param>
    /// <param name="description">팝업의 내용</param>
    /// <param name="positive">확인 버튼</param>
    /// <param name="negative">취소 버튼</param>
    public void DisplayConfirmPopup(Action<bool> action, string title, string description, string positive,
        string negative)
    {
        buttonAction += action;
        _currentType = EPopupType.Confirm;
        SetPopupTextAndDisplayPopup(title, description, positive, negative);
    }

    /// <summary>
    /// 경고 문구를 띄우는 팝업입니다. User의 선택을 받을 수도 안받을 수도 있습니다.
    /// </summary>
    /// <param name="action">User의 버튼 입력 정보를 받을 Handler</param>
    /// <param name="title">팝업의 타이틀</param>
    /// <param name="description">팝업의 내용</param>
    /// <param name="positive">확인 버튼</param>
    /// <param name="negative">취소 버튼 (Default = "")</param>
    public void DisplayWarningPopup(Action<bool> action, string title, string description, string positive,
        string negative="")
    {
        buttonAction += action;
        _currentType = EPopupType.Warning;
        SetPopupTextAndDisplayPopup(title, description, positive, negative);
    }

    /// <summary>
    /// 에러 문구를 띄우는 팝업입니다. 확인 버튼 입력 시 게임을 강제로 종료합니다.
    /// </summary>
    /// <param name="action">User의 버튼 입력 정보를 받을 Handler</param>
    /// <param name="title">팝업의 타이틀</param>
    /// <param name="description">팝업의 내용</param>
    /// <param name="positive">확인 버튼</param>
    public void DisplayErrorPopup(Action<bool> action, string title, string description, string positive)
    {
        buttonAction += action;
        _currentType = EPopupType.Error;
        SetPopupTextAndDisplayPopup(title, description, positive);
    }
    
    public void DisplayTutorialPopup(Action<bool> action, string title, string description, string positive)
    {
        SceneManagerBase.Instance.TogglePause();
        buttonAction += action;
        _currentType = EPopupType.Tutorial;
        SetPopupTextAndDisplayPopup(title, description, positive);
    }

    private void SetPopupTextAndDisplayPopup(string title, string description, string positive, string negative = "")
    {
        switch (_currentType)
        {
            case EPopupType.Info or EPopupType.Tutorial:
                _typeText.text = "알림";
                break;
            case EPopupType.Confirm:
                _typeText.text = "확인";
                break;
            case EPopupType.Warning:
                _typeText.text = "경고";
                break;
            case EPopupType.Error:
                _typeText.text = "오류";
                break;
            case EPopupType.None:
            default:
                Debug.Assert(false);
                break;
        }

        _title.text = title;
        _description.text = description;
        _positiveText.text = positive;
        _negativeText.text = negative;

        _negativeText.transform.parent.gameObject.SetActive(!negative.Equals(string.Empty));
        _tutorialVideoRawImage.SetActive(_currentType is EPopupType.Tutorial);
        
        _popupPrefab.SetActive(true);
    }

    public void ExecuteActionOnButtonClick(bool isPositive)
    {
        buttonAction?.Invoke(isPositive);
        ClosePopup();
    }

    public void ClosePopup()
    {
        buttonAction = null;
        _popupPrefab.SetActive(false);
    }
}
