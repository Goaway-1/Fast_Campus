using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    //�߻� �� �̵�
    [SerializeField]
    [SyncVar]
    Vector3 MoveDirection = Vector3.zero;

    [SerializeField]
    [SyncVar]
    float Speed = 0.0f;

    [SyncVar]
    bool NeedMove = false;  //�̵��� �ʿ�����

    //�浹 ����
    [SyncVar]
    bool Hited = false;

    //���� (�ð�, �Ÿ�)
    const float LifeTime = 15f; //�ð�

    [SyncVar]
    float FireTime = 0;

    [SyncVar]
    [SerializeField]
    int Damage = 1;

    [SyncVar]
    [SerializeField]
    int OwnerInstancID; //Owner �ذ�

    [SyncVar]
    [SerializeField]
    string filePath;

    //ĳ�̰���
    public string FilePath
    {
        get { return filePath; }
        set { filePath = value; }
    }

    void Start()
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer){
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            transform.SetParent(inGameSceneMain.BulletManager.transform);
            inGameSceneMain.BulletCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(false);
        }
    }

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

    public void Fire(int ownerInstanceID, Vector3 firePostion, Vector3 direction, float speed, int damage)  //�ܺο��� ����
    {
        OwnerInstancID = ownerInstanceID;
        SetPosition(firePostion);   //��ǥ����
        MoveDirection = direction;
        Speed = speed;
        Damage = damage;

        NeedMove = true;
        FireTime = Time.time;

        UpdateNetworkBullet();  //������
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

        Actor owner = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.GetActor(OwnerInstancID);
        Actor actor = collider.GetComponentInParent<Actor>();   //�̰� ���������ٺ����٤ǹ٤ǹٺ�
        if (actor && actor.IsDead || actor.gameObject.layer == owner.gameObject.layer) 
            return;

        actor.OnBulletHited(Damage, transform.position);

        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        Hited = true;
        NeedMove = false;

        //����Ʈ �߰�
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectManager.BulletDisapperFxIndex, transform.position);
        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
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
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Remove(this);
    }

    [ClientRpc]
    public void RpcSetActive(bool value)
    {
        this.gameObject.SetActive(value);
        base.SetDirtyBit(1);
    }

    public void SetPosition(Vector3 position)
    {
        if (isServer) RpcSetPosition(position);   //Host�� ���
        else                                    //Client�� ���
        {
            CmdSetPosition(position);
            if (isLocalPlayer) transform.position = position;
        }
    }

    [Command]
    public void CmdSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }

    public void UpdateNetworkBullet()
    {
        if (isServer) RpcUpdateNetworkBullet();   //Host�� ���
        else CmdUpdateNetworkBullet();            //Client�� ���
    }
    
    [Command]
    public void CmdUpdateNetworkBullet()
    {
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcUpdateNetworkBullet()
    {
        base.SetDirtyBit(1);
    }
}
