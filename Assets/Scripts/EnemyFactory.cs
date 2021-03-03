using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public const string EnemyFath = "Prefabs/Enemy";

    //프리팹을 키로 사용하여 캐싱
    Dictionary<string, GameObject> EnemyFileCache = new Dictionary<string, GameObject>();

    public GameObject Load(string resourcePath)
    {
        GameObject go = null;

        if (EnemyFileCache.ContainsKey(resourcePath))        //이미 로드되어있는 경우 -> 처음을 제외한 과정
        {
            go = EnemyFileCache[resourcePath];    //메모리 상에 올라와 있는 것을 가져온다.
        }
        else     //맨 처음에만 보충되겠다.
        {
            go = Resources.Load<GameObject>(resourcePath);  //프리팹을 메모리에 로드한다.
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
