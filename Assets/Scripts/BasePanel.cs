using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    private void Awake()
    {
        InitializePanel();
    }

    private void OnDestroy()    //�ı��ɶ�
    {
        DestroyPanel();
    }

    private void OnGUI()    //�������� ȣ��
    {
        if (GUILayout.Button("What"))  //��ư������ ���ÿ� ��ư�� �����ٸ�
        {
            Close();
        }
    }
    public virtual void InitializePanel()   //�ʱ� ����
    {
        PanelManager.RegistPanel(GetType(), this);
    }
    public virtual void DestroyPanel()  //����
    {
        PanelManager.UnregistPanel(GetType());  //GetType ���� �ڽ��� ���´�.
    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
