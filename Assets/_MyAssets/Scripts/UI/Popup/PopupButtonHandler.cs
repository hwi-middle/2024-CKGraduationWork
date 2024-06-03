using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PopupButtonHandler : MonoBehaviour
{
    [SerializeField] private Button _positiveButton;
    [SerializeField] private Button _negativeButton;
    
    private void Awake()
    {
        _positiveButton.onClick.AddListener(OnPositiveButtonClick);
        _negativeButton.onClick.AddListener(OnNegativeButtonClick);
    }

    private void OnPositiveButtonClick()
    {
        PopupHandler.Instance.ExecuteActionOnButtonClick(true);
    }
    
    private void OnNegativeButtonClick()
    {
        PopupHandler.Instance.ExecuteActionOnButtonClick(false);
    }
}
