using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : Actor
{  
    //�̵��� ����
    [SerializeField]
    [SyncVar]           //�̰� ����
    Vector3 MoveVector = Vector3.zero;

    [SerializeField]
    NetworkIdentity networkIdentity = null;

    [SerializeField]
    float Speed;

    [SerializeField]
    BoxCollider boxCollider;

    //�Ѿ˰� ����
    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    float BulletSpeed = 1f;

    InputController inputController = new InputController();

    [SyncVar]
    bool Host = false; //Host �÷��̾� �Ǵ�

    protected override void Initialize()
    {
        base.Initialize();
        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHP(CurrentHP, MaxHP);   //�ʱ�ȭ


        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        if (isLocalPlayer) inGameSceneMain.Hero = this;

        if(isServer && isLocalPlayer)
        {
            Host = true;
            UpdateNetworkActor();
        }

        Transform startTransform;

        if (Host)   //������ ���
            startTransform = inGameSceneMain.PlayerStartTransform1;
        else
            startTransform = inGameSceneMain.PlayerStartTransform2;

        SetPosition(startTransform.position);
    }

    protected override void UpdateActor()   //Actor�� Update�� ����
    {
        UpdateInput();
        UpdateMove();
    }

    [ClientCallback]    //�� Ŭ���̾�Ʈ������ �����̵ȴ�.
    public void UpdateInput()
    {
        inputController.UpdateInput();
    }

    void UpdateMove()
    {
        if (MoveVector.sqrMagnitude == 0)   //������ ���� ��� 0���� Ȯ��
            return;

        //���������� NetworkBehaviour �ν��Ͻ��� Update�� ȣ��Ǿ� �����
        //cmdMove()

        // MonoBehaviour �ν��Ͻ��� Update�� ȣ��Ǿ� ����ǰ� �������� �ļ�
        // �̰�� Ŭ���̾�Ʈ�� �����ϸ� Command�� ���������� �ڱ��ڽ��� CmdMove�� �������� ����
        if (isServer)
        {
            RpcMove(MoveVector);    //Host�� ��� RpcMove�� ������
        }
        else
        {
            CmdMove(MoveVector);    //Client �÷��̾��� ��� Cmd�� ȣ��Ʈ�� ������ �ڱ��� self ����
            if (isLocalPlayer)
                transform.position += AdjustMoveVector(MoveVector);
        }
    }

    [Command]   //Cmd������ Command�� ���� -> Ŭ���̾�Ʈ�� �������� ������
    public void CmdMove(Vector3 moveVector)
    {
        this.MoveVector = moveVector;
        transform.position += moveVector;
        base.SetDirtyBit(1);            //MoveVector�� SyncVar�ε� �ٲ��. (������ �뺸)
        this.MoveVector = Vector3.zero; //Ÿ �÷��̾ ������� Update�� ���� �ʱ�ȭ ���� �����Ƿ� ����� �ٷ� �ʱ�ȭ
    }

    [ClientRpc]     
    public void RpcMove(Vector3 moveVector)
    {
        this.MoveVector = moveVector;
        transform.position += moveVector;
        base.SetDirtyBit(1);        //MoveVector�� SyncVar�ε� �ٲ��. (������ �뺸)
    }

    public void ProcessInput(Vector3 moveDirection) //InputController���� ��� (�Է½�) 
    {
        MoveVector = moveDirection * Speed * Time.deltaTime;
    }

    Vector3 AdjustMoveVector(Vector3 moveVector)    //ȭ�� ������ ������ ���ϰ� (transform ���)
    {
        Transform mainBGQuadTransform = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().MainBGQuadTransform;
        Vector3 result = Vector3.zero;
        result = boxCollider.transform.position + moveVector;   //�� �̵��� ���� ��ġ

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

    private void OnTriggerEnter(Collider other) //������ ������ ���´�. ��밡 �ε�ģ��
    {
        Enemy enemy = other.GetComponentInParent<Enemy>(); //�ε�ģ�Ŵ� �ڽ� �ݶ��̴��ϱ� ������ �θ� ȣ��
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

    public override void OnCrash(Actor attacker, int damage, Vector3 crashPos)    //���� �ε�ģ��
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
        //��ȯ�� BasePanel�� ��ȯ�Ǳ� ������ as PlayerStatePanel�� ��ȯ����
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
