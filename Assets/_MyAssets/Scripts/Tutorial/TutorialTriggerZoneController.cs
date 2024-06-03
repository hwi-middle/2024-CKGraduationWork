using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TutorialPopupList
{
    public static void DisplayCubeSelectTutorialPopup(Action<bool> action)
    {
        PopupHandler.Instance.DisplayTutorialPopup("큐브 선택", "W, S키를 이용해 큐브를 선택합니다.", "확인",
            ETutorialVideoIndex.Tutorial_Cube_Select, action, true);
    }

    public static void DisplayCubeRotateTutorialPopup(Action<bool> action)
    {
        PopupHandler.Instance.DisplayTutorialPopup("큐브 회전", "A, D키를 이용해 큐브를 회전합니다.", "확인",
            ETutorialVideoIndex.Tutorial_Cube_Rotation, action);
    }

    public static void DisplayItemAimingTutorialPopup(Action<bool> action)
    {
        PopupHandler.Instance.DisplayTutorialPopup("아이템 조준", "우클릭을 누르고 있으면 조준을 합니다.", "확인",
            ETutorialVideoIndex.Tutorial_Item_Aiming, action, true);
    }
    
    public static void DisplayItemThrowingTutorialPopup(Action<bool> action)
    {
        PopupHandler.Instance.DisplayTutorialPopup("아이템 던지기", "조준 한 상태에서 좌클릭을 누르면 아이템을 던집니다.", "확인",
            ETutorialVideoIndex.Tutorial_Item_Throwing, action);
    }
}

public class TutorialTriggerZoneController : MonoBehaviour
{
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private ETutorialVideoIndex _tutorialVideo;
    
    private void Awake()
    {
        Debug.Assert(_tutorialVideo is not ETutorialVideoIndex.None, "Tutorial not Exist");
        GetComponent<MeshRenderer>().enabled = SceneManagerBase.Instance.IsDebugMode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        switch (_tutorialVideo)
        {
            case ETutorialVideoIndex.Tutorial_Cube_Select:
                TutorialPopupList.DisplayCubeSelectTutorialPopup(HandleCubeSelectTutorialButtonClick);
                _inputData.clairvoyanceEvent += HandlePopupCloseAction;
                break;
            case ETutorialVideoIndex.Tutorial_Cube_Rotation:
                break;
            case ETutorialVideoIndex.Tutorial_Item_Aiming:
                TutorialPopupList.DisplayItemAimingTutorialPopup(HandleItemAimingTutorialButtonClick);
                _inputData.clairvoyanceEvent += HandlePopupCloseAction;
                break;
            case ETutorialVideoIndex.Tutorial_Item_Throwing:
                break;
            case ETutorialVideoIndex.None:
            default:
                Debug.Assert(false);
                break;
        }
    }

    private void HandlePopupCloseAction()
    {
        PopupHandler.Instance.ExecuteActionOnButtonClick(true);
    }
    
    // Cube Tutorials
    private void HandleCubeSelectTutorialButtonClick(bool isPositive)
    {
        _inputData.clairvoyanceEvent -= HandlePopupCloseAction;
        TutorialPopupList.DisplayCubeRotateTutorialPopup(HandleCubeRotateTutorialButtonClick);
        _inputData.clairvoyanceEvent += HandlePopupCloseAction;
    }
    
    private void HandleCubeRotateTutorialButtonClick(bool isPositive)
    {
        _inputData.clairvoyanceEvent -= HandlePopupCloseAction;
        Destroy(gameObject);
    }
    
    // Item Tutorials
    private void HandleItemAimingTutorialButtonClick(bool isPositive)
    {
        _inputData.clairvoyanceEvent -= HandlePopupCloseAction;
        TutorialPopupList.DisplayItemThrowingTutorialPopup(HandleItemThrowingTutorialButtonClick);
        _inputData.clairvoyanceEvent += HandlePopupCloseAction;
    }
    
    private void HandleItemThrowingTutorialButtonClick(bool isPositive)
    {
        _inputData.clairvoyanceEvent -= HandlePopupCloseAction;
        Destroy(gameObject);
    }
}
