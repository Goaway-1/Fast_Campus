using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public const int PlayerBulletIndex = 0;
    public const int EnemyBulletIndex = 1;

    //캐싱관련
    [SerializeField]
    PrefabsCacheData[] bulletFiles;

    Dictionary<string, GameObject> FileCache = new Dictionary<string, GameObject>();
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

    public void Prepare()
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
            return;

        for (int i = 0; i < bulletFiles.Length; i++)
        {
            GameObject go = Load(bulletFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletCacheSystem.GenerateCache(bulletFiles[i].filePath, go, bulletFiles[i].cacheCount,this.transform);
        }
    } 

    public Bullet Generate(int index)
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
            return null;

        string filePath = bulletFiles[index].filePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletCacheSystem.Archive(filePath);

        Bullet bullet = go.GetComponent<Bullet>();

        return bullet;
    }

    public bool Remove(Bullet bullet)
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
            return true;

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletCacheSystem.Restore(bullet.FilePath, bullet.gameObject);
        return true;
    }
}
