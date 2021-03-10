using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SquadronData
{
    public float SquadronGenerateTime;      //시간이 되면
    public Squadron squadron;               //이 스쿼드론 생성
}

public class SquadronManager : MonoBehaviour
{
    float GameStartedTime;  //시작시간

    int SquadronIndex;      //몇개만들껀디

    [SerializeField]
    SquadronData[] squadronDatas;

    bool running = false;   //게임 실행중?

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartGame();
        }
        CheckSquadronGeneratings();
    }

    public void StartGame() //게임 시작
    {
        GameStartedTime = Time.time;
        SquadronIndex = 0;
        running = true;
        Debug.Log("Game Started!");
    }

    void CheckSquadronGeneratings() //생성을 체크
    {
        if (!running)
            return;

        if(Time.time - GameStartedTime >= squadronDatas[SquadronIndex].SquadronGenerateTime)
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
    void GenerateSquadron(SquadronData data)    //스쿼드론 생성
    {
        Debug.Log("GenerateSquadron");
    }
    void AllSquadronGenerated() //모든 스쿼드론 종료
    {
        Debug.Log("AllSquadronGenerated");

        running = false;
    }
}
