using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConfigPanel : BasePanel
{
    const string DefaultIPAddress = "localhost";
    const string DefaultPort = "7777";

    [SerializeField]
    InputField IPAddressInputField;

    [SerializeField]
    InputField PortInputField;

    public override void InitializePanel()
    {
        base.InitializePanel();

        //IP와 Port번호를 기본값으로 설정
        IPAddressInputField.text = DefaultIPAddress;
        PortInputField.text = DefaultPort;
        Close(); //일단 꺼놓기
    }

    public void OnHostButton()  //호스트 접속
    {
        SystemManager.Instance.ConnectionInfo.Host = true;
        TitleSceneMain sceneMain = SystemManager.Instance.GetCurrentSceneMain<TitleSceneMain>();
        sceneMain.GotoNextScene(); //다음 씬으로 넘어가겠다.
    }

    public void OnClientButton()    //클라이언트로 접속
    {
        SystemManager.Instance.ConnectionInfo.Host = false;
        TitleSceneMain sceneMain = SystemManager.Instance.GetCurrentSceneMain<TitleSceneMain>();

        //IP입력 값
        if (!string.IsNullOrEmpty(IPAddressInputField.text) || IPAddressInputField.text != DefaultIPAddress)
            SystemManager.Instance.ConnectionInfo.IPAddress = IPAddressInputField.text;

        //Port번호 입력 값
        if(!string.IsNullOrEmpty(PortInputField.text) || PortInputField.text != DefaultPort)
        {
            int port = 0;
            if (int.TryParse(PortInputField.text, out port))    //int 형으로 변환
                SystemManager.Instance.ConnectionInfo.port = port;
            else
            {
                Debug.LogError("OnClientButton error port = " + PortInputField.text);
                return;
            }
        }
        sceneMain.GotoNextScene();
    }
}
