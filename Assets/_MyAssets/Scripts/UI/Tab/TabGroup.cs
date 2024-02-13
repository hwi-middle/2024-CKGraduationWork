using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    private List<TabHeader> _tabHeaders;
    [SerializeField] private TabHeader _defaultHeader;
    private TabHeader _selectedHeader;

    private void Start()
    {
        if (_defaultHeader != null)
        {
            _selectedHeader = _defaultHeader;
            OnHeaderClick(_selectedHeader);
            _selectedHeader.MarkAsSelected();
        }
    }

    public void Register(TabHeader header)
    {
        _tabHeaders ??= new List<TabHeader>();
        _tabHeaders.Add(header);
    }

    public void OnHeaderClick(TabHeader header)
    {
        ResetTabs();
        header.TabPage.OnHeaderClick();
    }

    private void ResetTabs()
    {
        foreach (TabHeader header in _tabHeaders)
        {
            header.OnReset();
            header.TabPage.OnReset();
        }
    }
}
