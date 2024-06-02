using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[Serializable]
public class TabButtonTextInfo
{
    public TMP_Text text;
    public bool isSelected;
    public bool isHighlighted;
}

public class TabTextOffsetUpdater : MonoBehaviour
{
    [SerializeField] private List<TabButtonTextInfo> _textInfos;
    private float _defaultOffset;

    private void Start()
    {
        _defaultOffset = _textInfos[0].text.transform.localPosition.y;
    }

    private void OnEnable()
    {
        GameObject wasSelectedObject = null;
        foreach (TabButtonTextInfo textInfo in _textInfos)
        {
            if (textInfo.isSelected)
            {
                wasSelectedObject = textInfo.text.transform.parent.gameObject;
            }
        }

        if (wasSelectedObject == null)
        {
            return;
        }
        
        EventSystem.current.SetSelectedGameObject(wasSelectedObject);
    }

    private void Update()
    {
        foreach(TabButtonTextInfo textInfo in _textInfos)
        {
            if (textInfo.isSelected || textInfo.isHighlighted)
            {
                continue;
            }
            
            textInfo.text.transform.localPosition = new Vector3(textInfo.text.transform.localPosition.x, _defaultOffset,
                textInfo.text.transform.localPosition.z);
        }
    }

    public void UpdateTextOffsetToHighlighted(int index)
    {
        if (_textInfos[index].isSelected)
        {
            return;
        }
        
        _textInfos[index].text.transform.localPosition = new Vector3(_textInfos[index].text.transform.localPosition.x, _defaultOffset * 2,
            _textInfos[index].text.transform.localPosition.z);
        _textInfos[index].isHighlighted = true;
    }

    public void OnPointerExit(int index)
    {
        _textInfos[index].isHighlighted = false;
    }

    public void OnDeselected(int index)
    {
        _textInfos[index].isSelected = false;
        OnPointerExit(index);
    }
    
    public void UpdateTextOffsetToSelected(int index)
    {
        if(_textInfos[index].isSelected)
        {
            return;
        }
        
        _textInfos[index].text.transform.localPosition = new Vector3(_textInfos[index].text.transform.localPosition.x, _defaultOffset * 2,
            _textInfos[index].text.transform.localPosition.z);

        for(int i=0;i<_textInfos.Count;i++)
        {
            if (i == index)
            {
                _textInfos[i].isSelected = true;
                continue;
            }

            _textInfos[i].isSelected = false;
        }
    }
}
