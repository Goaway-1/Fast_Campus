using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    EnemyFactory enemyFactory;

    List<Enemy> enemies = new List<Enemy>();

    public List<Enemy> Enemies 
    {
        get { return enemies; }
    }

    //캐싱관련
    [SerializeField]
    PrefabsCacheData[] enemyFiles;

    private void Start()
    {
        Prepare();
    }

    private void Update()
    {
    }

    public bool GenerateEnemy(SquadronMemberStruct data)   //만들어줘라
    {
        string FilePath = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyID).FilePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyCacheSystem.Archive(FilePath);  //프리펩 호출

        go.transform.position = new Vector3(data.GeneratePointX, data.GeneratePointY,0);

        Enemy enemy = go.GetComponent<Enemy>();
        enemy.FilePath = FilePath;
        enemy.Reset(data);

        enemies.Add(enemy);
        return true;
    }

    public bool RemoveEnemy(Enemy enemy)    //삭제해라
    {
        if (!enemies.Contains(enemy))   //키값이 아니라 이 오브젝트가 없다면
        {
            Debug.LogError("No exist Enemy");
            return false;
        }
        enemies.Remove(enemy);
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyCacheSystem.Restore(enemy.FilePath, enemy.gameObject);

        return true;
    }

    public void Prepare()   //초기 단체 생성
    {
        for (int i = 0; i < enemyFiles.Length; i++)
        {
            GameObject go = enemyFactory.Load(enemyFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyCacheSystem.GenerateCache(enemyFiles[i].filePath, go, enemyFiles[i].cacheCount);
        }
    }
}
