using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabPage : MonoBehaviour
{
    public void OnHeaderClick()
    {
        gameObject.SetActive(true);
    }

    public void OnReset()
    {
        gameObject.SetActive(false);
    }
}
