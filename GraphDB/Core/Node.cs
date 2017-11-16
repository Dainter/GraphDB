using System.Collections.Generic;
using System.Xml;


namespace GraphDB.Core
{
    public class Node//ͼ���ݿ�ڵ��ࣺ����洢��һ����ڵ����Ϣ�������ϲ����ṩ���ܽӿں���
    {
        //��Ա����
        int intNodeNum;                           //�ڵ���
        string nodeName;
        string nodeType;
        XmlElement xmlPayload;
        List<Edge> OutLink;       //���� ʹ���ֵ�ṹ��ţ�Ŀ��ڵ�ţ����߶���
        List<Edge> InLink;
        int intSaveIndex;
        //����///////////////////////////////
        public int Number
        {
            get
            {
                return intNodeNum;
            }
        }
        public string Name
        {
            get
            {
                return nodeName;
            }
        }
        public string Type
        {
            get
            {
                return nodeType;
            }
        }
        public XmlElement Payload
        {
            get
            {
                return xmlPayload;
            }
            set
            {
                xmlPayload = value;
            }
        }

        public int InDegree
        {
            get
            {
                return InLink.Count;
            }
        }
        public int OutDegree
        {
            get
            {
                return OutLink.Count;
            }
        }
        public List<Edge> OutBound
        {
            get
            {
                return OutLink;
            }
        }
        public List<Edge> InBound
        {
            get
            {
                return InLink;
            }
        }
        public int SaveIndex
        {
            get
            {
                return intSaveIndex;
            }
            set
            {
                intSaveIndex = value;
            }
        }
        //����///////////////////////////////
        //�ڵ���Node���캯��
        public Node(int intMaxNodeNum, string newName, string newType, XmlElement payload)    
        {
            XmlDocument doc = new XmlDocument();
            this.intNodeNum = intMaxNodeNum;
            this.nodeName = newName;
            this.nodeType = newType;
            this.intSaveIndex = this.intNodeNum;
            if (payload != null)
            {
                this.xmlPayload = (XmlElement)doc.ImportNode(payload, true);
            }
            else
            {
                this.xmlPayload = doc.CreateElement("Payload");
            }
            OutLink = new List<Edge>();
            InLink = new List<Edge>();
            intMaxNodeNum++;
        }

        public Node(int intMaxNodeNum, Node oriNode)
        {
            this.intNodeNum = intMaxNodeNum;
            this.nodeName = string.Copy(oriNode.Name);
            this.nodeType = string.Copy(oriNode.Type);
            this.intSaveIndex = this.intNodeNum;
            if(oriNode.Payload != null)
            {
                this.Payload = (XmlElement)oriNode.Payload.CloneNode(true);
            }
            else
            {
                this.xmlPayload = new XmlDocument().CreateElement("Payload");
            }
            OutLink = new List<Edge>();
            InLink = new List<Edge>();
        }
        //xml���캯��
        public Node(int intMaxNodeNum, XmlElement xNode)
        {
            string newType, newName;

            this.intNodeNum = intMaxNodeNum;
            //ȡ���ƶ���ǩ��Inner Text
            newType = GetText(xNode, "Type");
            newName = GetText(xNode, "Name");
            this.xmlPayload = GetNode(xNode, "Payload");
            if(this.xmlPayload == null)
            {
                    this.xmlPayload = new XmlDocument().CreateElement("Payload");
            }
            //��ֵ���ʼ��
            this.nodeType = newType;
            this.nodeName = newName;
            this.intSaveIndex = this.intNodeNum;
            OutLink = new List<Edge>();
            InLink = new List<Edge>();
        }
        //���ߺ�������xml�ڵ��ж�ȡĳ����ǩ��InnerText
        XmlElement GetNode(XmlElement curNode, string sLabel)
        {
            if (curNode == null)
            {
                return null;
            }
            //�����ӽڵ��б�
            foreach (XmlElement xNode in curNode.ChildNodes)
            {
                if (xNode.Name == sLabel)
                {//���Һ�ָ��������ͬ�ı�ǩ��������Innner Text
                    return xNode;
                }
            }
            return null;
        }

