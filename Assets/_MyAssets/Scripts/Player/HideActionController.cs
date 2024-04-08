using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideActionController : MonoBehaviour
{
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private PlayerData _data;

    private IEnumerator _hideActionRoutine;

    private Camera _mainCamera;

    private void OnEnable()
    {
        _inputData.hideEvent += OnHide;
    }

    private void OnDisable()
    {
        _inputData.hideEvent -= OnHide;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        
    }

    private void OnHide()
    {
        Transform cameraTransform = _mainCamera.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        LayerMask layerMask = LayerMask.GetMask("Hideable");
        
        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return; 
        }

        float hitDistance = Vector3.Distance(transform.position, hit.point);
        if (hitDistance > _data.maxDistanceHideableObject)
        {
            return;
        }

        if (_hideActionRoutine != null)
        {
            return;
        }

        StartCoroutine(_hideActionRoutine);
    }

    private IEnumerator HideActionRoutine(Vector3 targetPosition, Vector3 targetNormal)
    {
        float t = 0;
        const float DURATION = 2.0f;
        while (t < DURATION)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f); 
            yield return null;
            t += Time.deltaTime;
        }
    }
}
