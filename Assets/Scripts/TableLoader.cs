using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TableLoader<TMarshalStruct> : MonoBehaviour
{
    [SerializeField]
    protected string FilePath;

    TableRecordParser<TMarshalStruct> tableRecordParser = new TableRecordParser<TMarshalStruct>();

    public bool Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(FilePath);  //textasset는 Text의 전체 String이 들어가 있다.
        if (textAsset == null)
        {
            Debug.LogError("Load Faild! filePath = " + FilePath);
            return false;
        }

        ParseTable(textAsset.text);

        return true;
    }

    void ParseTable(string text)
    {
        StringReader reader = new StringReader(text);   //System.IO.StringReader

        string line = null;
        bool fieldRead = false;
        while ((line = reader.ReadLine()) != null)      //파일 끝날 때까지 계속 레코드 파싱
        {
            if (!fieldRead) //한줄은 점프
            {
                fieldRead = true;
                continue;
            }

            TMarshalStruct data = tableRecordParser.ParseRecordLine(line);
            AddData(data);
        }
    }
    protected virtual void AddData(TMarshalStruct data)
    {

    }
}
