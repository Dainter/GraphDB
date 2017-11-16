namespace GraphDB.Contract
{
    public interface IIoStrategy//文件读写算法接口
    {
        string Path { get; set; }
        //Graph ReadFile(ref ErrorCode err);
        //void SaveFile(XmlDocument doc, ref ErrorCode err);
    }
}
