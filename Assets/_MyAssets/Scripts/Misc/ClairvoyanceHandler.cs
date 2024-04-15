using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClairvoyanceHandler : MonoBehaviour
{
    [SerializeField] private PlayerInputData _inputData;
    [SerializeField] private Material _grayscaleMaterial;
    
    private Camera _mainCamera;
    
    private bool _isClairvoyance;
    private static readonly int Alpha = Shader.PropertyToID("_Alpha");

    private void OnEnable()
    {
        _inputData.clairvoyanceEvent += HandleClairvoyanceAction;
    }
    
    private void OnDisable()
    {
        _inputData.clairvoyanceEvent -= HandleClairvoyanceAction;
    }
    
    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    
    private void HandleClairvoyanceAction()
    {
        _isClairvoyance = !_isClairvoyance;
        _grayscaleMaterial.SetFloat(Alpha, _isClairvoyance ? 1 : 0);
    }
}
