using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;   //marshal을 위함
using System.Reflection;                //marshal을 위함
using System.Text;

public class MarshalTableConstant   //string을 할당할때 사용할 변수
{
    public const int charBufferSize = 256;
}

public class TableRecordParser<TMarshalStruct>  //Tamplit형
{
    public TMarshalStruct ParseRecordLine(string line)  //record "한줄"을 읽어들일때 사용
    {
        //TMarshalStruct 크기에 맞춰서 Byte 배열 할당
        Type type = typeof(TMarshalStruct);
        int structSize = Marshal.SizeOf(type);      //System.Runtime.InteropServices.Marshal
        byte[] structBytes = new byte[structSize];  //여기에 저장
        int structBytesIndex = 0;

        //line 문자열을 spliter로 자른다.
        const string spliter = ",";
        string[] fieldDataList = line.Split(spliter.ToCharArray());
        
        //각 필드에 대한 정보
        Type dataType;
        string splited;
        byte[] fieldByte;
        byte[] keyBytes;

        FieldInfo[] fieldInfos = type.GetFields();      //System.Reflection.FieldInfo -> 필드에 대한 정보들이 배열로 반환
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            dataType = fieldInfos[i].FieldType;
            splited = fieldDataList[i];

            fieldByte = new byte[4];    //32비트 기준
            MakeBytesByFieldType(out fieldByte, dataType, splited);

            //명시적으로 보인다. fieldByte의 값을 structBytes에 누적
            //for (int index = 0; index < fieldByte.Length; index++)
            //{
            //    structBytes[structBytesIndex++] = fieldByte[index];
            //}

            Buffer.BlockCopy(fieldByte, 0, structBytes, structBytesIndex, fieldByte.Length);    //한줄에
            structBytesIndex += fieldByte.Length;

            //첫번째 필드를 key값으로 사용하기 위해 백업
            if (i == 0)
            {
                keyBytes = fieldByte;
            }
        }
        //marshaling
        TMarshalStruct tStruct = MakeStructFromBytes<TMarshalStruct>(structBytes);  
        //AddData(keyBytes, tStruct);
        return tStruct;
    }

    /// <summary>
    /// 문자열 splite를 주어진 dataType에 맞게 fieldByte 배열에 변환해서 반환
    /// </summary>
    /// <param name="fieldByte">결과 값을 받을 배열</param>
    /// <param name="dataType">splite를 변환할때 사용될 자료형</param>
    /// <param name="splite">반환할 값이 있는 문자열</param>
    protected void MakeBytesByFieldType(out byte[] fieldByte, Type dataType, string splite)
    {
        fieldByte = new byte[1];

        if (typeof(int) == dataType)        //System.BitConverter
        {
            fieldByte = BitConverter.GetBytes(int.Parse(splite));   //바이틀 변환
        }
        else if (typeof(float) == dataType)
        {
            fieldByte = BitConverter.GetBytes(float.Parse(splite));
        }
        else if (typeof(bool) == dataType)
        {
            bool value = bool.Parse(splite);
            int temp = value ? 1 : 0;

            fieldByte = BitConverter.GetBytes((int)temp);
        }
        else if (typeof(string) == dataType)
        {
            fieldByte = new byte[MarshalTableConstant.charBufferSize];  //마샬링을 하기 위해서 고정 크기 버퍼 생성
            byte[] byteArr = Encoding.UTF8.GetBytes(splite);            //System.Text.Encoding   
            //변환된 byte 배열을 고정크기 버퍼에 복사
            Buffer.BlockCopy(byteArr, 0, fieldByte, 0, byteArr.Length); //System.Buffer; -> 한공간에
        }
    }

    /// <summary>
    /// 마샬링을 통한 byte 배열의 T형 구조체 변환
    /// </summary>
    /// <typeparam name="T">마샬링에 적합하게 정의된 구조체의 타입</typeparam>
    /// <param name="bytes">마샬링할 데이터가 저장된 배열</param>
    /// <returns>변환된 T형 구조체</returns>
    public static T MakeStructFromBytes<T>(byte[] bytes)
    {
        int size = Marshal.SizeOf(typeof(T));
        IntPtr ptr = Marshal.AllocHGlobal(size);    //마샹 메모리 할당 (int 포인터)

        Marshal.Copy(bytes, 0, ptr, size);

        T tStruct = (T)Marshal.PtrToStructure(ptr, typeof(T));      //메모리로부터 T형 구조체로 변환
        Marshal.FreeHGlobal(ptr);                                   //할당된 메모리 해제
        return tStruct;                                             //변환된 값 반환
    }
}
