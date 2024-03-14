using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScissorManager : Singleton<ScissorManager>
{
    [SerializeField] private PlayerData _myData;
    
    private Camera _mainCamera;
    
    private IEnumerator _normalAttackWait;
    private IEnumerator _readyToAttackWait;
    private IEnumerator _guardWait;
    private IEnumerator _wireActionWait;
    
    private const float TOLERANCE = 0.1f;
    private const float LERP_SPEED = 1.0f;

    private static readonly Vector3 CAMERA_CENTER_RAY = new(0.5f, 0.5f, 0);

    private static readonly Vector3 IDLE_POS = new(0.0f, 0.0f, -1.0f);
    private static readonly Quaternion IDLE_ROT = Quaternion.identity;

    private static readonly Vector3 READY_POS = new(0.5f, 0.0f, 1.0f);
    private static readonly Quaternion READY_ROT = Quaternion.Euler(90.0f, 0.0f, 0.0f);

    private static readonly Vector3 DEFENSE_POS = new(0.0f, 0.0f, 1.0f);
    private static readonly Quaternion DEFENSE_ROT = Quaternion.Euler(0.0f, 0.0f, 45.0f);

    public float WireRange { get; set; }
    
    private Vector3 _hitPosition;
    public Vector3 HitPosition => _hitPosition;

    public bool CanHangWire
    {
        get
        {
            Debug.Assert(_mainCamera != null, "_mainCamera != null");
            Ray ray = _mainCamera.ViewportPointToRay(CAMERA_CENTER_RAY);

            bool isHit = Physics.Raycast(ray, out RaycastHit hit);
            Debug.DrawRay(ray.origin, hit.point, Color.blue);

            if (!isHit)
            {
                return false;
            }

            if (!hit.transform.CompareTag("WirePoint"))
            {
                return false;
            }
            
            WireRange = (hit.point - ray.origin).magnitude;

            if (WireRange >= _myData.maxWireRange)
            {
                return false;
            }
            
            _hitPosition = hit.transform.position;
            return true;
        }
    }

    private void Awake()
    {
        Debug.Assert(_myData != null, "_myData != null");
        _mainCamera = Camera.main;
    }

    #region About Attack

    public void ReturnToIdle()
    {
        StartCoroutine(ReturnToIdleRoutine());
    }

    private IEnumerator ReturnToIdleRoutine()
    {
        float moveTime = 0;
        while ((transform.localPosition - IDLE_POS).magnitude >= TOLERANCE
               || Quaternion.Angle(transform.localRotation, IDLE_ROT) >= TOLERANCE)
        {
            moveTime += Time.deltaTime;
            float t = LERP_SPEED * moveTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, IDLE_POS, t);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, IDLE_ROT, t);
            yield return new WaitForEndOfFrame();
        }
    }

    public void ActivateAttackReady()
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
        float moveTime = 0.0f;
        while ((transform.localPosition - READY_POS).magnitude >= TOLERANCE
               || Quaternion.Angle(transform.localRotation, READY_ROT) >= TOLERANCE)
        {
            moveTime += Time.deltaTime;
            float t = LERP_SPEED * moveTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, READY_POS, t);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, READY_ROT, t);
            yield return new WaitForEndOfFrame();
        }

        PlayerManager.Instance.ChangePlayerState(EPlayerState.Ready);
        _readyToAttackWait = null;
    }

    public void ActivateNormalAttack()
    {
        if (_normalAttackWait != null)
        {
            return;
        }
        
        // Aim 방향으로 공격
        RotatePlayer();
        WaitNormalAttack();
    }

    public void RotatePlayer()
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
        float moveTime = 0;

        while ((transform.localPosition - endPosition).magnitude >= TOLERANCE)
        {
            moveTime += Time.deltaTime;
            float t = LERP_SPEED * moveTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, endPosition, t);
            yield return new WaitForEndOfFrame();
        }

        const float COLLIDER_RANGE = 0.5f;
        const float COLLIDER_RADIUS = 0.5f;
        Vector3 checkPosition = new Vector3(transform.localPosition.x, transform.localPosition.y,
            transform.localPosition.z + COLLIDER_RANGE);
        checkPosition = transform.TransformPoint(checkPosition);

        Collider[] colliders = Physics.OverlapSphere(checkPosition, COLLIDER_RADIUS);
        // colliders 에 Enemy가 있으면 Attack Call 하는 느낌으로


        moveTime = 0;
        while ((READY_POS - transform.localPosition).magnitude >= TOLERANCE)
        {
            moveTime += Time.deltaTime;
            float t = LERP_SPEED * moveTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, READY_POS, t);
            yield return new WaitForEndOfFrame();
        }

        _normalAttackWait = null;
        PlayerManager.Instance.ChangePlayerState(EPlayerState.Ready);
    }

    #endregion

    #region Defense & Guard

    public void ActivateGuard()
    {
        if (_guardWait != null)
        {
            return;
        }

        _guardWait = GuardWaitRoutine();
        StartCoroutine(_guardWait);
    }

    public void StopGuardCoroutine()
    {
        if (_guardWait == null)
        {
            return;
        }
        
        StopCoroutine(_guardWait);
        _guardWait = null;
    }

    private IEnumerator GuardWaitRoutine()
    {
        float moveTime = 0;
        while ((transform.localPosition - DEFENSE_POS).magnitude >= TOLERANCE
               || Quaternion.Angle(transform.localRotation, DEFENSE_ROT) >= TOLERANCE)
        {
            moveTime += Time.deltaTime;
            float t = LERP_SPEED * moveTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, DEFENSE_POS, t);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, DEFENSE_ROT, t);
            yield return new WaitForEndOfFrame();
        }
    }

    private void ActivateParry()
    {

    }

    #endregion

    #region PositionMoving

    public void ActivatePositionMove()
    {
        // 가위를 타고 넘어가는 이동
    }

    #endregion

    public void ActivateHangWire()
    {
        StartCoroutine(HangWireRoutine());
    }

    private IEnumerator HangWireRoutine()
    {
        const float DRAW_TOLERANCE = 0.2f;
        while ((transform.position - _hitPosition).magnitude >= DRAW_TOLERANCE)
        {
            LineDraw.Instance.Draw(transform.position, _hitPosition);
            yield return new WaitForEndOfFrame();
        }
    }

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
