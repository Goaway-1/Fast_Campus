using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyGenerateData 
{
    public string FilePath;
    public int MaxHp;
    public int Damage;
    public int CrashDamage;
    public float BulletSpeed;
    public int FireRemainCount;
    public int GamePoint;

    public Vector3 GeneratePoint;
    public Vector3 AppearPoint;

    public Vector3 DisappearPoint;
}


public class Squadron : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
