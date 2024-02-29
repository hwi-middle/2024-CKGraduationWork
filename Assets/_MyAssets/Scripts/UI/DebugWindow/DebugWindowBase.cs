using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugWindowBase : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Slider _opacitySlider;
    [SerializeField] private Button _closeButton;
    
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    protected virtual void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        Debug.Assert(_canvasGroup != null);
        _opacitySlider.onValueChanged.AddListener(OnOpacitySliderValueChanged);
        _closeButton.onClick.AddListener(OnClickCloseButton);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }
    
    private void OnOpacitySliderValueChanged(float value)
    {
        _canvasGroup.alpha = value;
    }

    private void OnClickCloseButton()
    {
        gameObject.SetActive(false);
    }
}
