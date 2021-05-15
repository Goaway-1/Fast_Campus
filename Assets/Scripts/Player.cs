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

    InputController inputController = new InputController();

    [SyncVar]
    bool Host = false; //Host 플레이어 판단

    protected override void Initialize()
    {
        base.Initialize();
        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHP(CurrentHP, MaxHP);   //초기화


        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        if (isLocalPlayer) inGameSceneMain.Hero = this;

        if(isServer && isLocalPlayer)
        {
            Host = true;
            UpdateNetworkActor();
        }

        Transform startTransform;

        if (Host)   //서버인 경우
            startTransform = inGameSceneMain.PlayerStartTransform1;
        else
            startTransform = inGameSceneMain.PlayerStartTransform2;

        SetPosition(startTransform.position);
    }

    protected override void UpdateActor()   //Actor의 Update와 연관
    {
        UpdateInput();
        UpdateMove();
    }

    [ClientCallback]    //내 클라이언트에서만 실행이된다.
    public void UpdateInput()
    {
        inputController.UpdateInput();
    }

    void UpdateMove()
    {
        if (MoveVector.sqrMagnitude == 0)   //백터의 값이 모두 0인지 확인
            return;

        //정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행시
        //cmdMove()

        // MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        // 이경우 클라이언트로 접속하면 Command로 보내지지만 자기자신은 CmdMove를 실행하지 못함
        if (isServer)
        {
            RpcMove(MoveVector);    //Host인 경우 RpcMove로 보내고
        }
        else
        {
            CmdMove(MoveVector);    //Client 플레이어인 경우 Cmd로 호스트를 보낸후 자기을 self 동작
            if (isLocalPlayer)
                transform.position += AdjustMoveVector(MoveVector);
        }
    }

    [Command]   //Cmd형식은 Command로 동작 -> 클라이언트가 서버한테 보내는
    public void CmdMove(Vector3 moveVector)
    {
        this.MoveVector = moveVector;
        transform.position += moveVector;
        base.SetDirtyBit(1);            //MoveVector가 SyncVar인데 바뀌였다. (서버에 통보)
        this.MoveVector = Vector3.zero; //타 플레이어가 보낸경우 Update를 통해 초기화 되지 않으므로 사용후 바로 초기화
    }

    [ClientRpc]     
    public void RpcMove(Vector3 moveVector)
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
