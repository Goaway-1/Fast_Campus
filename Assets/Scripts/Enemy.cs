using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Enemy : Actor
{
    public enum State : int //int 상속
    { 
        None = -1,  //사용전
        Ready = 0,  //준비완료
        Appear,     //등장
        Battle,     //전투중
        Dead,       //사망
        Disapper,   //퇴장
    }

    [SerializeField]
    [SyncVar]
    State CurrentState = State.None;        //현재 상태

    [SerializeField]
    [SyncVar]
    Vector3 TargetPostion;  //현재 목표로인 위치


    //속도 관련
    const float MaxSpeed = 10.0f;       //변하지 않는 속도 값(가속을 이용할껀디)
    const float MaxSpeedTime = 0.5f;    //가속 시간
    
    [SerializeField]
    [SyncVar]
    float CurrentSpeed;     //현재 속도

    [SyncVar]
    Vector3 CurrentVelocity;

    [SyncVar]
    float MoveStartTime = 0.0f; //움직이기 시작한 시간 --> 속도를 점점 증가 시키기 위함

    //총알과 관련
    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    [SyncVar]
    float BulletSpeed = 1f;

    [SyncVar]
    float LastActionUpdateTime = 0.0f;  //언제 마지막으로 싸웠노

    [SerializeField]
    [SyncVar]
    int FireRemainCount = 3;    //총알의 한계

    [SerializeField]
    [SyncVar]
    int GamePoint = 10; //게임 점수

    [SyncVar]   //캐시 관련
    [SerializeField]
    string filePath;
    public string FilePath
    {
        get { return filePath; }
        set { filePath = value; }
    }

    Vector3 AppearPoint;        //입장시 도착 위치
    Vector3 DisappearPoint;     //퇴장시 목표 위치

    protected override void Initialize()    //호스트로 부터 Enemy를 만들라고 수신
    {
        base.Initialize();
        Debug.Log("Enemy : Initialize");

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)   //클라이언트 접속시 강제로 등록
        {
            transform.SetParent(inGameSceneMain.EnemyManager.transform);
            inGameSceneMain.EnemyCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(false);
        }

        if (actorInstanceID != 0)
            inGameSceneMain.ActorManager.Regist(actorInstanceID, this);
    }

    protected override void UpdateActor()
    {
        switch(CurrentState)    //현재 상태에 따른 행동들
        {
            case State.None:
                break;
            case State.Ready:
                UpdateReady();  //
                break;

            case State.Dead:
                break;

            case State.Appear:
            case State.Disapper:
                UpdateSpeed();
                UpdateMove();
                break;

            case State.Battle:
                UpdateBattle();
                break;

            default:
                break;
        }
    }

    void UpdateSpeed()  //속도의 갱신
    {
        CurrentSpeed = Mathf.Lerp(CurrentSpeed,MaxSpeed, (Time.time - MoveStartTime)/MaxSpeedTime);    //두 값 사이의 어딘가를 반환
    }

    void UpdateMove()   //이동
    {
        float distance = Vector3.Distance(TargetPostion, transform.position);   //타겟까지의 거리
        if (distance == 0)  //도착 판단
        {
            Arrived();
            return;
        }
        CurrentVelocity = (TargetPostion - transform.position).normalized * CurrentSpeed; //초당 방향 벡터(크키 1)

        //실질적으로 이동하는 부분 --> 속도 = 거리/시간
        transform.position = Vector3.SmoothDamp(transform.position, TargetPostion, ref CurrentVelocity, distance/CurrentSpeed, MaxSpeed);   //자연스럽게 이동
    }
    void Arrived()      //도착 알림
    {
        CurrentSpeed = 0;
        if (CurrentState == State.Appear)
        {
            CurrentState = State.Battle;
            LastActionUpdateTime = Time.time;    //배틀 시작 시간
        }
        else //if (CurrentState == State.Disapper)
        {
            CurrentState = State.None;
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);
        }
    }

    public void Reset(SquadronMemberStruct data)   //초기화 담당
    {
        if (isServer)
            RpcReset(data);
        else
        {
            CmdReset(data);
            if (isLocalPlayer)
                ResetData(data);
        }
    }

    void ResetData(SquadronMemberStruct data)
    {
        EnemyStruct enemyStruct = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyID);

        CurrentHP = MaxHP = enemyStruct.MaxHP;
        Damage = enemyStruct.Damage;
        crashDamage = enemyStruct.CrashDamage;
        BulletSpeed = enemyStruct.BulletSpeed;
        FireRemainCount = enemyStruct.FireRemainCount;
        GamePoint = enemyStruct.GamePoint;

        //Squadron 관련
        AppearPoint = new Vector3(data.AppearPointX, data.AppearPointY, 0);
        DisappearPoint = new Vector3(data.DisappearPointX, data.DisappearPointY, 0);

        CurrentState = State.Ready;
        LastActionUpdateTime = Time.time;

        isDead = false;     //Enemy는 재사용되기 때문에 초기화시켜주어야 한다.
    }

    public void Appear(Vector3 targetPos)   //등장
    {
        TargetPostion = targetPos;
        CurrentSpeed = MaxSpeed;

        CurrentState = State.Appear;
        MoveStartTime = Time.time;
    }
    void Disapper(Vector3 targetPos)    //소멸
    {
        TargetPostion = targetPos;
        CurrentSpeed = 0;

        CurrentState = State.Disapper;
    }
    void UpdateReady()
    {
        if(Time.time - LastActionUpdateTime > 1.0f)
        {
            Appear(AppearPoint);
        }
    }
    void UpdateBattle() 
    {
        if(Time.time - LastActionUpdateTime > 1f)
        {
            if (FireRemainCount > 0)
            {
                Fire();
                FireRemainCount--;
            }
            else
            {
                Disapper(DisappearPoint);
            }
            LastActionUpdateTime = Time.time;
        }
    }
    private void OnTriggerEnter(Collider other) //상대방의 정보가 나온다.
    {
        Player player = other.GetComponentInParent<Player>(); //부딪친거는 박스 콜라이더니까 상위인 부모 호출
        if (player)
        {
            if (!player.IsDead)
            {
                BoxCollider box = ((BoxCollider)other);
                Vector3 crashPos = player.transform.position + box.center;
                crashPos.x += box.size.x * 0.5f;

                player.OnCrash( crashDamage, crashPos);
            }
        }
    }

    public void Fire()
    {
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.EnemyBulletIndex);
        bullet.Fire(actorInstanceID, FireTransform.position, -FireTransform.right, BulletSpeed, Damage);
    }

    protected override void OnDead()
    {
        base.OnDead();

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.Accumulate(GamePoint);
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);

        CurrentState = State.Dead;
    }

    protected override void DecreaseHP(int value, Vector3 damagePos)
    {
        base.DecreaseHP(value, damagePos);

        Vector3 damagePoint = damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Generate(DamageManager.EnemyDamageIndex, damagePoint, value, Color.magenta);
    }

    [Command]
    public void CmdReset(SquadronMemberStruct data)
    {
        ResetData(data);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcReset(SquadronMemberStruct data)
    {
        ResetData(data);
        base.SetDirtyBit(1);
    }
}
