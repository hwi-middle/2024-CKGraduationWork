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
        Error
    }

    private GameObject _popupPrefab;

    private EPopupType _currentType = EPopupType.None;
    
    private TMP_Text _typeText;
    private TMP_Text _title;
    private TMP_Text _description;
    private TMP_Text _positiveButton;
    private TMP_Text _negativeButton;

    private Action<bool> buttonAction;
    
    public bool IsPopupActive => _popupPrefab.activeSelf;

    private void Awake()
    {
        _popupPrefab = Instantiate(Resources.Load<GameObject>("PopupCanvas"));
        Transform popupImage = _popupPrefab.transform.GetChild(0);
        _typeText = popupImage.GetChild(0).GetComponentInParent<TMP_Text>();

        // Title Object
        _title = popupImage.GetChild(1).GetComponent<TMP_Text>();
        
        // Subscription Object
        _description = popupImage.GetChild(2).GetComponent<TMP_Text>();
        
        // Positive Object
        _positiveButton = popupImage.GetChild(3).GetComponentInChildren<TMP_Text>();

        // Negative Object
        _negativeButton = popupImage.GetChild(4).GetComponentInChildren<TMP_Text>();
        
        _popupPrefab.SetActive(false);
    }

    public void FloatInfoPopup(Action<bool> action, string title, string description, string positive,
        string negative = "")
    {
        buttonAction += action;
        _currentType = EPopupType.Info;
        SetPopupText(title, description, positive, negative);
    }

    public void FloatConfirmPopup(Action<bool> action, string title, string description, string positive,
        string negative)
    {
        buttonAction += action;
        _currentType = EPopupType.Confirm;
        SetPopupText(title, description, positive, negative);
    }

    public void FloatWarningPopup(Action<bool> action, string title, string description, string positive,
        string negative)
    {
        buttonAction += action;
        _currentType = EPopupType.Warning;
        SetPopupText(title, description, positive, negative);
    }

    public void FloatErrorPopup(Action<bool> action, string title, string description, string positive,
        string negative = "")
    {
        buttonAction += action;
        _currentType = EPopupType.Error;
        SetPopupText(title, description, positive, negative);
    }

    private void SetPopupText(string title, string description, string positive, string negative)
    {
        switch (_currentType)
        {
            case EPopupType.Info:
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
        _positiveButton.text = positive;
        _negativeButton.text = negative;

        _negativeButton.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(negative));
        
        _popupPrefab.SetActive(true);
    }

    public void SetButtonState(bool state)
    {
        buttonAction?.Invoke(state);
        buttonAction = null;
        _popupPrefab.SetActive(false);
    }
}
