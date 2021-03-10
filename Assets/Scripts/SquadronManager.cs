using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SquadronData
{
    public float SquadronGenerateTime;      //�ð��� �Ǹ�
    public Squadron squadron;               //�� ������� ����
}

public class SquadronManager : MonoBehaviour
{
    float GameStartedTime;  //���۽ð�

    int SquadronIndex;      //����鲫��

    [SerializeField]
    SquadronData[] squadronDatas;

    bool running = false;   //���� ������?

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
    void GenerateSquadron(SquadronData data)    //������� ����
    {
        Debug.Log("GenerateSquadron");
    }
    void AllSquadronGenerated() //��� ������� ����
    {
        Debug.Log("AllSquadronGenerated");

        running = false;
    }
}
