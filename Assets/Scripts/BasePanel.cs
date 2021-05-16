using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    private void Awake()
    {
        InitializePanel();
    }

    private void Update()
    {
        UpdatePanel();
    }

    private void OnDestroy()    //파괴될때
    {
        DestroyPanel();
    }

    private void OnGUI()    //매프레임 호출
    {
        //if (GUILayout.Button("Canel"))  //버튼생성과 동시에 버튼을 누른다면
        //{
        //    Close();
        //}
    }
    public virtual void InitializePanel()   //초기 생성
    {
        PanelManager.RegistPanel(GetType(), this);
    }
    public virtual void DestroyPanel()  //삭제
    {
        PanelManager.UnregistPanel(GetType());  //GetType 쓰면 자식이 나온다.
    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void UpdatePanel()
    {

    }
}
