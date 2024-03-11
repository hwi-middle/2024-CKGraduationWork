using System;
using System.Collections;
using UnityEngine;

namespace _MyAssets.Scripts.Player
{
    public enum EPlayerState
    {
        Idle,
        Ready,
        Attacking,
        Defense
    }

    public class PlayerManager : Singleton<PlayerManager>
    {
        private IEnumerator _returnToIdleState;

        public EPlayerState CurrentState { get; private set; }

        private void Start()
        {
        }

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
            if (CurrentState != EPlayerState.Ready)
            {
                if (_returnToIdleState == null)
                {
                    return;
                }
                
                StopCoroutine(_returnToIdleState);
                _returnToIdleState = null;
                return;
            }

            if (_returnToIdleState != null)
            {
                return;
            }

            _returnToIdleState = ReturnToIdleStateRoutine();
            StartCoroutine(_returnToIdleState);
        }

        private IEnumerator ReturnToIdleStateRoutine()
        {
            const float RETURN_TIME = 10.0f;
            yield return new WaitForSeconds(RETURN_TIME);
            
            CurrentState = EPlayerState.Idle;
            _returnToIdleState = null;
            ScissorManager.Instance.ReturnToIdle();
        }
    }
}
