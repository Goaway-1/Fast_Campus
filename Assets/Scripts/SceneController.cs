using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNameConstants
{
    public static string TitleScene = "TitleScene";
    public static string LoadingScene = "LoadingScene";
    public static string InGame = "InGaem";
}
public class SceneController : MonoBehaviour
{
    private static SceneController instance = null;

    public static SceneController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneController>();
                if (instance == null)
                {
                    GameObject container = new GameObject("SceneController");
                    instance = container.AddComponent<SceneController>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;    //��Ƽ��� ���� �ٲ����
        SceneManager.sceneLoaded += OnSceneLoaded;    //���� �ε�Ǿ�����
        SceneManager.sceneUnloaded += OnSceneUnLoaded;   //���� ��ε� �Ǿ�����
    }

    //���� Scene�� Unload�ϰ� �ε� (1���� ���� ����)
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Single));
    }

    //���� Scene�� Unload ���� �ε� (2���� �� ����)
    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Additive));
    }

    IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)   //�ε�� (�񵿱���)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

        while (!asyncOperation.isDone)  //������ ����
            yield return null;

        Debug.Log("LoadSceneAsyne is Complete!");
    }

    public void OnActiveSceneChanged(Scene scene0, Scene scene1)
    {
        Debug.Log("OnActiveSceneChanged is Called! Scene0 = " + scene0.name + ", Scene1 = " + scene1.name);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("OnSceneLoaded is Called! Scene = " + scene.name + ", loadSceneMode = " + loadSceneMode.ToString());
    }
    public void OnSceneUnLoaded(Scene scene)
    {
        Debug.Log("OnSceneUnLoaded is Called! Scene = " + scene.name);
    }
}
