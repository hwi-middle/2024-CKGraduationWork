using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoubleSideSlicedFilledImage : MonoBehaviour
{
    private SlicedFilledImage _image1;
    private SlicedFilledImage _image2;
    
    public float FillAmount
    {
        set
        {
            if (_image1 == null || _image2 == null)
            {
                Init();
            }
            
            _image1.fillAmount = value;
            _image2.fillAmount = value;
        }
    }

    private void Init()
    {
        _image1 = transform.GetChild(0).GetComponent<SlicedFilledImage>();
        _image2 = transform.GetChild(1).GetComponent<SlicedFilledImage>();
        Debug.Assert(_image1 != null);
        Debug.Assert(_image2 != null);
    }
}
