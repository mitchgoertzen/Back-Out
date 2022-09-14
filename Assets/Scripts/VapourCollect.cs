using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VapourCollect : MonoBehaviour
{
    [SerializeField] private Color sliderColour;

    [SerializeField] private Slider vapourBar;

    private float vapourAmount;

    void Start()
    {
        vapourBar = GetComponent<Slider>();
        vapourBar.maxValue = 1000;
        vapourBar.value = 0;
        vapourBar.gameObject.transform.Find("Foreground").GetComponent<Image>().color = sliderColour;
    }

    public void SetLevel(float amount)
    {
        vapourAmount = amount;

        if (vapourAmount > vapourBar.maxValue)
        {
            vapourBar.value = vapourBar.maxValue;
        }
        else
        {
            vapourBar.value = vapourAmount;
        }
    }
}
