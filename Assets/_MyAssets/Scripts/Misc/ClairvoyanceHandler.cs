using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ClairvoyanceHandler : MonoBehaviour
{
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private Material _grayscaleMaterial;
    [SerializeField] private Material _clairvoyanceMaterial;
    
    private Camera _mainCamera;
    
    private bool _isClairvoyance;
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");

    private void OnEnable()
    {
        _inputData.clairvoyanceEvent += HandleClairvoyanceAction;
    }
    
    private void OnDisable()
    {
        ToggleClairvoyance(false);
        _inputData.clairvoyanceEvent -= HandleClairvoyanceAction;
    }
    
    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        ToggleClairvoyance(false);
    }

    private void HandleClairvoyanceAction()
    {
        if (PopupHandler.Instance.IsTutorialPopup && PopupHandler.Instance.IsPopupActive)
        {
            return;
        }
        
        ToggleClairvoyance(!_isClairvoyance);
    }
    
    private void ToggleClairvoyance(bool active)
    {
        _isClairvoyance = active;
        AudioPlayManager.Instance.PlayOnceSfxAudio(active
            ? ESfxAudioClipIndex.Player_Looking_On
            : ESfxAudioClipIndex.Player_Looking_Off);
        _grayscaleMaterial.SetFloat(Alpha, _isClairvoyance ? 1 : 0);
        _clairvoyanceMaterial.SetFloat(Alpha, _isClairvoyance ? 1 : 0);
        _mainCamera.GetComponent<UniversalAdditionalCameraData>().cameraStack[0].enabled = _isClairvoyance;
    }
}
