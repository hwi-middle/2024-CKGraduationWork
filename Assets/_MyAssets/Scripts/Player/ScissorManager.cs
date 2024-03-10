using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _MyAssets.Scripts.Player
{
    public class ScissorManager : Singleton<ScissorManager>
    {
        private const float TOLERANCE = 0.1f;
        private const float LERP_SPEED = 0.1f;
        private Camera _mainCamera;
        private IEnumerator _normalAttackWait;
        private IEnumerator _readyToAttackWait;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            
        }
        
        #region About Attack

        public void ReturnToIdle()
        {
            Vector3 endPosition = new Vector3(0, 0, -1);
            Quaternion endRotation = Quaternion.identity;

            StartCoroutine(ReturnToIdleRoutine(endPosition, endRotation));
        }

        private IEnumerator ReturnToIdleRoutine(Vector3 endPosition, Quaternion endRotation)
        {
            while ((transform.localPosition - endPosition).magnitude >= TOLERANCE
                   || Quaternion.Angle(transform.localRotation, endRotation) >= TOLERANCE)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, endPosition, LERP_SPEED);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, endRotation, LERP_SPEED);
                yield return null;
            }
        }
        
        private void ReadyToAttack()
        {
            if (_readyToAttackWait != null)
            {
                return;
            }
            
            Vector3 targetPosition = new Vector3(0.5f, 0, 1);
            Quaternion targetRotation = Quaternion.Euler(90, 0, 0);
            _readyToAttackWait = ReadyToAttackWait(targetPosition, targetRotation, LERP_SPEED);
            StartCoroutine(_readyToAttackWait);
        }

        private IEnumerator ReadyToAttackWait(Vector3 targetPosition, Quaternion targetRotation,float speed)
        {
            while ((transform.localPosition - targetPosition).magnitude >= TOLERANCE
                   || Quaternion.Angle(transform.localRotation, targetRotation) >= TOLERANCE)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, speed);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, speed);
                yield return null;
            }

            PlayerManager.Instance.ChangePlayerState(EPlayerState.Ready);
            _readyToAttackWait = null;
        }

        private void ActivateNormalAttack()
        {
            // Aim 방향으로 공격
            RotatePlayer();
            WaitNormalAttack();
        }

        private void RotatePlayer()
        {
            Debug.Assert(_mainCamera != null, "_mainCamera != null");

            Quaternion cameraRotation = _mainCamera.transform.localRotation;
            cameraRotation.x = 0;
            cameraRotation.z = 0;
            PlayerManager.Instance.RotatePlayerFromScissor(cameraRotation);
        }

        private void WaitNormalAttack()
        {
            if (_normalAttackWait != null)
            {
                return;
            }

            // 임시 공격 거리
            const float TMP_RANGE = 2.0f;
            Vector3 startPosition = new Vector3(0.5f, 0, 1);
            Vector3 endPosition = new Vector3(startPosition.x, 0, startPosition.z + TMP_RANGE);
            
            _normalAttackWait = WaitAttackRoutine(startPosition, endPosition);
            StartCoroutine(_normalAttackWait);
        }

        private IEnumerator WaitAttackRoutine(Vector3 startPosition, Vector3 endPosition)
        {
            while ((transform.localPosition - endPosition).magnitude >= TOLERANCE)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, endPosition, LERP_SPEED);
                yield return null;
            }

            const float COLLIDER_RANGE = 0.5f;
            const float COLLIDER_RADIUS = 0.5f;
            Vector3 checkPosition = new Vector3(transform.localPosition.x, transform.localPosition.y,
                transform.localPosition.z + COLLIDER_RANGE);
            checkPosition = transform.TransformPoint(checkPosition);
            
            Collider[] colliders = Physics.OverlapSphere(checkPosition, COLLIDER_RADIUS);
            // colliders 에 Enemy가 있으면 Attack Call 하는 느낌으로
            
            while ((startPosition - transform.localPosition).magnitude >= TOLERANCE)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, LERP_SPEED);
                yield return null;
            }
            
            _normalAttackWait = null;
            PlayerManager.Instance.ChangePlayerState(EPlayerState.Ready);
        }

        public void OnNormalAttackButtonClick(InputAction.CallbackContext ctx)
        {
            if (ctx.started && PlayerManager.Instance.CurrentState == EPlayerState.Idle)
            {
                ReadyToAttack();
                return;
            }

            if (!ctx.started || _normalAttackWait != null)
            {
                return;
            }

            ActivateNormalAttack();
            PlayerManager.Instance.ChangePlayerState(EPlayerState.Attacking);
        }
        
        #endregion

        private void OnDrawGizmos()
        {
            const float COLLIDER_RANGE = 0.5f;
            const float COLLIDER_RADIUS = 0.5f;
            Vector3 checkPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + COLLIDER_RANGE,
                transform.localPosition.z);

            checkPosition = transform.TransformPoint(checkPosition);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkPosition, COLLIDER_RADIUS);
        }

        private void ActivateScissorJump()
        {
            // 가위를 타고 넘어가는 이동
        }

        private bool ActivateDefense()
        {
            return false;
        }

        private void ActivateCounterAttack()
        {
            
        }
    
        

        public void OnDefenseButtonClick(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
            {
                return;
            }

            ActivateDefense();
        }

        public void OnScissorJumpButtonClick(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
            {
                return;
            }

            ActivateScissorJump();
        }
    }
}
