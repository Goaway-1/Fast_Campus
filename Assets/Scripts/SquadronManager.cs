using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadronManager : MonoBehaviour
{
    float GameStartedTime;  //���۽ð�

    int SquadronIndex;      //����鲫��

    [SerializeField]
    SquadronTable[] squadronDatas;

    [SerializeField]
    SquadronScheduleTable squadronScheduleTable;

    bool running = false;   //���� ������?

    void Start()
    {
        squadronDatas = GetComponentsInChildren<SquadronTable>();   //�Ʒ��ִ� ���̺� �� ������
        for (int i = 0; i < squadronDatas.Length; i++)
        {
            squadronDatas[i].Load();
        }

        squadronScheduleTable.Load();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartGame();
        }
        CheckSquadronGeneratings();
    }

    public void StartGame() //���� ����
    {
        GameStartedTime = Time.time;
        SquadronIndex = 0;
        running = true;
        Debug.Log("Game Started!");
    }

    void CheckSquadronGeneratings() //������ üũ
    {
        if (!running)
            return;

        if(Time.time - GameStartedTime >= squadronScheduleTable.GetScheduleData(SquadronIndex).GenerateTime)
        {
            GenerateSquadron(squadronDatas[SquadronIndex]);
            SquadronIndex++;

            if (SquadronIndex >= squadronDatas.Length)
            {
                AllSquadronGenerated();
                return;
            }
        }
    }
    void GenerateSquadron(SquadronTable table)    //������� ����
    {
        Debug.Log("GenerateSquadron");
        for (int i = 0; i < table.GetCount(); i++)
        {
            SquadronMemberStruct squadronMember = table.GetSquadronMember(i);
            SystemManager.Instance.EnemyManager.GenerateEnemy(squadronMember);
        }
    }

    void AllSquadronGenerated() //��� ������� ����
    {
        Debug.Log("AllSquadronGenerated");

        running = false;
    }
}
