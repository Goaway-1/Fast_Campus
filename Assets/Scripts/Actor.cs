using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Actor : NetworkBehaviour
{
    [SerializeField]
    [SyncVar]
    protected int MaxHP = 100;  //ü��

    public int HpMax
    {
        get { return MaxHP; }
    }

    [SerializeField]
    [SyncVar]
    protected int CurrentHP;    //���� ü��

    public int HPCurrent
    {
        get { return CurrentHP; }
    }

    [SerializeField]
    [SyncVar]
    protected int Damage = 1;   //�Ѿ� ������

    [SerializeField]
    [SyncVar]
    protected int crashDamage = 100;    //�浹 ������
    
    [SerializeField]
    [SyncVar]
    protected bool isDead = false;

    public bool IsDead
    {
        get { return isDead; }
    }

    protected int CrashDamage
    {
        get { return crashDamage; }
    }

    [SyncVar]
    protected int actorInstanceID = 0;  //������ ���̵�

    public int ActorInstanceID
    {
        get { return actorInstanceID; }
    }

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        CurrentHP = MaxHP;

        if (isServer)   //Host
        {
            actorInstanceID = GetInstanceID();      //������ ������Ʈ���� ID�� �ִ�.
            RpcSetActorInstanceID(actorInstanceID);
        }
    }

    private void Update()
    {
        UpdateActor();
    }
    protected virtual void UpdateActor()
    {

    }

    public virtual void OnBulletHited(int damage, Vector3 hitPos)   //�Ѿ˿� �ǰݽ�
    {
        Debug.Log("OnBulletHited damage = " + damage);
        DecreaseHP(damage,hitPos);
    }

    public virtual void OnCrash(int damage, Vector3 crashPos)     //��ü�� �ǰݽ�
    {
        Debug.Log("OnCrash damage = " + damage);
        DecreaseHP(damage, crashPos);
    }

    protected virtual void DecreaseHP(int value, Vector3 damagePos)  //ü�� ���� (�ܺ� ȣ��)
    {
        if (isDead)
            return;

        if (isServer)
        {
            RpcDecreasedHP(value, damagePos);
        }
        else
        {
            CmdDecreasedHP(value, damagePos);
            if (isLocalPlayer)
                InternalDecreaseHP(value, damagePos);
        }
    }

    protected virtual void InternalDecreaseHP(int value, Vector3 damagePos)  //ü�� ���� (���� ȣ��)
    {
        if (isDead)
            return;

        CurrentHP -= value;

        if (CurrentHP < 0)
            CurrentHP = 0;

        if (CurrentHP == 0)
            OnDead();
    }

    protected virtual void OnDead()
    {
        Debug.Log(name + "OnDead");
        isDead = true;

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectManager.ActorDeadFxIndex, transform.position);
    }
    public void SetPosition(Vector3 position)
    {
        //���������� NetworkBehaviour �ν��Ͻ��� Update�� ȣ��Ǿ� ����ǰ� ������
        //CmdSetPosition ���

        //MonoBehaviour �ν��Ͻ��� Update�� ȣ��Ǿ� ����ǰ� �������� �ļ�
        if (isServer)   //Host�� ���
        {
            RpcSetPosition(position);
        }
        else            //Client�� ���
        {
            CmdSetPosition(position);
            if (isLocalPlayer)
                transform.position = position;
        }
    }

    [Command]
    public void CmdSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);    //����ȭ?
    }

    [ClientRpc]
    public void RpcSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcSetActive(bool value)    //ȣ��Ʈ������ ȣ�� ����
    {
        this.gameObject.SetActive(value);
        base.SetDirtyBit(1);
    }

    public void UpdateNetworkActor()
    {
        if (isServer) RpcUpdateNetworkActor();
        else CmdUpdateNetworkActor();
    }

    [Command]
    public void CmdUpdateNetworkActor()
    {
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcUpdateNetworkActor()
    {
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcSetActorInstanceID(int instID)
    {
        this.actorInstanceID = instID;

        if (this.actorInstanceID != 0)
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.Regist(this.actorInstanceID, this);

        base.SetDirtyBit(1);
    }

    [Command]
    public void CmdDecreasedHP(int value, Vector3 damagePos)
    {
        InternalDecreaseHP(value, damagePos);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcDecreasedHP(int value, Vector3 damagePos)
    {
        InternalDecreaseHP(value, damagePos);
        base.SetDirtyBit(1);
    }
}
