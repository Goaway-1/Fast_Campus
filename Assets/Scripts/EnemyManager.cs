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

    //ĳ�̰���
    [SerializeField]
    PrefabsCacheData[] enemyFiles;

    private void Start()
    {
        Prepare();
    }

    private void Update()
    {
    }

    public bool GenerateEnemy(EnemyGenerateData data)   //��������
    {
        GameObject go = SystemManager.Instance.EnemyCacheSystem.Archive(data.FilePath);  //������ ȣ��

        go.transform.position = data.GeneratePoint;

        Enemy enemy = go.GetComponent<Enemy>();
        enemy.FilePath = data.FilePath;
        enemy.Reset(data);

        enemies.Add(enemy);
        return true;
    }

    public bool RemoveEnemy(Enemy enemy)    //�����ض�
    {
        if (!enemies.Contains(enemy))   //Ű���� �ƴ϶� �� ������Ʈ�� ���ٸ�
        {
            Debug.LogError("No exist Enemy");
            return false;
        }
        enemies.Remove(enemy);
        SystemManager.Instance.EnemyCacheSystem.Restore(enemy.FilePath, enemy.gameObject);

        return true;
    }

    public void Prepare()   //�ʱ� ��ü ����
    {
        for (int i = 0; i < enemyFiles.Length; i++)
        {
            GameObject go = enemyFactory.Load(enemyFiles[i].filePath);
            SystemManager.Instance.EnemyCacheSystem.GenerateCache(enemyFiles[i].filePath, go, enemyFiles[i].cacheCount);
        }
    }
}
