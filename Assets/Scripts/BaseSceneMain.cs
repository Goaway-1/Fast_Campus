using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSceneMain : MonoBehaviour
{
    private void Awake()
    {
        OnAwake();
    }
    private void Start()
    {
        OnStart();
    }
    private void Update()
    {
        UpdateScene();
    }
    private void OnDestroy()
    {
        Terminate();
    }

    protected virtual void OnAwake()
    {

    }
    protected virtual void OnStart()
    {
        
    }

    //외부에서 초기화 호출
    public virtual void Initialize()
    {

    }

    protected virtual void UpdateScene()
    {

    }
    protected virtual void Terminate()
    {

    }
}
