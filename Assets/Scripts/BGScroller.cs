using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   //직렬화
public class BGScrollData
{
    public Renderer RenderForScroll;    //Material임
    public float Speed;
    public float OffsetX;
}

public class BGScroller : MonoBehaviour
{
    [SerializeField]
    BGScrollData[] ScrollDatas;

    private void FixedUpdate()
    {
        UpdateScroll();
    }

    void UpdateScroll()
    {
        for (int i = 0; i < ScrollDatas.Length; i++)
        {
            SetTextureOffset(ScrollDatas[i]);
        }
    }
    
    void SetTextureOffset(BGScrollData scrollData)
    {
        scrollData.OffsetX += (float)(scrollData.Speed) * Time.fixedDeltaTime;
        
        if (scrollData.OffsetX > 1) //혹시 모를 오류 방지
            scrollData.OffsetX = scrollData.OffsetX % 1.0f;


        Vector2 Offset = new Vector2(scrollData.OffsetX, 0);
        scrollData.RenderForScroll.material.SetTextureOffset("_MainTex", Offset);   //_MainTex 는 프로퍼티 이름 
    }
}
