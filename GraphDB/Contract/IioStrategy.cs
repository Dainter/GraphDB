using System.Xml;

using GraphDB.Utility;

namespace GraphDB.Contract
{
    public interface IIoStrategy//文件读写算法接口
    {
        string Path { get; set; }
        XmlElement ReadFile( out ErrorCode err );
        void SaveFile(XmlDocument doc, out ErrorCode err);

    }
}
