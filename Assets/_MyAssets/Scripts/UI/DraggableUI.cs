using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IDragHandler
{
    [SerializeField] private Canvas _canvas;
    private RectTransform _rectTransform;

    protected virtual void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
}
