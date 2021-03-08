using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gage : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    public void SetHP(float currentValue, float MaxValue)
    {
        if (currentValue > MaxValue)
            currentValue = MaxValue;

        slider.value = currentValue / MaxValue;
    }
}
