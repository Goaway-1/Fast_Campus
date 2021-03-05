using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{  
    //�̵��� ����
    Vector3 MoveVector = Vector3.zero;
    float Speed = 5;

    [SerializeField]
    BoxCollider boxCollider;

    [SerializeField]
    Transform MainBGQuadTransform;

    //�Ѿ˰� ����
    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    float BulletSpeed = 1f;


    protected override void UpdateActor()   //Actor�� Update�� ����
    {
        UpdateMove();
    }

    void UpdateMove()
    {
        if (MoveVector.sqrMagnitude == 0)   //������ ���� ��� 0���� Ȯ��
            return;

        MoveVector = AdjustMoveVector(MoveVector);

        transform.position += MoveVector;
    }

    public void ProcessInput(Vector3 moveDirection) //InputController���� ��� (�Է½�) 
    {
        MoveVector = moveDirection * Speed * Time.deltaTime;
    }

    Vector3 AdjustMoveVector(Vector3 moveVector)    //ȭ�� ������ ������ ���ϰ� (transform ���)
    {
        Vector3 result = Vector3.zero;
        result = boxCollider.transform.position + moveVector;   //�� �̵��� ���� ��ġ

        if(result.x - boxCollider.size.x * 0.5f < -MainBGQuadTransform.localScale.x * 0.5f)
        {
            moveVector.x = 0;
        }
        if (result.x + boxCollider.size.x * 0.5f > MainBGQuadTransform.localScale.x * 0.5f)
        {
            moveVector.x = 0;
        }
        if (result.y - boxCollider.size.y * 0.5f < -MainBGQuadTransform.localScale.y * 0.5f)
        {
            moveVector.y = 0;
        }
        if (result.y + boxCollider.size.y * 0.5f > MainBGQuadTransform.localScale.y * 0.5f)
        {
            moveVector.y = 0;
        }
        return moveVector;
    }

    private void OnTriggerEnter(Collider other) //������ ������ ���´�. ��밡 �ε�ģ��
    {
        Enemy enemy = other.GetComponentInParent<Enemy>(); //�ε�ģ�Ŵ� �ڽ� �ݶ��̴��ϱ� ������ �θ� ȣ��
        if (enemy)
        {
            enemy.OnCrash(this);  
        }
    }

    public void OnCrash(Enemy enemy)    //���� �ε�ģ��
    {
        Debug.Log("OnCrash enemy = " + enemy);
    }

    public void Fire()
    {
        GameObject go = Instantiate(Bullet);

        Bullet bullet = go.GetComponent<Bullet>();
        bullet.Fire(this, FireTransform.position, FireTransform.right, BulletSpeed, Damage);
    }
}
