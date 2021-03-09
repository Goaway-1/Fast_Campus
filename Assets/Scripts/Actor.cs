using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField]
    protected int MaxHP = 100;  //ü��

    [SerializeField]
    protected int CurrentHP;    //���� ü��

    [SerializeField]
    protected int Damage = 1;   //�Ѿ� ������

    [SerializeField]
    protected int crashDamage = 100;    //�浹 ������

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

    public virtual void OnBulletHited(Actor attacker, int damage, Vector3 hitPos)   //�Ѿ˿� �ǰݽ�
    {
        Debug.Log("OnBulletHited damage = " + damage);
        DecreaseHP(attacker,damage,hitPos);
    }

    public virtual void OnCrash(Actor attacker,int damage, Vector3 crashPos)     //��ü�� �ǰݽ�
    {
        Debug.Log("OnCrash damage = " + damage);
        DecreaseHP(attacker,damage, crashPos);
    }

    protected virtual void DecreaseHP(Actor attacker, int value, Vector3 damagePos)  //ü�� ���� (�ܺ� ȣ�� X)
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
