using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideableObject : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private SceneManagerBase _sceneManager;
    
    private void Awake()
    {
        _meshRenderer = transform.GetComponent<MeshRenderer>();
        _sceneManager = GameObject.Find("@SceneManager").GetComponent<SceneManagerBase>();
    }

    private void Start()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        _meshRenderer.enabled = _sceneManager.IsDebugMode;
    }
}
