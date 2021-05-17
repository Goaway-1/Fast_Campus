using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PrefabsCacheData   //prefab�� ������ ->�Ѿ�,���� ���
{
    public string filePath;
    public int cacheCount;
}

public class PrefabCacheSystem 
{
    Dictionary<string, Queue<GameObject>> Caches = new Dictionary<string, Queue<GameObject>>(); //���� ���� ���������� Ÿ��

    public void GenerateCache(string filePath,GameObject gameObject, int cacheCount, Transform parentTransform = null)    //�ʱ� ����
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
                GameObject go = Object.Instantiate<GameObject>(gameObject,parentTransform); //object�� ���� Bemonohavor�� ������� �ʾƼ�
                go.SetActive(false);
                queue.Enqueue(go);  //�־��ش�.

                //enemy�� ������ ��Ʈ��ũ���� spwan�� ���ش�. ���� �ѹ����� ������ ����
                Enemy enemy = go.GetComponent<Enemy>();
                if(enemy != null)
                {
                    enemy.FilePath = filePath;
                    NetworkServer.Spawn(go);
                }

                //bullet�� ������ ��Ʈ��ũ���� spwan�� ���ش�. ���� �ѹ����� ������ ����
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

    public GameObject Archive(string filePath)  //�Ҵ�
    {
        if (!Caches.ContainsKey(filePath))  //Ű���� ������
        {
            Debug.LogError("Archive Error! no Cache Generated! FilePath = " + filePath);
            return null;
        }
        
        if(Caches[filePath].Count == 0) //������ ���� 0�� �϶�
        {
            Debug.LogError("Archive problem! not Enough Count!");
            return null;
        }

        GameObject go = Caches[filePath].Dequeue(); //�������� �ѱ��.
        go.SetActive(true);

        if (((FWNetworkManager)FWNetworkManager.singleton).isServer)    //�ѹ��� �˻�
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

    public bool Restore(string filePath, GameObject gameObject) //�ݳ�
    {
        if (!Caches.ContainsKey(filePath))
        {
            Debug.LogError("Restore Error! no Cache Generated! FilePath = " + filePath);
            return false;
        }

        gameObject.SetActive(false);

        if (((FWNetworkManager)FWNetworkManager.singleton).isServer)    //�ѹ��� �˻�
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

    public void Add(string filePath, GameObject gameObject) //�ܺο��� EnemyCache�� �߰��� �� �ֵ���
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
