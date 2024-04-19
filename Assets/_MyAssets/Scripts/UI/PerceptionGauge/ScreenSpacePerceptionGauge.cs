using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpacePerceptionGauge : MonoBehaviour
{
    private DoubleSideSlicedFilledImage _phase1PerceptionGaugeImage;
    private DoubleSideSlicedFilledImage _phase2PerceptionGaugeImage;
    private GameObject _phase3Image;

    private void Awake()
    {
        _phase1PerceptionGaugeImage = transform.GetChild(0).GetComponent<DoubleSideSlicedFilledImage>();
        _phase2PerceptionGaugeImage = transform.GetChild(1).GetComponent<DoubleSideSlicedFilledImage>();
        _phase3Image = transform.GetChild(2).gameObject;
    }
    
    public void SetPerceptionGauge(float phase1Value, float phase2Value)
    {
        _phase1PerceptionGaugeImage.FillAmount = phase1Value;
        _phase2PerceptionGaugeImage.FillAmount = phase2Value;
        _phase3Image.SetActive(phase2Value >= 1.0f);
    }
}
