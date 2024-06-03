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
}

public class TutorialTriggerZoneController : MonoBehaviour
{
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
                break;
            case ETutorialVideoIndex.Tutorial_Cube_Rotation:
                break;
            case ETutorialVideoIndex.None:
            default:
                Debug.Assert(false);
                break;
        }
    }
    
    private void HandleCubeSelectTutorialButtonClick(bool isPositive)
    {
        TutorialPopupList.DisplayCubeRotateTutorialPopup(HandleCubeRotateTutorialButtonClick);   
    }
    
    private void HandleCubeRotateTutorialButtonClick(bool isPositive)
    {
        Destroy(gameObject);
    }
}
