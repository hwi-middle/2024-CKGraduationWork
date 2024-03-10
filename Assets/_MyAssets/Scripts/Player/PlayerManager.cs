using System;
using System.Collections;
using UnityEngine;

namespace _MyAssets.Scripts.Player
{
    public enum EPlayerState
    {
        Idle,
        Ready,
        Attacking
    }

    public class PlayerManager : Singleton<PlayerManager>
    {
        private IEnumerator _returnToIdleState;

        public EPlayerState CurrentState { get; private set; }

        private void Update()
        {
            ReturnToIdleState();
        }

        public void ChangePlayerState(EPlayerState nextState)
        {
            CurrentState = nextState;
        }

        public void RotatePlayerFromScissor(Quaternion targetRotation)
        {
            const float IMMEDIATE_ROTATE_SPEED = 1.0f;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, IMMEDIATE_ROTATE_SPEED);
        }

        private void ReturnToIdleState()
        {
            switch (CurrentState)
            {
                case EPlayerState.Idle:
                    return;
                
                case EPlayerState.Ready when _returnToIdleState == null:
                    _returnToIdleState = ReturnToIdleStateRoutine();
                    StartCoroutine(_returnToIdleState);
                    return;
                
                case EPlayerState.Ready when _returnToIdleState != null:
                    return;
                
                case EPlayerState.Attacking when _returnToIdleState != null:
                    StopCoroutine(_returnToIdleState);
                    _returnToIdleState = null;
                    return;
                case EPlayerState.Attacking when _returnToIdleState == null:
                    return;
                
                default:
                    Debug.Assert(false);
                    return;
            }
        }

        private IEnumerator ReturnToIdleStateRoutine()
        {
            const float RETURN_TIME = 3.0f;
            yield return new WaitForSeconds(RETURN_TIME);
            
            CurrentState = EPlayerState.Idle;
            _returnToIdleState = null;
            ScissorManager.Instance.ReturnToIdle();
        }
    }
}
