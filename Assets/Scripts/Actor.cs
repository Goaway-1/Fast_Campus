using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField]
    protected int MaxHP = 100;  //체력

    [SerializeField]
    protected int CurrentHP;    //현재 체력

    [SerializeField]
    protected int Damage = 1;   //총알 데미지

    [SerializeField]
    protected int crashDamage = 100;    //충돌 데미지

    private bool isDead = false;

    public bool IsDead
    {
        get { return isDead; }
    }

    protected int CrashDamage
    {
        get { return crashDamage; }
    }

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        CurrentHP = MaxHP;
    }

    private void Update()
    {
        UpdateActor();
    }
    protected virtual void UpdateActor()
    {

    }

    public virtual void OnBulletHited(Actor attacker, int damage, Vector3 hitPos)   //총알에 피격시
    {
        Debug.Log("OnBulletHited damage = " + damage);
        DecreaseHP(attacker,damage,hitPos);
    }

    public virtual void OnCrash(Actor attacker,int damage, Vector3 crashPos)     //기체에 피격시
    {
        Debug.Log("OnCrash damage = " + damage);
        DecreaseHP(attacker,damage, crashPos);
    }

    protected virtual void DecreaseHP(Actor attacker, int value, Vector3 damagePos)  //체력 감소 (외불 호출 X)
    {
        if (isDead)
            return;

        CurrentHP -= value;

        if (CurrentHP < 0)
            CurrentHP = 0;

        if (CurrentHP == 0)
            OnDead(attacker);
    }

    protected virtual void OnDead(Actor killer)
    {
        Debug.Log(name + "OnDead");
        isDead = true;

        SystemManager.Instance.EffectManager.GenerateEffect(EffectManager.ActorDeadFxIndex, transform.position);
    }
}
