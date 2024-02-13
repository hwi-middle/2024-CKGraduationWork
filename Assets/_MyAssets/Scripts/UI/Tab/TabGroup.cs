using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    private List<TabHeader> tabHeaders;
    [SerializeField] private TabHeader defaultHeader;
    private TabHeader _selectedHeader;

    private void Start()
    {
        if (defaultHeader != null)
        {
            _selectedHeader = defaultHeader;
            OnHeaderClick(_selectedHeader);
            _selectedHeader.MarkAsSelected();
        }
    }

    public void Register(TabHeader header)
    {
        tabHeaders ??= new List<TabHeader>();
        tabHeaders.Add(header);
    }

    public void OnHeaderClick(TabHeader header)
    {
        ResetTabs();
        header.TabPage.OnHeaderClick();
        // header.TabPage.gameObject.SetActive(true);
    }

    private void ResetTabs()
    {
        foreach (TabHeader header in tabHeaders)
        {
            header.OnReset();
            header.TabPage.OnReset();
            // header.TabPage.gameObject.SetActive(false);
        }
    }
}
