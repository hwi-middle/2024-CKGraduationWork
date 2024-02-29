using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DW_InputVisualizer : DebugWindowBase
{
    private enum EKeyImages
    {
        W,
        A,
        S,
        D,
        Space,
    }

    [SerializeField] private List<Image> _keyImages;
    
    private void Update()
    {
        _keyImages[(int)EKeyImages.W].color = Input.GetKey(KeyCode.W) ? Color.red : Color.white;
        _keyImages[(int)EKeyImages.A].color = Input.GetKey(KeyCode.A) ? Color.red : Color.white;
        _keyImages[(int)EKeyImages.S].color = Input.GetKey(KeyCode.S) ? Color.red : Color.white;
        _keyImages[(int)EKeyImages.D].color = Input.GetKey(KeyCode.D) ? Color.red : Color.white;
        _keyImages[(int)EKeyImages.Space].color = Input.GetKey(KeyCode.Space) ? Color.red : Color.white;
    }
}
