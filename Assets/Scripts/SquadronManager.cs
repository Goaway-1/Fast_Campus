using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadronManager : MonoBehaviour
{
    float GameStartedTime;  //���۽ð�

    int ScheduleIndex;      //����鲫��

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
        CheckSquadronGeneratings();
    }

    public void StartGame() //���� ����
    {
        GameStartedTime = Time.time;
        ScheduleIndex = 0;
        running = true;
        Debug.Log("Game Started!");
    }

    void CheckSquadronGeneratings() //������ üũ
    {
        if (!running)
            return;

        SquadronScheduleDataStruct data = squadronScheduleTable.GetScheduleData(ScheduleIndex);

        if(Time.time - GameStartedTime >= data.GenerateTime)
        {
            GenerateSquadron(squadronDatas[data.SquadronID]);
            ScheduleIndex++;

            if (ScheduleIndex >= squadronScheduleTable.GetDataCount())
            {
                AllSquadronGenerated();
                return;
            }
        }
    }
    void GenerateSquadron(SquadronTable table)    //������� ����
    {
        Debug.Log("GenerateSquadron : " + ScheduleIndex);
        for (int i = 0; i < table.GetCount(); i++)
        {
            SquadronMemberStruct squadronMember = table.GetSquadronMember(i);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.GenerateEnemy(squadronMember);
        }
    }

    void AllSquadronGenerated() //��� ������� ����
    {
        Debug.Log("AllSquadronGenerated");

        running = false;
    }
}
