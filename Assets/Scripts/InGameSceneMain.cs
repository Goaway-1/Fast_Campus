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

    public Player Hero  //���� ������Ƽ
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

    GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();     //��������

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

    //ĳ�̰��� 4����
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
    //ĳ�̰��� 4���� ����

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