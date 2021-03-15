using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSceneMain : BaseSceneMain
{
    const float GameReadyIntavel = 3.0f;

    public enum GameState : int
    {
        Ready = 0,
        Running,
        End
    }

    GameState currentGameState = GameState.Ready;
    public GameState CurrentGameState
    {
        get { return currentGameState; }
    }


    [SerializeField]
    Player player;

    public Player Hero  //접근 프로퍼티
    {
        get
        {
            if (!player)
            {
                Debug.LogError("Main Player is not Setted!");
            }
            return player;
        }
        set
        {
            player = value;
        }
    }

    GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();     //점수관리

    public GamePointAccumulator GamePointAccumulator
    {
        get { return gamePointAccumulator; }
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

    //캐싱관련 4가지
    PrefabCacheSystem enemyCacheSystem = new PrefabCacheSystem();   //적
    PrefabCacheSystem bulletCacheSystem = new PrefabCacheSystem();  //총알
    PrefabCacheSystem effectCacheSystem = new PrefabCacheSystem();  //효과
    PrefabCacheSystem damageCacheSystem = new PrefabCacheSystem();  //효과

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
    //캐싱관련 4가지 종료

    [SerializeField]
    SquadronManager squadronManager;
    public SquadronManager SquadronManager
    {
        get { return squadronManager; }
    }

    float SceneStartTime;

    [SerializeField]
    Transform mainBGQuadTransform;

    public Transform MainBGQuadTransform
    {
        get { return mainBGQuadTransform;  }
    }

    protected override void OnStart()
    {
        base.OnStart();
        SceneStartTime = Time.time;
    }
    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;

        if (currentTime - SceneStartTime > GameReadyIntavel && CurrentGameState == GameState.Ready)
        {
            squadronManager.StartGame();
            currentGameState = GameState.Running;
        }
    }
}