namespace GraphDB.Contract
{
    public interface IIoStrategy//�ļ���д�㷨�ӿ�
    {
        string Path { get; set; }
        //Graph ReadFile(ref ErrorCode err);
        //void SaveFile(XmlDocument doc, ref ErrorCode err);
    }
}
