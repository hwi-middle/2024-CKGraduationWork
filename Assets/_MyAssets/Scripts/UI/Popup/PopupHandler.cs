using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupHandler : Singleton<PopupHandler>
{
    private PopupData _popupData;
    private GameObject _popupPrefab;

    private TMP_Text _title;
    private TMP_Text _description;
    private TMP_Text _positiveButton;
    private TMP_Text _negativeButton;

    private Action<bool> buttonAction;
    
    public bool IsPopupActive => _popupPrefab.activeSelf;

    private void Awake()
    {
        _popupData = SceneManagerBase.Instance.PopupData;
        _popupPrefab = Instantiate(Resources.Load<GameObject>("PopupCanvas"));
        Transform popupImage = _popupPrefab.transform.GetChild(0);

        // Title Object
        _title = popupImage.GetChild(0).GetComponent<TMP_Text>();
        
        // Subscription Object
        _description = popupImage.GetChild(1).GetComponent<TMP_Text>();
        
        // Positive Object
        _positiveButton = popupImage.GetChild(2).GetComponentInChildren<TMP_Text>();

        // Negative Object
        _negativeButton = popupImage.GetChild(3).GetComponentInChildren<TMP_Text>();
        
        _popupPrefab.SetActive(false);
    }

    public void SubscribeButtonAction(Action<bool> action)
    {
        buttonAction += action;
    }
    
    public void UnsubscribeButtonAction(Action<bool> action)
    {
        buttonAction -= action;
    }
    
    public bool ShowPopup(string popupName)
    {
        PopupData.Popup popup = FindPopupData(popupName);
        
        if (popup == null)
        {
            return false;
        }
        
        _title.text = popup.title.Replace("\\n", "\n");
        _description.text = popup.description.Replace("\\n", "\n");
        _positiveButton.text = popup.positiveButton.Replace("\\n", "\n");
        _negativeButton.text = popup.negativeButton.Replace("\\n", "\n");

        _popupPrefab.SetActive(true);
        return true;
    }

    private PopupData.Popup FindPopupData(string popupName)
    {
        Debug.Assert(_popupData != null,"_popupData != null");
        Debug.Assert(_popupData.popupList != null, "_popupData.popupList != null");
        
        foreach (var data in _popupData.popupList)
        {
            if (data.popupName == popupName)
            {
                return data;
            }
        }
        
        return null;
    }

    public void SetButtonState(bool state)
    {
        buttonAction?.Invoke(state);
        _popupPrefab.SetActive(false);
    }
}
