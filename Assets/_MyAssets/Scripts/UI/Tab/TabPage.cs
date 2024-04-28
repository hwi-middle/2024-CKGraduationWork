using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabPage : MonoBehaviour
{
    [SerializeField] private GameObject _firstSelectedObject;
    public GameObject FirstSelected => _firstSelectedObject;
    
    public void OnHeaderClick()
    {
        gameObject.SetActive(true);
    }

    public void OnReset()
    {
        gameObject.SetActive(false);
    }
}
