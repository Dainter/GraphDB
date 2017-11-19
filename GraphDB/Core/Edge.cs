using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Contract.Serial;
using GraphDB.Utility;

namespace GraphDB.Core
{
    public class Edge: IEdge//ͼ���ݿ������ࣺ����洢����������Ϣ
    {
        //��Ա����
        private INode myFromNode;//�������
        private INode myToNode;//�����յ�
        private string myFromGuid;//�������GUID
        private string myToGuid;//�����յ�GUID
        private readonly string myAttribute;//��������
        private string myValue;//����ȡֵ

        //����//////////////////////////
        public INode From
        {
            get
            {
                return myFromNode;
            }
            set
            {
                myFromNode = value;
                myFromGuid = value.Guid;
            }
        }
        public INode To
        {
            get
            {
                return myToNode;
            }
            set
            {
                myToNode = value;
                myToGuid = value.Guid;
            }
        }
        [Serializable]
        public string FromGuid
        {
            get
            {
                return myFromGuid;
            }
            set
            {
                myFromGuid = value;
            }
        }
        [Serializable]
        public string ToGuid
        {
            get
            {
                return myToGuid;
            }
            set
            {
                myToGuid = value;
            }
        }
        [Serializable]
        public string Attribute => myAttribute;
        [Serializable]
        public string Value
        {
            get
            {
                return myValue;
            }
            set
            {
                myValue = value;
            }
        }

        //����/////////////////////////
        //������Edge���캯��
        public Edge(string newAttribute, string newValue = "1")//���캯�� �������������и�ֵ
        {
            myAttribute = newAttribute;
            myValue = newValue;
        }
        //������Edge���캯��
        public Edge( XmlElement xNode)//���캯�� �������������и�ֵ
        {
            //ȡ���ƶ���ǩ��Inner Text
            var newType = xNode.GetText("Attribute");
            var newValue = xNode.GetText("Value");
            //��ֵ���ʼ��
            myAttribute = newType;
            myValue = newValue;
        }
    }
}