        //���ߺ�������xml�ڵ��ж�ȡĳ����ǩ��InnerText
        string GetText(XmlElement curNode, string sLabel)
        {
            if (curNode == null)
            {
                return "";
            }
            //�����ӽڵ��б�
            foreach (XmlElement xNode in curNode.ChildNodes)
            {
                if (xNode.Name == sLabel)
                {//���Һ�ָ��������ͬ�ı�ǩ��������Innner Text
                    return xNode.InnerText;
                }
            }
            return "";
        }
        //���ڵ����ݱ���Ϊxml��ʽ
        public virtual XmlElement ToXML(ref XmlDocument doc)
        {
            XmlElement curNode = doc.CreateElement("Node");
            XmlElement type_xml, name_xml;
            XmlText type_txt, name_txt;

            curNode.SetAttribute("num", this.SaveIndex.ToString());                   //���������Ե�TagԪ��
            //�ڵ�����
            name_xml = doc.CreateElement("Name");
            type_xml = doc.CreateElement("Type");
            //���������Ե��ı�Ԫ��
            name_txt = doc.CreateTextNode(this.Name);
            type_txt = doc.CreateTextNode(this.Type);
            //������Ԫ�ظ����ı�����
            name_xml.AppendChild(name_txt);
            type_xml.AppendChild(type_txt);
            //��ǰ�ڵ��м�������Խڵ�
            curNode.AppendChild(name_xml);
            curNode.AppendChild(type_xml);
            curNode.AppendChild(doc.ImportNode(xmlPayload, true));
            return curNode;
        }

