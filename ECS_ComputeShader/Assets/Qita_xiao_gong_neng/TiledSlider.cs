using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TiledSlider : RawImage
{
    private Slider m_Slider;
    protected override void Start()
    {
        m_Slider = GetComponentInParent<Slider>();
    }
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        float val = m_Slider.value;
        uvRect = new Rect(0, 0, val, 1);
        Debug.Log("V: " + uvRect.width);
    }
}