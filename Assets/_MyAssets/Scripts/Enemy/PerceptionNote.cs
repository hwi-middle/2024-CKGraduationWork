using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PerceptionNote : MonoBehaviour
{
    public EnemyBase owner;
    private Camera _mainCamera;
    private readonly Color _startColor = Color.yellow;
    private readonly Color _endColor = Color.red;
    private Image _bgImage;
    private Image _fillImage;
    private Sprite _questionMarkSprite;
    private Sprite _exclamationMarkSprite;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _bgImage = GetComponent<Image>();
        _fillImage = transform.GetChild(0).GetComponent<Image>();
        _questionMarkSprite = Resources.Load<Sprite>("PerceptionNote/help-sign");
        _exclamationMarkSprite = Resources.Load<Sprite>("PerceptionNote/warning-sign");
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePerceptionGaugeFill();
        UpdatePerceptionGaugePosition();
    }
    
    private void UpdatePerceptionGaugeFill()
    {
        if (Mathf.Approximately( owner.PerceptionGauge, 0f))
        {
            _bgImage.enabled = false;
            _fillImage.enabled = false;
            return; 
        }

        if (Mathf.Approximately(owner.PerceptionGauge, 100f))
        {
            _bgImage.enabled = false;
            _fillImage.sprite = _exclamationMarkSprite;
            _fillImage.fillAmount = 1f;
            _fillImage.color = _endColor;
            return;
        }
        
        _bgImage.enabled = true;
        _fillImage.enabled = true;
        _fillImage.sprite = _questionMarkSprite;
        float fillAmount = owner.PerceptionGauge / 100f;
        _fillImage.fillAmount = fillAmount;
        _fillImage.color = Color.Lerp(_startColor, _endColor, fillAmount);
    }

    private void UpdatePerceptionGaugePosition()
    {
        _bgImage.transform.position = _mainCamera.WorldToScreenPoint(owner.transform.position + Vector3.up * 2f);
    }
}
