using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;   //LayoutKind»ç¿ë À§ÇÔ

[System.Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]   //LayoutKind.Sequential = ¼øÂ÷ÀûÀ¸·Î, CharSet.Ansi =  ASCII
public struct EnemyStruct   //class ¾Æ´Ô@@ ¿¢¼¿ÆÄÀÏÀÇ ¼ø¼­¿Í µ¿ÀÏ
{
    public int index;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MarshalTableConstant.charBufferSize)]   //string Å¸ºù´À ÇØÁà¾ßµÅ.
    public string FilePath;
    public int MaxHP;
    public int Damage;
    public int CrashDamage;
    public int BulletSpeed;
    public int FireRemainCount;
    public int GamePoint;
}

public class EnemyTable : TableLoader<EnemyStruct>
{
    Dictionary<int, EnemyStruct> tableDatas = new Dictionary<int, EnemyStruct>();

    void Start()
    {
        Load();
    }

    protected override void AddData(EnemyStruct data)
    {
        base.AddData(data);

        Debug.Log("data.index = " + data.index);
        Debug.Log("data.FilePath = " + data.FilePath);
        Debug.Log("data.MaxHP = " + data.MaxHP);
        Debug.Log("data.Damage = " + data.Damage);
        Debug.Log("data.CrashDamage = " + data.CrashDamage);
        Debug.Log("data.BulletSpeed = " + data.BulletSpeed);
        Debug.Log("data.FireRemainCount = " + data.FireRemainCount);
        Debug.Log("data.GamePoint = " + data.GamePoint);

        tableDatas.Add(data.index, data);
    }

    public EnemyStruct GetEnemy(int index)
    {
        if (!tableDatas.ContainsKey(index))
        {
            Debug.LogError("GetEnemy Error! index = " + index);
            return default(EnemyStruct);
        }

        return tableDatas[index];
    }
}
