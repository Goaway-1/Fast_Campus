using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//게임 점수 관리
public class GamePointAccumulator
{
    int gamePoint = 0;

    public int GamePoint
    {
        get { return gamePoint; }
    }
    
    public void Accumulate(int value)
    {
        gamePoint += value;

        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetScore(gamePoint);
    }
    public void Reset()
    {
        gamePoint = 0;
    }
}
