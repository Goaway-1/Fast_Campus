using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //발사 및 이동
    [SerializeField]
    Vector3 MoveDirection = Vector3.zero;

    [SerializeField]
    float Speed = 10.0f;

    bool NeedMove = false;  //이동이 필요한지

    //충돌 감지
    bool Hited = false;

    //폭파 (시간, 거리)
    const float LifeTime = 15f; //시간
    float FireTime = 0;

    [SerializeField]
    int Damage = 1;

    //현재 문제의 아이
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

    public void Fire(Actor owner, Vector3 firePostion, Vector3 direction, float speed, int damage)  //외부에서 접근
    {
        Owner = owner;
        transform.position = firePostion;
        MoveDirection = direction;
        Speed = speed;
        Damage = damage;

        NeedMove = true;
        FireTime = Time.time;
    }

    //움직이는데 안 닿을 수도 있다.
    Vector3 AdjustMove(Vector3 moveVector)    //움직이는데 안 닿을 수도 있다.
    {
        RaycastHit hitinfo;

        if(Physics.Linecast(transform.position, transform.position + moveVector,out hitinfo))   //강제이동 time.detlatime이 프레임별 영향이 다르기 때문에
        {
            moveVector = hitinfo.point - transform.position;
            OnBulletCollision(hitinfo.collider);
        }

        return moveVector;  //이상없음
    }

    void OnBulletCollision(Collider collider)   //Bullet이 어디가에 닿았을때
    {
        //중복충돌방지
        if (Hited)
            return;

        //총알끼리 충돌방지
        if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyBullet") || collider.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))        //Layer를 int형으로 가져올수 있다.
        {
            return;
        }

        Actor actor = collider.GetComponentInParent<Actor>();   //이거 뭔지봐보바봐보바ㅗ바ㅗ바봐
        if (actor && actor.IsDead)
            return;

        actor.OnBulletHited(Owner, Damage);


        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        Hited = true;
        NeedMove = false;

        Disapper();
    }

    private void OnTriggerEnter(Collider other) //두번 들어올수도 있따.
    {
        OnBulletCollision(other);
    }

    bool ProcessDisapperCondition() //총알의 파괴
    {
        if(transform.position.x > 15f || transform.position.x < -15f ||
            transform.position.y > 15f || transform.position.y < -15f) //1. 거리
        {
            Disapper();
            return true;
        }
        else if(Time.time - FireTime > LifeTime)    //2.시간
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
