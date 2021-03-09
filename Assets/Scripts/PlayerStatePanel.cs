using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStatePanel : BasePanel
{
    [SerializeField]
    Text scoreValue;

    [SerializeField]
    Gage HPGage;

    public void SetScore(int value)     //점수 설정
    {
        Debug.Log("SetScore value = " + value);

        scoreValue.text = value.ToString();
    }

    public void SetHP(float currentValue, float maxValue)   //HP설정
    {
        HPGage.SetHP(currentValue, maxValue);
    }
}
