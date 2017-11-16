using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Utility;

namespace GraphDB.Core
{
    public class Edge: IEdge//ͼ���ݿ������ࣺ����洢����������Ϣ
    {
        //��Ա����
        private INode myFromNode;//�������
        private INode myToNode;//�����յ�
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
            }
        }
        public string Attribute => myAttribute;
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
            if (newType == "")
            {
                newType = "����";
            }
            var newValue = xNode.GetText("Value");
            if (newValue == "")
            {
                newValue = "1";
            }
            //��ֵ���ʼ��
            myAttribute = newType;
            myValue = newValue;
        }

        //���������ݱ���Ϊxml��ʽ
        public virtual XmlElement ToXML(ref XmlDocument doc)
        {
            XmlElement curEdge = doc.CreateElement("Edge");         //��������Ԫ��
            XmlElement typeXML, valueXML, startXML, endXML;
            XmlText typeTxt, valueTxt, startTxt, endTxt;

            //�ڵ�����
            typeXML = doc.CreateElement("Attribute");
            valueXML = doc.CreateElement("Value");
            //�ڵ�λ��
            startXML = doc.CreateElement("Start");
            endXML = doc.CreateElement("End");
            //���������Ե��ı�Ԫ��
            typeTxt = doc.CreateTextNode(Attribute);               
            valueTxt = doc.CreateTextNode(Value);
            startTxt = doc.CreateTextNode(From.Guid);
            endTxt = doc.CreateTextNode(To.Guid);
            //������Ԫ�ظ����ı�����
            typeXML.AppendChild(typeTxt);                                    
            valueXML.AppendChild(valueTxt);
            startXML.AppendChild(startTxt);
            endXML.AppendChild(endTxt);
            //��ǰ�ڵ��м�������Խڵ�
            curEdge.AppendChild(typeXML);                                   
            curEdge.AppendChild(valueXML);
            curEdge.AppendChild(startXML);
            curEdge.AppendChild(endXML);

            return curEdge;
        }
    }
}
