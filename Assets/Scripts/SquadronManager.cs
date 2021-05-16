using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadronManager : MonoBehaviour
{
    float GameStartedTime;  //시작시간

    int ScheduleIndex;      //몇개만들껀디

    [SerializeField]
    SquadronTable[] squadronDatas;

    [SerializeField]
    SquadronScheduleTable squadronScheduleTable;

    bool running = false;   //게임 실행중?

    void Start()
    {
        squadronDatas = GetComponentsInChildren<SquadronTable>();   //아래있는 테이블 다 가져옴
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

    public void StartGame() //게임 시작
    {
        GameStartedTime = Time.time;
        ScheduleIndex = 0;
        running = true;
        Debug.Log("Game Started!");
    }

    void CheckSquadronGeneratings() //생성을 체크
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
    void GenerateSquadron(SquadronTable table)    //스쿼드론 생성
    {
        Debug.Log("GenerateSquadron : " + ScheduleIndex);
        for (int i = 0; i < table.GetCount(); i++)
        {
            SquadronMemberStruct squadronMember = table.GetSquadronMember(i);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.GenerateEnemy(squadronMember);
        }
    }

    void AllSquadronGenerated() //모든 스쿼드론 생성
    {
        Debug.Log("AllSquadronGenerated");

        running = false;
    }
}
