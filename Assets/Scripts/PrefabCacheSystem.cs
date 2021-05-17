using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PrefabsCacheData   //prefab의 데이터 ->총알,몬스터 등등
{
    public string filePath;
    public int cacheCount;
}

public class PrefabCacheSystem 
{
    Dictionary<string, Queue<GameObject>> Caches = new Dictionary<string, Queue<GameObject>>(); //이중 저장 여러가지의 타입

    public void GenerateCache(string filePath,GameObject gameObject, int cacheCount, Transform parentTransform = null)    //초기 생성
    {
        if (Caches.ContainsKey(filePath))
        {
            Debug.Log("Already cache generated! filePath = " + filePath);
            return;
        }
        else
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            for (int i = 0; i < cacheCount; i++)
            {
                GameObject go = Object.Instantiate<GameObject>(gameObject,parentTransform); //object에 있음 Bemonohavor를 상속하지 않아서
                go.SetActive(false);
                queue.Enqueue(go);  //넣어준다.

                //enemy가 나오면 네트워크에서 spwan을 해준다. 추후 한번으로 수정할 예정
                Enemy enemy = go.GetComponent<Enemy>();
                if(enemy != null)
                {
                    enemy.FilePath = filePath;
                    NetworkServer.Spawn(go);
                }

                //bullet가 나오면 네트워크에서 spwan을 해준다. 추후 한번으로 수정할 예정
                Bullet bullet = go.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet.FilePath = filePath;
                    NetworkServer.Spawn(go);
                }
            }

            Caches.Add(filePath, queue);
        }
    }

    public GameObject Archive(string filePath)  //할당
    {
        if (!Caches.ContainsKey(filePath))  //키값이 없을때
        {
            Debug.LogError("Archive Error! no Cache Generated! FilePath = " + filePath);
            return null;
        }
        
        if(Caches[filePath].Count == 0) //생성된 수가 0개 일때
        {
            Debug.LogError("Archive problem! not Enough Count!");
            return null;
        }

        GameObject go = Caches[filePath].Dequeue(); //정보들을 넘긴다.
        go.SetActive(true);

        if (((FWNetworkManager)FWNetworkManager.singleton).isServer)    //한번더 검사
        {
            Enemy enemy = go.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RpcSetActive(true);
            }

            Bullet bullet = go.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.RpcSetActive(true);
            }
        }
        return go;
    }

    public bool Restore(string filePath, GameObject gameObject) //반납
    {
        if (!Caches.ContainsKey(filePath))
        {
            Debug.LogError("Restore Error! no Cache Generated! FilePath = " + filePath);
            return false;
        }

        gameObject.SetActive(false);

        if (((FWNetworkManager)FWNetworkManager.singleton).isServer)    //한번더 검사
        {
            Enemy enemy = gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RpcSetActive(false);
            }
            Bullet bullet = gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.RpcSetActive(false);
            }
        }
        Caches[filePath].Enqueue(gameObject);   
        return true;
    }

    public void Add(string filePath, GameObject gameObject) //외부에서 EnemyCache를 추가할 수 있도록
    {
        Queue<GameObject> queue;
        if (Caches.ContainsKey(filePath)) queue = Caches[filePath];
        else
        {
            queue = new Queue<GameObject>();
            Caches.Add(filePath, queue);
        }

        queue.Enqueue(gameObject);
    }
}
