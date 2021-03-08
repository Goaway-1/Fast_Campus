using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    //�гε��� ����
    static Dictionary<Type, BasePanel> Panels = new Dictionary<Type, BasePanel>();

    //�г� ���
    public static bool RegistPanel(Type PanelClassType, BasePanel basePanel)
    {
        if (Panels.ContainsKey(PanelClassType)) //���� Ÿ���� â�� �������� �ʴ´�.
        {
            Debug.LogError("RegistPanel Error! Already exist Type! PanelClassType = " + PanelClassType.ToString());
            return false;
        }

        Debug.Log("RegistPanel is called! Type = " + PanelClassType.ToString() + ", basePanel" + basePanel.name);

        Panels.Add(PanelClassType, basePanel);
        return true;
    }

    public static bool UnregistPanel(Type PanelClassType)   //����
    {
        if (!Panels.ContainsKey(PanelClassType))
        {
            Debug.LogError("UnregistPanel Error! Can't Found Type! PanelClassType = " + PanelClassType.ToString());
            return false;
        }

        Panels.Remove(PanelClassType);  //Dictionary ���� �Լ�
        return true;
    }

    public static BasePanel GetPanel(Type PanelClassType)   //����
    {
        if (!Panels.ContainsKey(PanelClassType))
        {
            Debug.LogError("GetPanel Error! Can't Found Type! PanelClassType = " + PanelClassType.ToString());
            return null;
        }

        return Panels[PanelClassType];  //Ÿ���� �̸����� ȣ��
    }
}