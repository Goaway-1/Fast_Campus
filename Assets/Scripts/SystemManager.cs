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
    Player player;

    public Player Hero  //���� ������Ƽ
    {
        get { return player; }
    }

    [SerializeField]
    EffectManager effectManager;

    public EffectManager EffectManager
    {
        get { return effectManager; }
    }

    [SerializeField]
    EnemyManager enemyManager;

    public EnemyManager EnemyManager
    {
        get { return enemyManager; }
    }

    [SerializeField]
    BulletManager bulletManager;

    public BulletManager BulletManager
    {
        get { return bulletManager; }
    }

    [SerializeField]
    DamageManager damageManager;

    public DamageManager DamageManager
    {
        get { return damageManager; }
    }

    GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();     //��������

    public GamePointAccumulator GamePointAccumulator
    {
        get { return gamePointAccumulator; }
    }

    //ĳ�̰��� 3����
    PrefabCacheSystem enemyCacheSystem = new PrefabCacheSystem();   //��
    PrefabCacheSystem bulletCacheSystem = new PrefabCacheSystem();  //�Ѿ�
    PrefabCacheSystem effectCacheSystem = new PrefabCacheSystem();  //ȿ��
    PrefabCacheSystem damageCacheSystem = new PrefabCacheSystem();  //ȿ��

    public PrefabCacheSystem EnemyCacheSystem
    {
        get { return enemyCacheSystem; }
    }
    public PrefabCacheSystem BulletCacheSystem
    {
        get { return bulletCacheSystem; }
    }
    public PrefabCacheSystem EffectCacheSystem
    {
        get { return effectCacheSystem; }
    }
    public PrefabCacheSystem DamageCacheSystem
    {
        get { return damageCacheSystem; }
    }
    //ĳ�̰��� 3���� ����

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
