using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;   //HLAPI�� ��ġ �Ǿ��־�� �����Ѵ�.

public class FWNetworkManager : NetworkManager
{
    public const int WaitingPlayerCount = 2; //2���� ��ٸ��ڴ�.

    int PlayerCount = 0;

    public bool isServer
    {
        get;
        private set;
    }

    #region SERVER SID EVENT
    public override void OnServerConnect(NetworkConnection conn)    //���������
    {
        Debug.Log("OnSeverConnect call : " + conn.address + "," + conn.connectionId);
        base.OnServerConnect(conn);
    }

    public override void OnServerSceneChanged(string sceneName)     //�������� ���� �ٲ����
    {
        Debug.Log("OnServerSceneChanged : " + sceneName);
        base.OnServerSceneChanged(sceneName);
    }
    public override void OnServerReady(NetworkConnection conn)  //������ �غ�Ǿ�����
    {
        Debug.Log("OnServerReady : " + conn.address + "," + conn.connectionId);
        base.OnServerReady(conn);

        PlayerCount++;

        if(PlayerCount >= WaitingPlayerCount)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            inGameSceneMain.GameStart();
        }
    }
    public override void OnServerError(NetworkConnection conn, int errorCode)   //������ ������ �߻�������
    {
        Debug.Log("OnServerError : ErrorCode = " + errorCode);
        base.OnServerError(conn, errorCode);
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect : " + conn.address);
        base.OnServerDisconnect(conn);
    }

    public override void OnStartServer()    //ȣ��Ʈ�� �����Ҷ��� ����
    {
        Debug.Log("OnStartSever");
        base.OnStartServer();
        isServer = true;
    }
    #endregion

    #region CLIENT SIDE EVENT
    public override void OnStartClient(NetworkClient client)
    {
        Debug.Log("OnStartClient : " + client.serverIp);
        base.OnStartClient(client);
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("OnClientConnect : connectionID " + conn.connectionId + ", hostID = " + conn.hostId);
        base.OnClientConnect(conn);
    }
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        Debug.Log("OnClientSceneChanged : " + conn.hostId);
        base.OnClientSceneChanged(conn);
    }
    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("OnClientError : " + errorCode);
        base.OnClientError(conn, errorCode);
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnClientDisconect : " + conn.hostId);
        base.OnClientDisconnect(conn);
    }
    public override void OnClientNotReady(NetworkConnection conn)
    {
        Debug.Log("OnClientDisconect : " + conn.hostId);
        base.OnClientNotReady(conn);
    }
    public override void OnDropConnection(bool success, string extendedInfo)    //������ ���� �Ǿ�����
    {
        Debug.Log("OnDropConnection : " + extendedInfo);
        base.OnDropConnection(success, extendedInfo);
    }
    #endregion
}