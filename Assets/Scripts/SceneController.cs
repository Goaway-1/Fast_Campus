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
        SceneManager.activeSceneChanged += OnActiveSceneChanged;    //액티브된 씬이 바뀌였을때
        SceneManager.sceneLoaded += OnSceneLoaded;    //씬이 로드되었을때
        SceneManager.sceneUnloaded += OnSceneUnLoaded;   //씬이 언로드 되었을때
    }

    //이전 Scene을 Unload하고 로딩 (1가지 씬만 존재)
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Single));
    }

    //이전 Scene을 Unload 없이 로딩 (2가지 씬 존재)
    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Additive));
    }

    IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)   //로드씬 (비동기방식)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

        while (!asyncOperation.isDone)  //끝나면 종료
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
