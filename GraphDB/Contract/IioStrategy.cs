using System.Xml;

using GraphDB.Utility;

namespace GraphDB.Contract
{
    public interface IIoStrategy//�ļ���д�㷨�ӿ�
    {
        string Path { get; set; }
        XmlElement ReadFile( out ErrorCode err );
        void SaveFile(XmlDocument doc, out ErrorCode err);

    }
}
