using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Actor : NetworkBehaviour
{
    [SerializeField]
    [SyncVar]
    protected int MaxHP = 100;  //체력

    public int HpMax
    {
        get { return MaxHP; }
    }

    [SerializeField]
    [SyncVar]
    protected int CurrentHP;    //현재 체력

    public int HPCurrent
    {
        get { return CurrentHP; }
    }

    [SerializeField]
    [SyncVar]
    protected int Damage = 1;   //총알 데미지

    [SerializeField]
    [SyncVar]
    protected int crashDamage = 100;    //충돌 데미지
    
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
    protected int actorInstanceID = 0;  //서버의 아이디

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
            actorInstanceID = GetInstanceID();      //각각의 오브젝트마다 ID가 있다.
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

    public virtual void OnBulletHited(int damage, Vector3 hitPos)   //총알에 피격시
    {
        Debug.Log("OnBulletHited damage = " + damage);
        DecreaseHP(damage,hitPos);
    }

    public virtual void OnCrash(int damage, Vector3 crashPos)     //기체에 피격시
    {
        Debug.Log("OnCrash damage = " + damage);
        DecreaseHP(damage, crashPos);
    }

    protected virtual void DecreaseHP(int value, Vector3 damagePos)  //체력 감소 (외부 호출)
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

    protected virtual void InternalDecreaseHP(int value, Vector3 damagePos)  //체력 감소 (내부 호출)
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
        //정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때
        //CmdSetPosition 사용

        //MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        if (isServer)   //Host의 경우
        {
            RpcSetPosition(position);
        }
        else            //Client인 경우
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
        base.SetDirtyBit(1);    //동기화?
    }

    [ClientRpc]
    public void RpcSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcSetActive(bool value)    //호스트에서만 호출 가능
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
