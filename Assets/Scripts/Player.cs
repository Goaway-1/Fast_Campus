using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : Actor
{  
    //이동과 관련
    [SerializeField]
    [SyncVar]           //이게 뭔데
    Vector3 MoveVector = Vector3.zero;

    [SerializeField]
    NetworkIdentity networkIdentity = null;

    [SerializeField]
    float Speed;

    [SerializeField]
    BoxCollider boxCollider;

    //총알과 관련
    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    float BulletSpeed = 1f;


    protected override void Initialize()
    {
        base.Initialize();
        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHP(CurrentHP, MaxHP);   //초기화

        if (isLocalPlayer)
        {
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Hero = this;
        }
    }

    protected override void UpdateActor()   //Actor의 Update와 연관
    {
        UpdateMove();
    }

    void UpdateMove()
    {
        if (MoveVector.sqrMagnitude == 0)   //백터의 값이 모두 0인지 확인
            return;

        MoveVector = AdjustMoveVector(MoveVector);

        //transform.position += MoveVector;
        CmdMove(MoveVector);
    }

    [Command]   //Cmd형식은 Command로 동작 -> 클라이언트가 서버한테 보내는
    public void CmdMove(Vector3 moveVector)
    {
        this.MoveVector = moveVector;
        transform.position += moveVector;
        base.SetDirtyBit(1);        //MoveVector가 SyncVar인데 바뀌였다. (서버에 통보)
    }

    public void ProcessInput(Vector3 moveDirection) //InputController에서 사용 (입력시) 
    {
        MoveVector = moveDirection * Speed * Time.deltaTime;
    }

    Vector3 AdjustMoveVector(Vector3 moveVector)    //화면 밖으로 나가지 못하게 (transform 사용)
    {
        Transform mainBGQuadTransform = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().MainBGQuadTransform;
        Vector3 result = Vector3.zero;
        result = boxCollider.transform.position + moveVector;   //곧 이동할 나의 위치

        if(result.x - boxCollider.size.x * 0.5f < -mainBGQuadTransform.localScale.x * 0.5f)
        {
            moveVector.x = 0;
        }
        if (result.x + boxCollider.size.x * 0.5f > mainBGQuadTransform.localScale.x * 0.5f)
        {
            moveVector.x = 0;
        }
        if (result.y - boxCollider.size.y * 0.5f < -mainBGQuadTransform.localScale.y * 0.5f)
        {
            moveVector.y = 0;
        }
        if (result.y + boxCollider.size.y * 0.5f > mainBGQuadTransform.localScale.y * 0.5f)
        {
            moveVector.y = 0;
        }
        return moveVector;
    }

    private void OnTriggerEnter(Collider other) //상대방의 정보가 나온다. 상대가 부딪친거
    {
        Enemy enemy = other.GetComponentInParent<Enemy>(); //부딪친거는 박스 콜라이더니까 상위인 부모 호출
        if (enemy)
        {
            if (!enemy.IsDead)
            {
                BoxCollider box = ((BoxCollider)other);
                Vector3 crashPos = enemy.transform.position + box.center;
                crashPos.x += box.size.x * 0.5f;

                enemy.OnCrash(this, crashDamage, crashPos);
            }
        }
    }

    public override void OnCrash(Actor attacker, int damage, Vector3 crashPos)    //내가 부딪친거
    {
        base.OnCrash(attacker, damage, crashPos);
    }

    public void Fire()
    {
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.PlayerBulletIndex);
        bullet.Fire(this, FireTransform.position, FireTransform.right, BulletSpeed, Damage);
    }
    protected override void DecreaseHP(Actor attacker, int value, Vector3 damagePos)
    {
        base.DecreaseHP(attacker, value, damagePos);
        //반환시 BasePanel이 반환되기 때문에 as PlayerStatePanel로 변환해줌
        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHP(CurrentHP, MaxHP);

        Vector3 damagePoint = damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Generate(DamageManager.PlayerDamageIndex, damagePoint, value, Color.red);
    }

    protected override void OnDead(Actor killer)
    {
        base.OnDead(killer);
        gameObject.SetActive(false);
    }
}
