using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;   //HLAPI가 설치 되어있어야 지원한다.

public class FWNetworkManager : NetworkManager
{
    public const int WaitingPlayerCount = 2; //2명을 기다리겠다.

    int PlayerCount = 0;

    public bool isServer
    {
        get;
        private set;
    }

    #region SERVER SID EVENT
    public override void OnServerConnect(NetworkConnection conn)    //서버연결시
    {
        Debug.Log("OnSeverConnect call : " + conn.address + "," + conn.connectionId);
        base.OnServerConnect(conn);
    }

    public override void OnServerSceneChanged(string sceneName)     //서버에서 씬이 바뀌였을때
    {
        Debug.Log("OnServerSceneChanged : " + sceneName);
        base.OnServerSceneChanged(sceneName);
    }
    public override void OnServerReady(NetworkConnection conn)  //서버가 준비되었을때
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
    public override void OnServerError(NetworkConnection conn, int errorCode)   //서버가 오류를 발생했을때
    {
        Debug.Log("OnServerError : ErrorCode = " + errorCode);
        base.OnServerError(conn, errorCode);
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnServerDisconnect : " + conn.address);
        base.OnServerDisconnect(conn);
    }

    public override void OnStartServer()    //호스트로 접속할때만 들어옴
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
    public override void OnDropConnection(bool success, string extendedInfo)    //강제로 종료 되었을때
    {
        Debug.Log("OnDropConnection : " + extendedInfo);
        base.OnDropConnection(success, extendedInfo);
    }
    #endregion
}
