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

    float LastBattleUpdateTime = 0.0f;  //���� ���������� �ο���

    [SerializeField]
    int FireRemainCount = 3;    //�Ѿ��� �Ѱ�

    int GamePoint = 10; //���� ����

    //ĳ�� ����
    public string FilePath
    {
        get; set;
    }

    protected override void UpdateActor()
    {
        switch(CurrentState)    //���� ���¿� ���� �ൿ��
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
            LastBattleUpdateTime = Time.time;    //��Ʋ ���� �ð�
        }
        else //if (CurrentState == State.Disapper)
        {
            CurrentState = State.None;
        }
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
    private void OnTriggerEnter(Collider other) //������ ������ ���´�.
    {
        Player player = other.GetComponentInParent<Player>(); //�ε�ģ�Ŵ� �ڽ� �ݶ��̴��ϱ� ������ �θ� ȣ��
        if (player)
        {
            player.OnCrash(this,crashDamage);   
        }
    }

    public override void OnCrash(Actor attacker, int damage)    //���� �ε�ģ��
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
