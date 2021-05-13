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

        //IP�� Port��ȣ�� �⺻������ ����
        IPAddressInputField.text = DefaultIPAddress;
        PortInputField.text = DefaultPort;
        Close(); //�ϴ� ������
    }

    public void OnHostButton()  //ȣ��Ʈ ����
    {
        SystemManager.Instance.ConnectionInfo.Host = true;
        TitleSceneMain sceneMain = SystemManager.Instance.GetCurrentSceneMain<TitleSceneMain>();
        sceneMain.GotoNextScene(); //���� ������ �Ѿ�ڴ�.
    }

    public void OnClientButton()    //Ŭ���̾�Ʈ�� ����
    {
        SystemManager.Instance.ConnectionInfo.Host = false;
        TitleSceneMain sceneMain = SystemManager.Instance.GetCurrentSceneMain<TitleSceneMain>();

        //IP�Է� ��
        if (!string.IsNullOrEmpty(IPAddressInputField.text) || IPAddressInputField.text != DefaultIPAddress)
            SystemManager.Instance.ConnectionInfo.IPAddress = IPAddressInputField.text;

        //Port��ȣ �Է� ��
        if(!string.IsNullOrEmpty(PortInputField.text) || PortInputField.text != DefaultPort)
        {
            int port = 0;
            if (int.TryParse(PortInputField.text, out port))    //int ������ ��ȯ
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
