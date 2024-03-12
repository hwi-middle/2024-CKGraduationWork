using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _MyAssets.Scripts.Player
{
    public class ScissorManager : Singleton<ScissorManager>
    {
        [SerializeField] private PlayerData _myData;

        private Camera _mainCamera;
        
        private IEnumerator _normalAttackWait;
        private IEnumerator _readyToAttackWait;
        private IEnumerator _defenseWait;

        private const float TOLERANCE = 0.1f;
        private const float LERP_SPEED = 0.2f;
        
        private static readonly Vector3 IDLE_POS = new(0.0f, 0.0f, -1.0f);
        private static readonly Quaternion IDLE_ROT = Quaternion.identity;

        private static readonly Vector3 READY_POS = new(0.5f, 0.0f, 1.0f);
        private static readonly Quaternion READY_ROT = Quaternion.Euler(90.0f, 0.0f, 0.0f);

        private static readonly Vector3 DEFENSE_POS = new(0.0f, 0.0f, 1.0f);
        private static readonly Quaternion DEFENSE_ROT = Quaternion.Euler(0.0f, 0.0f, 45.0f);

        private float _ropeRange;
        private Vector3 _hitPosition;

        private bool CanHangRope
        {
            get
            {
                Debug.Assert(_mainCamera != null, "_mainCamera != null");
                Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                bool isHit = Physics.Raycast(ray, out RaycastHit hit);
                Debug.DrawRay(ray.origin, hit.point, Color.blue);

                if (!isHit)
                {
                    return false;
                }

                if (!hit.transform.CompareTag("RopePoint"))
                {
                    return false;
                }

                _ropeRange = (hit.point - ray.origin).magnitude;
                _hitPosition = hit.transform.position;
                return true;
            }
        }

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
            StartCoroutine(ReturnToIdleRoutine());
        }

        private IEnumerator ReturnToIdleRoutine()
        {
            while ((transform.localPosition - IDLE_POS).magnitude >= TOLERANCE
                   || Quaternion.Angle(transform.localRotation, IDLE_ROT) >= TOLERANCE)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, IDLE_POS, LERP_SPEED);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, IDLE_ROT, LERP_SPEED);
                yield return new WaitForEndOfFrame();
            }
        }

        private void ReadyToAttack()
        {
            if (_readyToAttackWait != null)
            {
                return;
            }

            _readyToAttackWait = ReadyToAttackWaitRoutine();
            StartCoroutine(_readyToAttackWait);
        }

        private IEnumerator ReadyToAttackWaitRoutine()
        {
            while ((transform.localPosition - READY_POS).magnitude >= TOLERANCE
                   || Quaternion.Angle(transform.localRotation, READY_ROT) >= TOLERANCE)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, READY_POS, LERP_SPEED);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, READY_ROT, LERP_SPEED);
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

            _normalAttackWait = WaitAttackRoutine();
            StartCoroutine(_normalAttackWait);
        }

        private IEnumerator WaitAttackRoutine()
        {
            Vector3 endPosition = new Vector3(READY_POS.x, 0.0f, READY_POS.z + _myData.attackRange);
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

            while ((READY_POS - transform.localPosition).magnitude >= TOLERANCE)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, READY_POS, LERP_SPEED);
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

            if (!ctx.started || _normalAttackWait != null || PlayerManager.Instance.CurrentState == EPlayerState.Defense)
            {
                return;
            }

            ActivateNormalAttack();
            PlayerManager.Instance.ChangePlayerState(EPlayerState.Attacking);
        }

        #endregion

        #region Defense & CounterAttack

        private void ActivateDefense()
        {
            if (_defenseWait != null)
            {
                return;
            }

            _defenseWait = DefenseWaitRoutine();
            StartCoroutine(_defenseWait);
        }

        private IEnumerator DefenseWaitRoutine()
        {
            while ((transform.localPosition - DEFENSE_POS).magnitude >= TOLERANCE
                   || Quaternion.Angle(transform.localRotation, DEFENSE_ROT) >= TOLERANCE)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, DEFENSE_POS, LERP_SPEED);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, DEFENSE_ROT, LERP_SPEED);
                yield return null;
            }
        }

        private void ActivateCounterAttack()
        {
            
        }

        public void OnDefenseButtonClick(InputAction.CallbackContext ctx)
        {
            if (PlayerManager.Instance.CurrentState == EPlayerState.Attacking)
            {
                return;
            }
            
            if (ctx.canceled)
            {
                if (_defenseWait != null)
                {
                    StopCoroutine(_defenseWait);
                }

                _defenseWait = null;
                ReadyToAttack();
                return;
            }

            PlayerManager.Instance.ChangePlayerState(EPlayerState.Defense);
            ActivateDefense();
        }

        #endregion

        #region ScissorJump

        private void ActivateScissorJump()
        {
            // 가위를 타고 넘어가는 이동
        }

        public void OnScissorJumpButtonClick(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
            {
                return;
            }

            ActivateScissorJump();
        }

        #endregion
        
        #region Rope

        public void OnRopeButtonClick(InputAction.CallbackContext ctx)
        {
            if (PlayerMove.Instance.IsRopeAction || !ctx.started)
            {
                return;
            }

            if (!CanHangRope)
            {
                return;
            }
            
            RotatePlayer();
            PlayerMove.Instance.ApplyRopeAction(_hitPosition, _ropeRange);
            StartCoroutine(HangRopeRoutine());
        }

        private IEnumerator HangRopeRoutine()
        {
            const float DRAW_TOLERANCE = 0.2f;
            while ((transform.position - _hitPosition).magnitude >= DRAW_TOLERANCE)
            {
                LineDraw.Instance.Draw(transform.position, _hitPosition);
                yield return null;
            }
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
    }
}