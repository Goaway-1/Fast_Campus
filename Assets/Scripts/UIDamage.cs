using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDamage : MonoBehaviour
{
    enum DamageState : int
    {
        None = 0,
        SizeUp,
        Display,
        FadeOut
    }

    [SerializeField]
    DamageState damageState = DamageState.None;

    //각 시간 진행기간
    const float SizeUpDuration = 0.1f;
    const float DisplayDuration = 0.5f;
    const float FadeOutDuration = 0.2f;

    [SerializeField]
    Text damageText;    

    Vector3 CurrentVelocity;    //SmoothDamp 사용시 사용

    //시작 시간 저장용
    float DisplayStartTime;
    float FadeOutStartTime;
    
    public string FilePath
    {
        get; set;
    }

    private void Update()
    {
        UpdateDamage();
    }

    public void ShowDamage(int damage, Color textColor)
    {
        damageText.color = textColor;
        damageText.text = damage.ToString();
        Reset();
        damageState = DamageState.SizeUp;   //보여지기 시작
    }

    void Reset()
    {
        transform.localScale = Vector3.zero;    //사이즈가 0
        Color newColor = damageText.color;
        newColor.a = 1.0f;                      //투명도 1
        damageText.color = newColor;
    }

    void UpdateDamage()
    {
        if (damageState == DamageState.None)
            return;

        switch (damageState)
        {
            case DamageState.SizeUp:
                transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one, ref CurrentVelocity, SizeUpDuration);

                if(transform.localScale == Vector3.one)
                {
                    damageState = DamageState.Display;
                    DisplayStartTime = Time.time;
                }
                break;
            case DamageState.Display:
                if(Time.time - DisplayStartTime > DisplayDuration)
                {
                    damageState = DamageState.FadeOut;
                    FadeOutStartTime = Time.time;
                }
                break;
            case DamageState.FadeOut:
                Color newColor = damageText.color;
                newColor.a = Mathf.Lerp(1, 0, (Time.time - FadeOutStartTime) / FadeOutDuration);    //1~0으로 투명값 조절
                damageText.color = newColor;

                if (newColor.a == 0)
                {
                    damageState = DamageState.None;
                    SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Remove(this);
                }
                break;
        }
    }
}