        //��������
        public bool AddEdge(Edge newEdge)
        {
            if (newEdge == null)
            {
                return false;
            }
            //�����������ǰ�ߵ���ʼ�ڵ��Ǳ��ڵ㣬����ֹ�ڵ㲻�Ǳ��ڵ�
            if (newEdge.Start.Number != intNodeNum || newEdge.End.Number == intNodeNum)
            {
                return false;
            }
            //���OutbOund�Ѿ������ñ�
            if (OutBoundContainsEdge(newEdge) == true)
            {
                return false;
            }
            //��Links�м�������Ŀ  
            OutLink.Add(newEdge);   
            return true;
        }
        //Inbound��ע��
        public bool RegisterInbound(Edge newEdge)
        {
            if (newEdge == null)
            {
                return false;
            }
            //�����������ǰ�ߵ���ʼ�ڵ㲻�Ǳ��ڵ㣬����ֹ�ڵ��Ǳ��ڵ�
            if (newEdge.End.Number != intNodeNum || newEdge.Start.Number == intNodeNum)
            {
                return false;
            }
            //���Inbound�����ñ���ע��
            if (InBoundContainsEdge(newEdge) == true)
            {
                return false;
            }
            //�����±�
            InLink.Add(newEdge);
            return true;
        }
        //ȥ������
        public bool RemoveEdge(Edge curEdge)
        {
            if (curEdge == null)
            {
                return false;
            }
            //�����������ǰ�ߵ���ʼ�ڵ��Ǳ��ڵ㣬����ֹ�ڵ㲻�Ǳ��ڵ�
            if (curEdge.Start.Number != intNodeNum || curEdge.End.Number == intNodeNum)
            {
                return false;
            }
            //���OutbOund�������ñ����˳�
            if (OutBoundContainsEdge(curEdge) == false)
            {
                return false;
            }
            OutLink.Remove(curEdge);
            return true;
        }
        //�����������,���ر�����ı��б�
        public List<Edge> ClearEdge()
        {
            List<Edge> EdgeList = new List<Edge>();
            //���Ƚ�OutBound���������ߵ���ֹ�ڵ���ע���ñ�
            foreach (Edge edge in this.OutBound)
            {
                edge.End.UnRegisterInbound(edge);
                edge.Start = null;
                edge.End = null;
                //��ǰ�߼��뷵�ؽ���б�
                EdgeList.Add(edge);
            }
            //��OutBound��������б�
            this.OutBound.Clear();
            //���Ƚ�InBound���������ߵ���ʼ�ڵ���ȥ���ñ�
            foreach (Edge edge in this.InBound)
            {
                edge.Start.RemoveEdge(edge);
                edge.Start = null;
                edge.End = null;
                //��ǰ�߼��뷵�ؽ���б�
                EdgeList.Add(edge);
            }
            //��InBound��������б�
            this.InBound.Clear();
            //���ر��ڵ��漰�������б�
            return EdgeList;
        }
        //Inboundע��
        public bool UnRegisterInbound(Edge curEdge)
        {
            if (curEdge == null)
            {
                return false;
            }
            //�����������ǰ�ߵ���ʼ�ڵ㲻�Ǳ��ڵ㣬����ֹ�ڵ��Ǳ��ڵ�
            if (curEdge.End.Number != intNodeNum || curEdge.Start.Number == intNodeNum)//�����������ǰ�ڵ���Ŀ��ڵ㲻��������Ŀ��ڵ㲻�ǵ�ǰ�ڵ�
            {
                return false;
            }
            //���Inbound��������ǰ����ע��
            if (InBoundContainsEdge(curEdge) == false)
            {
                return false;
            }
            InLink.Remove(curEdge);
            return true;

        }
        //����OutBound�Ƿ������Ŀ��ڵ�������
        bool OutBoundContainsEdge(Edge newEdge)
        {
            if (OutLink.Contains(newEdge))
            {
                return true;
            }
            foreach (Edge edge in OutLink)
            {
                if (edge.End.Number == newEdge.End.Number)
                {
                    if (edge.Attribute == newEdge.Attribute)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //����InBound�Ƿ������Ŀ��ڵ�������
        bool InBoundContainsEdge(Edge newEdge)
        {
            if (InLink.Contains(newEdge))
            {
                return true;
            }
            foreach (Edge edge in InLink)
            {
                if (edge.Start.Number == newEdge.Start.Number)
                {
                    if (edge.Attribute == newEdge.Attribute)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string FieldOutputAll()
        {
            string strResult = "";

            strResult += "Name\t";
            strResult += "Type\t";
            return strResult + "\n";
        }

        public string FieldOutput(List<string> labels)
        {
            string strResult = "";

            foreach (string label in labels)
            {
                if (label == "Name")
                {
                    strResult += "Name\t";
                }
                else if (label == "Type")
                {
                    strResult += "Type\t";
                }
            }
            return strResult + "\n";
        }

        public string DataOutputAll()
        {
            string strResult = "";

            strResult += this.Name+"\t";
            strResult += this.Type + "\t";
            return strResult + "\n";
        }

        public string DataOutput(List<string> labels)
        {
            string strResult = "";

            foreach (string label in labels)
            {
                if (label == "Name")
                {
                    strResult += this.Name + "\t";
                }
                else if (label == "Type")
                {
                    strResult += this.Type + "\t";
                }
            }
            return strResult + "\n";
        }

        public string DataOutput()
        {
            string strResult = "";

            strResult +="Name:" + this.Name + "\n";
            strResult +="Type:" + this.Type ;
            
            return strResult;
        }

        //��������
        public Edge GetEdge(string sName, string sType, string opt)
        {
            if (opt == "In")
            {
                foreach (Edge edge in InBound)
                {
                    if (edge.Start.Name == sName && edge.Start.Type == sType)
                    {
                        return edge;
                    }
                }
                return null;
            }
            else if (opt == "Out")
            {
                foreach (Edge edge in OutBound)
                {
                    if (edge.End.Name == sName && edge.End.Type == sType)
                    {
                        return edge;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        //��������
        public Edge GetEdge(string sType, string opt)
        {
            if (opt == "In")
            {
                foreach (Edge edge in InBound)
                {
                    if (edge.Attribute == sType)
                    {
                        return edge;
                    }
                }
                return null;
            }
            else if (opt == "Out")
            {
                foreach (Edge edge in OutBound)
                {
                    if (edge.Attribute == sType)
                    {
                        return edge;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public List<Edge> GetEdges(string sType, string opt)
        {
            List<Edge> res = new List<Edge>();
            if (opt == "In")
            {
                foreach (Edge edge in InBound)
                {
                    if (edge.Attribute == sType)
                    {
                        res.Add(edge);
                    }
                }
            }
            else if (opt == "Out")
            {
                foreach (Edge edge in OutBound)
                {
                    if (edge.Attribute == sType)
                    {
                        res.Add(edge);
                    }
                }
            }
            return res;
        }

    }
}
