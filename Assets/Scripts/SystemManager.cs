using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    static SystemManager instance;

    // 싱글톤
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

    public Player Hero  //접근 프로퍼티
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

    GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();     //점수관리

    public GamePointAccumulator GamePointAccumulator
    {
        get { return gamePointAccumulator; }
    }

    //캐싱관련 3가지
    PrefabCacheSystem enemyCacheSystem = new PrefabCacheSystem();   //적
    PrefabCacheSystem bulletCacheSystem = new PrefabCacheSystem();  //총알
    PrefabCacheSystem effectCacheSystem = new PrefabCacheSystem();  //효과

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
    //캐싱관련 3가지 종료

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
