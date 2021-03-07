using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoCachableEffect : MonoBehaviour
{
    public string FilePath
    {
        get; set;
    }

    private void OnEnable()
    {
        StartCoroutine("CheckIfAlive");
    }

    IEnumerator CheckIfAlive()  
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!GetComponent<ParticleSystem>().IsAlive(true))  //�����ٸ� �����ϱ� ����
            {
                SystemManager.Instance.EffectManager.RemoveEffect(this);
                break;
            }
        }
    }
}
