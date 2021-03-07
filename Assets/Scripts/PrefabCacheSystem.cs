using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabsCacheData   //prefab의 데이터 ->총알,몬스터 등등
{
    public string filePath;
    public int cacheCount;
}

public class PrefabCacheSystem 
{
    Dictionary<string, Queue<GameObject>> Caches = new Dictionary<string, Queue<GameObject>>(); //이중 저장 여러가지의 타입

    public void GenerateCache(string filePath,GameObject gameObject, int cacheCount)    //초기 생성
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
                GameObject go = Object.Instantiate<GameObject>(gameObject); //object에 있음 Bemonohavor를 상속하지 않아서
                go.SetActive(false);
                queue.Enqueue(go);  //넣어준다.
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

        Caches[filePath].Enqueue(gameObject);   
        return true;
    }
}
