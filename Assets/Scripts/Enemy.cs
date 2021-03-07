using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    State CurrentState = State.None;        //현재 상태

    [SerializeField]
    Vector3 TargetPostion;  //현재 목표로인 위치


    //속도 관련
    const float MaxSpeed = 10.0f;       //변하지 않는 속도 값(가속을 이용할껀디)
    const float MaxSpeedTime = 0.5f;    //가속 시간
    
    [SerializeField]
    float CurrentSpeed;     //현재 속도

    Vector3 CurrentVelocity;
    float MoveStartTime = 0.0f; //움직이기 시작한 시간 --> 속도를 점점 증가 시키기 위함

    //총알과 관련
    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    float BulletSpeed = 1f;

    float LastBattleUpdateTime = 0.0f;  //언제 마지막으로 싸웠노

    [SerializeField]
    int FireRemainCount = 3;    //총알의 한계

    int GamePoint = 10; //게임 점수

    //캐시 관련
    public string FilePath
    {
        get; set;
    }

    protected override void UpdateActor()
    {
        switch(CurrentState)    //현재 상태에 따른 행동들
        {
            case State.None:
            case State.Ready:
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
            LastBattleUpdateTime = Time.time;    //배틀 시작 시간
        }
        else //if (CurrentState == State.Disapper)
        {
            CurrentState = State.None;
        }
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
    void UpdateBattle() 
    {
        if(Time.time - LastBattleUpdateTime > 1f)
        {
            if (FireRemainCount > 0)
            {
                Fire();
                FireRemainCount--;
            }
            else
            {
                Disapper(new Vector3(-15, transform.position.y, transform.position.z));
            }
            LastBattleUpdateTime = Time.time;
        }
    }
    private void OnTriggerEnter(Collider other) //상대방의 정보가 나온다.
    {
        Player player = other.GetComponentInParent<Player>(); //부딪친거는 박스 콜라이더니까 상위인 부모 호출
        if (player)
        {
            player.OnCrash(this,crashDamage);   
        }
    }

    public override void OnCrash(Actor attacker, int damage)    //내가 부딪친거
    {
        base.OnCrash(attacker, damage);
    }

    public void Fire()
    {
        Bullet bullet = SystemManager.Instance.BulletManager.Generate(BulletManager.EnemyBulletIndex);
        bullet.Fire(this, FireTransform.position, -FireTransform.right, BulletSpeed, Damage);
    }

    protected override void OnDead(Actor killer)
    {
        base.OnDead(killer);

        SystemManager.Instance.GamePointAccumulator.Accumulate(GamePoint);
        SystemManager.Instance.EnemyManager.RemoveEnemy(this);

        CurrentState = State.Dead;
    }
}
