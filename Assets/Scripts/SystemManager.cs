using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    static SystemManager instance;

    // �̱���
    public static SystemManager Instance
    {
        get 
        {
            if(instance == null)
            {
                instance = FindObjectOfType<SystemManager>();
                if(instance == null)
                {
                    GameObject container = new GameObject("Manager");
                    instance = container.AddComponent<SystemManager>();
                }
            }
            return instance; 
        }
    }

    [SerializeField]
    EnemyTable enemyTable;

    public EnemyTable EnemyTable
    {
        get { return enemyTable; }
    }

    BaseSceneMain currentSceneMain;
    public BaseSceneMain CurrentSceneMain
    {
        set
        {
            currentSceneMain = value;
        }
    }

    [SerializeField]
    NetworkConnectionInfo connectionInfo = new NetworkConnectionInfo();

    public NetworkConnectionInfo ConnectionInfo
    {
        get { return connectionInfo; }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        BaseSceneMain baseSceneMain = GameObject.FindObjectOfType<BaseSceneMain>();
        Debug.Log("OnSceneLoaded! baseSceneMain.name = " + baseSceneMain.name);
        SystemManager.Instance.currentSceneMain = baseSceneMain;
    }

    public T GetCurrentSceneMain<T>() where T : BaseSceneMain   //BaseSceneMain�̰ų� ��ӹ��� �� �޴´�.
    {
        return currentSceneMain as T;   //��ȯ�� ����
    }
}
