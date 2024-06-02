using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabHeader : MonoBehaviour
{
    private static readonly Color DefaultButtonColor = Color.white;
    private static readonly Color SelectedButtonColor = Color.black;
    private static readonly Color DefaultTextColor = new Color(0.1960784f, 0.1960784f, 0.1960784f);
    private static readonly Color SelectedTextColor = Color.white;
    
    [SerializeField] private TabPage _tabPage;
    private TabGroup _tabGroup;
    public TabPage TabPage => _tabPage;

    private Button _button;
    private Image _buttonBg;
    private TMP_Text _buttonText;

    private void Awake()
    {
        _tabGroup = transform.parent.GetComponent<TabGroup>();
        Debug.Assert(_tabGroup != null);
        _tabGroup.Register(this);
        
        _button = GetComponent<Button>();
        Debug.Assert(_button != null);
        _button.onClick.AddListener(OnClick);
        
        _buttonBg = _button.GetComponent<Image>();
        Debug.Assert(_buttonBg);
        
        _buttonText = _button.transform.GetChild(0).GetComponent<TMP_Text>();
        Debug.Assert(_buttonText);
    }

    private void OnClick()
    {
        _tabGroup.OnHeaderClick(this);
        MarkAsSelected();
    }

    public void OnReset()
    {
        MarkAsUnselected();
    }

    public void MarkAsSelected()
    {
        //_buttonBg.color = SelectedButtonColor;
        _buttonText.color = SelectedTextColor;
    }

    public void MarkAsUnselected()
    {
        _buttonBg.color = DefaultButtonColor;
        _buttonText.color = DefaultTextColor;
    }
}
