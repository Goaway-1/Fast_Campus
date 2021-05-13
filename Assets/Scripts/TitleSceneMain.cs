using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneMain : BaseSceneMain
{
    public void OnStartButton() //NetworkConfigPanel을 화면에 출력
    {
        PanelManager.GetPanel(typeof(NetworkConfigPanel)).Show();   
    }

    public void GotoNextScene()
    {
        SceneController.Instance.LoadScene(SceneNameConstants.LoadingScene);
    }
}
