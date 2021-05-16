using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    //발사 및 이동
    [SerializeField]
    [SyncVar]
    Vector3 MoveDirection = Vector3.zero;

    [SerializeField]
    [SyncVar]
    float Speed = 0.0f;

    [SyncVar]
    bool NeedMove = false;  //이동이 필요한지

    //충돌 감지
    [SyncVar]
    bool Hited = false;

    //폭파 (시간, 거리)
    const float LifeTime = 15f; //시간

    [SyncVar]
    float FireTime = 0;

    [SyncVar]
    [SerializeField]
    int Damage = 1;

    [SyncVar]
    [SerializeField]
    int OwnerInstancID; //Owner 해결

    [SyncVar]
    [SerializeField]
    string filePath;

    //캐싱관련
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

    public void Fire(int ownerInstanceID, Vector3 firePostion, Vector3 direction, float speed, int damage)  //외부에서 접근
    {
        OwnerInstancID = ownerInstanceID;
        SetPosition(firePostion);   //좌표변경
        MoveDirection = direction;
        Speed = speed;
        Damage = damage;

        NeedMove = true;
        FireTime = Time.time;

        UpdateNetworkBullet();  //지속적
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

        Actor owner = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.GetActor(OwnerInstancID);
        Actor actor = collider.GetComponentInParent<Actor>();   //이거 뭔지봐보바봐보바ㅗ바ㅗ바봐
        if (actor && actor.IsDead || actor.gameObject.layer == owner.gameObject.layer) 
            return;

        actor.OnBulletHited(Damage, transform.position);

        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        Hited = true;
        NeedMove = false;

        //이펙트 추가
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectManager.BulletDisapperFxIndex, transform.position);
        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
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
        if (isServer) RpcSetPosition(position);   //Host의 경우
        else                                    //Client의 경우
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
        if (isServer) RpcUpdateNetworkBullet();   //Host의 경우
        else CmdUpdateNetworkBullet();            //Client의 경우
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
