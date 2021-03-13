using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;   //marshal�� ����
using System.Reflection;                //marshal�� ����
using System.Text;

public class MarshalTableConstant   //string�� �Ҵ��Ҷ� ����� ����
{
    public const int charBufferSize = 256;
}

public class TableRecordParser<TMarshalStruct>  //Tamplit��
{
    public TMarshalStruct ParseRecordLine(string line)  //record "����"�� �о���϶� ���
    {
        //TMarshalStruct ũ�⿡ ���缭 Byte �迭 �Ҵ�
        Type type = typeof(TMarshalStruct);
        int structSize = Marshal.SizeOf(type);      //System.Runtime.InteropServices.Marshal
        byte[] structBytes = new byte[structSize];  //���⿡ ����
        int structBytesIndex = 0;

        //line ���ڿ��� spliter�� �ڸ���.
        const string spliter = ",";
        string[] fieldDataList = line.Split(spliter.ToCharArray());
        
        //�� �ʵ忡 ���� ����
        Type dataType;
        string splited;
        byte[] fieldByte;
        byte[] keyBytes;

        FieldInfo[] fieldInfos = type.GetFields();      //System.Reflection.FieldInfo -> �ʵ忡 ���� �������� �迭�� ��ȯ
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            dataType = fieldInfos[i].FieldType;
            splited = fieldDataList[i];

            fieldByte = new byte[4];    //32��Ʈ ����
            MakeBytesByFieldType(out fieldByte, dataType, splited);

            //��������� ���δ�. fieldByte�� ���� structBytes�� ����
            //for (int index = 0; index < fieldByte.Length; index++)
            //{
            //    structBytes[structBytesIndex++] = fieldByte[index];
            //}

            Buffer.BlockCopy(fieldByte, 0, structBytes, structBytesIndex, fieldByte.Length);    //���ٿ�
            structBytesIndex += fieldByte.Length;

            //ù��° �ʵ带 key������ ����ϱ� ���� ���
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
    /// ���ڿ� splite�� �־��� dataType�� �°� fieldByte �迭�� ��ȯ�ؼ� ��ȯ
    /// </summary>
    /// <param name="fieldByte">��� ���� ���� �迭</param>
    /// <param name="dataType">splite�� ��ȯ�Ҷ� ���� �ڷ���</param>
    /// <param name="splite">��ȯ�� ���� �ִ� ���ڿ�</param>
    protected void MakeBytesByFieldType(out byte[] fieldByte, Type dataType, string splite)
    {
        fieldByte = new byte[1];

        if (typeof(int) == dataType)        //System.BitConverter
        {
            fieldByte = BitConverter.GetBytes(int.Parse(splite));   //����Ʋ ��ȯ
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
            fieldByte = new byte[MarshalTableConstant.charBufferSize];  //�������� �ϱ� ���ؼ� ���� ũ�� ���� ����
            byte[] byteArr = Encoding.UTF8.GetBytes(splite);            //System.Text.Encoding   
            //��ȯ�� byte �迭�� ����ũ�� ���ۿ� ����
            Buffer.BlockCopy(byteArr, 0, fieldByte, 0, byteArr.Length); //System.Buffer; -> �Ѱ�����
        }
    }

    /// <summary>
    /// �������� ���� byte �迭�� T�� ����ü ��ȯ
    /// </summary>
    /// <typeparam name="T">�������� �����ϰ� ���ǵ� ����ü�� Ÿ��</typeparam>
    /// <param name="bytes">�������� �����Ͱ� ����� �迭</param>
    /// <returns>��ȯ�� T�� ����ü</returns>
    public static T MakeStructFromBytes<T>(byte[] bytes)
    {
        int size = Marshal.SizeOf(typeof(T));
        IntPtr ptr = Marshal.AllocHGlobal(size);    //���� �޸� �Ҵ� (int ������)

        Marshal.Copy(bytes, 0, ptr, size);

        T tStruct = (T)Marshal.PtrToStructure(ptr, typeof(T));      //�޸𸮷κ��� T�� ����ü�� ��ȯ
        Marshal.FreeHGlobal(ptr);                                   //�Ҵ�� �޸� ����
        return tStruct;                                             //��ȯ�� �� ��ȯ
    }
}
