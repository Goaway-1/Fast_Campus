using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public enum State : int //int ���
    { 
        None = -1,  //�����
        Ready = 0,  //�غ�Ϸ�
        Appear,     //����
        Battle,     //������
        Dead,       //���
        Disapper,   //����
    }

    [SerializeField]
    State CurrentState = State.None;        //���� ����

    [SerializeField]
    Vector3 TargetPostion;  //���� ��ǥ���� ��ġ


    //�ӵ� ����
    const float MaxSpeed = 10.0f;       //������ �ʴ� �ӵ� ��(������ �̿��Ҳ���)
    const float MaxSpeedTime = 0.5f;    //���� �ð�
    
    [SerializeField]
    float CurrentSpeed;     //���� �ӵ�

    Vector3 CurrentVelocity;
    float MoveStartTime = 0.0f; //�����̱� ������ �ð� --> �ӵ��� ���� ���� ��Ű�� ����

    //�Ѿ˰� ����
    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    float BulletSpeed = 1f;

    float LastActionUpdateTime = 0.0f;  //���� ���������� �ο���

    [SerializeField]
    int FireRemainCount = 3;    //�Ѿ��� �Ѱ�

    int GamePoint = 10; //���� ����

    //ĳ�� ����
    public string FilePath
    {
        get; set;
    }

    Vector3 AppearPoint;        //����� ���� ��ġ
    Vector3 DisappearPoint;     //����� ��ǥ ��ġ

    protected override void UpdateActor()
    {
        switch(CurrentState)    //���� ���¿� ���� �ൿ��
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

    void UpdateSpeed()  //�ӵ��� ����
    {
        CurrentSpeed = Mathf.Lerp(CurrentSpeed,MaxSpeed, (Time.time - MoveStartTime)/MaxSpeedTime);    //�� �� ������ ��򰡸� ��ȯ
    }

    void UpdateMove()   //�̵�
    {
        float distance = Vector3.Distance(TargetPostion, transform.position);   //Ÿ�ٱ����� �Ÿ�
        if (distance == 0)  //���� �Ǵ�
        {
            Arrived();
            return;
        }
        CurrentVelocity = (TargetPostion - transform.position).normalized * CurrentSpeed; //�ʴ� ���� ����(ũŰ 1)

        //���������� �̵��ϴ� �κ� --> �ӵ� = �Ÿ�/�ð�
        transform.position = Vector3.SmoothDamp(transform.position, TargetPostion, ref CurrentVelocity, distance/CurrentSpeed, MaxSpeed);   //�ڿ������� �̵�
    }
    void Arrived()      //���� �˸�
    {
        CurrentSpeed = 0;
        if (CurrentState == State.Appear)
        {
            CurrentState = State.Battle;
            LastActionUpdateTime = Time.time;    //��Ʋ ���� �ð�
        }
        else //if (CurrentState == State.Disapper)
        {
            CurrentState = State.None;
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);
        }
    }

    public void Reset(SquadronMemberStruct data)   //�ʱ�ȭ ���
    {
        EnemyStruct enemyStruct = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyID);

        //Enemy ����
        CurrentHP = MaxHP = enemyStruct.MaxHP;
        Damage = enemyStruct.Damage;
        crashDamage = enemyStruct.CrashDamage;
        BulletSpeed = enemyStruct.BulletSpeed;
        FireRemainCount = enemyStruct.FireRemainCount;
        GamePoint = enemyStruct.GamePoint;

        //Squadron ����
        AppearPoint = new Vector3(data.AppearPointX, data.AppearPointY,0);
        DisappearPoint = new Vector3(data.DisappearPointX, data.DisappearPointY, 0);

        CurrentState = State.Ready;
        LastActionUpdateTime = Time.time;
    }

    public void Appear(Vector3 targetPos)   //����
    {
        TargetPostion = targetPos;
        CurrentSpeed = MaxSpeed;

        CurrentState = State.Appear;
        MoveStartTime = Time.time;
    }
    void Disapper(Vector3 targetPos)    //�Ҹ�
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
    private void OnTriggerEnter(Collider other) //������ ������ ���´�.
    {
        Player player = other.GetComponentInParent<Player>(); //�ε�ģ�Ŵ� �ڽ� �ݶ��̴��ϱ� ������ �θ� ȣ��
        if (player)
        {
            if (!player.IsDead)
            {
                BoxCollider box = ((BoxCollider)other);
                Vector3 crashPos = player.transform.position + box.center;
                crashPos.x += box.size.x * 0.5f;

                player.OnCrash(this, crashDamage, crashPos);
            }
        }
    }

    public override void OnCrash(Actor attacker, int damage, Vector3 crachPos)    //���� �ε�ģ��
    {
        base.OnCrash(attacker, damage, crachPos);
    }

    public void Fire()
    {
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.EnemyBulletIndex);
        bullet.Fire(this, FireTransform.position, -FireTransform.right, BulletSpeed, Damage);
    }

    protected override void OnDead(Actor killer)
    {
        base.OnDead(killer);

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.Accumulate(GamePoint);
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);

        CurrentState = State.Dead;
    }

    protected override void DecreaseHP(Actor attacker, int value, Vector3 damagePos)
    {
        base.DecreaseHP(attacker, value, damagePos);

        Vector3 damagePoint = damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Generate(DamageManager.EnemyDamageIndex, damagePoint, value, Color.magenta);
    }
}
