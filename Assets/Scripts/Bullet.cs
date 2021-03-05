using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //�߻� �� �̵�
    [SerializeField]
    Vector3 MoveDirection = Vector3.zero;

    [SerializeField]
    float Speed = 10.0f;

    bool NeedMove = false;  //�̵��� �ʿ�����

    //�浹 ����
    bool Hited = false;

    //���� (�ð�, �Ÿ�)
    const float LifeTime = 15f; //�ð�
    float FireTime = 0;

    [SerializeField]
    int Damage = 1;

    //���� ������ ����
    Actor Owner;

    private void FixedUpdate()
    {
        if (ProcessDisapperCondition())
        {
            Disapper();
        }

        UpdateMove();
    }

    void UpdateMove()
    {
        if (!NeedMove)
            return;

        Vector3 moveVector = MoveDirection.normalized * Speed * Time.deltaTime;
        moveVector = AdjustMove(moveVector);
        transform.position += moveVector;
    }

    public void Fire(Actor owner, Vector3 firePostion, Vector3 direction, float speed, int damage)  //�ܺο��� ����
    {
        Owner = owner;
        transform.position = firePostion;
        MoveDirection = direction;
        Speed = speed;
        Damage = damage;

        NeedMove = true;
        FireTime = Time.time;
    }

    //�����̴µ� �� ���� ���� �ִ�.
    Vector3 AdjustMove(Vector3 moveVector)    //�����̴µ� �� ���� ���� �ִ�.
    {
        RaycastHit hitinfo;

        if(Physics.Linecast(transform.position, transform.position + moveVector,out hitinfo))   //�����̵� time.detlatime�� �����Ӻ� ������ �ٸ��� ������
        {
            moveVector = hitinfo.point - transform.position;
            OnBulletCollision(hitinfo.collider);
        }

        return moveVector;  //�̻����
    }

    void OnBulletCollision(Collider collider)   //Bullet�� ��𰡿� �������
    {
        //�ߺ��浹����
        if (Hited)
            return;

        //�Ѿ˳��� �浹����
        if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyBullet") || collider.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))        //Layer�� int������ �����ü� �ִ�.
        {
            return;
        }

        Actor actor = collider.GetComponentInParent<Actor>();   //�̰� ���������ٺ����٤ǹ٤ǹٺ�
        if (actor && actor.IsDead)
            return;

        actor.OnBulletHited(Owner, Damage);


        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        Hited = true;
        NeedMove = false;

        Disapper();
    }

    private void OnTriggerEnter(Collider other) //�ι� ���ü��� �ֵ�.
    {
        OnBulletCollision(other);
    }

    bool ProcessDisapperCondition() //�Ѿ��� �ı�
    {
        if(transform.position.x > 15f || transform.position.x < -15f ||
            transform.position.y > 15f || transform.position.y < -15f) //1. �Ÿ�
        {
            Disapper();
            return true;
        }
        else if(Time.time - FireTime > LifeTime)    //2.�ð�
        {
            Disapper();
            return true;
        }
        return false;
    }

    void Disapper()
    {
        Destroy(gameObject);
    }
}
