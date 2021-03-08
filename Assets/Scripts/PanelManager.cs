using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    //패널들을 저장
    static Dictionary<Type, BasePanel> Panels = new Dictionary<Type, BasePanel>();

    //패널 등록
    public static bool RegistPanel(Type PanelClassType, BasePanel basePanel)
    {
        if (Panels.ContainsKey(PanelClassType)) //같은 타입의 창은 존재하지 않는다.
        {
            Debug.LogError("RegistPanel Error! Already exist Type! PanelClassType = " + PanelClassType.ToString());
            return false;
        }

        Debug.Log("RegistPanel is called! Type = " + PanelClassType.ToString() + ", basePanel" + basePanel.name);

        Panels.Add(PanelClassType, basePanel);
        return true;
    }

    public static bool UnregistPanel(Type PanelClassType)   //삭제
    {
        if (!Panels.ContainsKey(PanelClassType))
        {
            Debug.LogError("UnregistPanel Error! Can't Found Type! PanelClassType = " + PanelClassType.ToString());
            return false;
        }

        Panels.Remove(PanelClassType);  //Dictionary 내부 함수
        return true;
    }

    public static BasePanel GetPanel(Type PanelClassType)   //추출
    {
        if (!Panels.ContainsKey(PanelClassType))
        {
            Debug.LogError("GetPanel Error! Can't Found Type! PanelClassType = " + PanelClassType.ToString());
            return null;
        }

        return Panels[PanelClassType];  //타입의 이름으로 호출
    }
}