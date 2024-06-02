using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAnimationEvents : MonoBehaviour
{
    public void OnThrowReadyAnimationEnd()
    {
        PlayerStateManager.Instance.AddPlayerState(EPlayerState.ItemHold);
        PlayerStateManager.Instance.RemovePlayerState(EPlayerState.ItemReady);
        
        CameraController.Instance.ChangeCameraFromFollowToAiming();
        PlayerMove.Instance.AlignPlayerToCameraForward();
        StartCoroutine(CameraBlendingRoutine());
    }

    public void OnThrowReadyAnimationStart()
    {
        AudioPlayManager.Instance.PlayOnceSfxAudio(ESfxAudioClipIndex.Noise_Item1);
    }

    public void OnThrowAnimationEnd()
    {
        PlayerStateManager.Instance.RemovePlayerState(EPlayerState.ItemThrow);
        ItemThrowHandler.Instance.ThrowItem();
    }

    private IEnumerator CameraBlendingRoutine()
    {
        yield return new WaitForEndOfFrame();

        while (CameraController.Instance.IsBlending)
        {
            yield return null;
        }

        ItemThrowHandler.Instance.IsOnAiming = true;
    }
}
