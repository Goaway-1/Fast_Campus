using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public const int BulletDisapperFxIndex = 0;
    public const int ActorDeadFxIndex = 1;

    [SerializeField]
    PrefabsCacheData[] effectFiles;

    Dictionary<string, GameObject> FileCache = new Dictionary<string, GameObject>();

    private void Start()
    {
        Prepare();
    }

    public GameObject Load(string resourcePath) //EnemyFactory의 Load와 유사
    {
        GameObject go = null;

        if (FileCache.ContainsKey(resourcePath)) //캐시가 있다면
        {
            go = FileCache[resourcePath];
        }
        else        //캐시가 없다면 
        {
            go = Resources.Load<GameObject>(resourcePath);
            if (!go)
            {
                Debug.LogError("Load Error! path = " + resourcePath);
                return null;
            }
            FileCache.Add(resourcePath, go);
        }
        return go;
    }

    public GameObject GenerateEffect(int index, Vector3 position)   //생성
    {
        if(index < 0 || index > effectFiles.Length)
        {
            Debug.LogError("GenerateEffect error! out of range! index = " + index);
            return null;
        }
        string filePath = effectFiles[index].filePath;
        GameObject go = SystemManager.Instance.EffectCacheSystem.Archive(filePath);
        go.transform.position = position;

        AutoCachableEffect effect = go.GetComponent<AutoCachableEffect>();
        effect.FilePath = filePath;

        return go;
    }

    public bool RemoveEffect(AutoCachableEffect effect)    //삭제해라
    {
        SystemManager.Instance.EffectCacheSystem.Restore(effect.FilePath, effect.gameObject);

        return true;
    }

    public void Prepare()   //초기 단체 생성
    {
        for (int i = 0; i < effectFiles.Length; i++)
        {
            GameObject go = Load(effectFiles[i].filePath);
            SystemManager.Instance.EffectCacheSystem.GenerateCache(effectFiles[i].filePath, go, effectFiles[i].cacheCount);
        }
    }
}
