using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public const string EnemyFath = "Prefabs/Enemy";

    //�������� Ű�� ����Ͽ� ĳ��
    Dictionary<string, GameObject> EnemyFileCache = new Dictionary<string, GameObject>();

    public GameObject Load(string resourcePath)
    {
        GameObject go = null;

        if (EnemyFileCache.ContainsKey(resourcePath))        //�̹� �ε�Ǿ��ִ� ��� -> ó���� ������ ����
        {
            go = EnemyFileCache[resourcePath];    //�޸� �� �ö�� �ִ� ���� �����´�.
        }
        else     //�� ó������ ����ǰڴ�.
        {
            go = Resources.Load<GameObject>(resourcePath);  //�������� �޸𸮿� �ε��Ѵ�.
            if (!go)
            {
                Debug.LogError("Load Error! path = " + resourcePath);
                return null;
            }

            EnemyFileCache.Add(resourcePath, go);
        }

        GameObject instancedGo = Instantiate<GameObject>(go);

        return instancedGo;
    }
}
