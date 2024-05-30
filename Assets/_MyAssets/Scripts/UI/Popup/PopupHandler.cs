using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

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

    [SerializeField] private TutorialVideoData _tutorialVideoData;

    private GameObject _popupPrefab;
    private GameObject _tutorialVideoRawImage;
    private GameObject _buttonsImage;

    private EPopupType _currentType = EPopupType.None;
    
    private TMP_Text _title;
    private TMP_Text _description;
    private TMP_Text _positiveText;
    private TMP_Text _negativeText;

    private VideoPlayer _videoPlayer;

    private Action<bool> buttonAction;
    
    public bool IsPopupActive => _popupPrefab.activeSelf;

    private bool IsTutorialPopup => _currentType == EPopupType.Tutorial;
        
    private void Awake()
    {
        _popupPrefab = Instantiate(Resources.Load<GameObject>("PopupCanvas"));
        Transform popupImage = _popupPrefab.transform.GetChild(0);
        
        // --- Canvas Child ---
        
        // Video Player for Tutorial
        _videoPlayer = _popupPrefab.transform.GetChild(1).GetComponent<VideoPlayer>();
        
        // --- Popup Image Child ---
        
        // Title Object
        _title = popupImage.GetChild(0).GetComponent<TMP_Text>();
        
        // Description Object
        _description = popupImage.GetChild(1).GetComponent<TMP_Text>();
        
        // Index 3 is Video Player Raw Image
        _tutorialVideoRawImage = popupImage.GetChild(2).gameObject;
        
        // --- Buttons ---
        
        // Buttons Background
        _buttonsImage = popupImage.GetChild(3).gameObject;
        
        // Positive Object
        _positiveText = _buttonsImage.transform.GetChild(0).GetComponentInChildren<TMP_Text>();

        // Negative Object
        _negativeText = _buttonsImage.transform.GetChild(1).GetComponentInChildren<TMP_Text>();
        
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
    
    public void DisplayTutorialPopup(string title, string description, string positive, ETutorialVideoIndex index,
        Action<bool> action = null)
    {
        SceneManagerBase.Instance.TogglePause();
        buttonAction += action;
        _currentType = EPopupType.Tutorial;
        SetTextAndDisplayTutorialPopup(title, description, positive, index);
    }

    private void SetPopupTextAndDisplayPopup(string title, string description, string positive, string negative = "")
    {
        _title.text = title;
        _description.text = description;
        _positiveText.text = positive;
        _negativeText.text = negative;

        _description.gameObject.SetActive(!description.Equals(string.Empty));
        _negativeText.transform.parent.gameObject.SetActive(!negative.Equals(string.Empty));
        
        _tutorialVideoRawImage.SetActive(false);
        _popupPrefab.SetActive(true);
    }
    
    private void SetTextAndDisplayTutorialPopup(string title, string description, string positive, ETutorialVideoIndex index)
    {
        _title.text = title;
        _description.text = description;
        _positiveText.text = positive;

        // Index 0 is None -> index - 1 is the correct index
        _videoPlayer.clip = _tutorialVideoData.tutorialVideos[(int)index - 1].videoClip;
        
        _negativeText.transform.parent.gameObject.SetActive(false);
        _tutorialVideoRawImage.SetActive(true);
        _popupPrefab.SetActive(true);
    }

    public void ExecuteActionOnButtonClick(bool isPositive)
    {
        AudioPlayManager.Instance.PlayOnceSfxAudio(ESfxAudioClipIndex.UI_Click);
        buttonAction?.Invoke(isPositive);
        ClosePopup();
    }

    private void ClosePopup()
    {
        if (IsTutorialPopup)
        {
            SceneManagerBase.Instance.TogglePause();
        }
        
        AudioPlayManager.Instance.PlayOnceSfxAudio(ESfxAudioClipIndex.UI_Close);
        buttonAction = null;
        _popupPrefab.SetActive(false);
    }
}
