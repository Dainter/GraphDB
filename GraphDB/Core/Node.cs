using System.Collections.Generic;
using System.Xml;


namespace GraphDB.Core
{
    public class Node//图数据库节点类：负责存储单一网络节点的信息，并向上层类提供功能接口函数
    {
        //成员变量
        int intNodeNum;                           //节点编号
        string nodeName;
        string nodeType;
        XmlElement xmlPayload;
        List<Edge> OutLink;       //连边 使用字典结构存放（目标节点号，连边对象）
        List<Edge> InLink;
        int intSaveIndex;
        //属性///////////////////////////////
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
        //方法///////////////////////////////
        //节点类Node构造函数
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
        //xml构造函数
        public Node(int intMaxNodeNum, XmlElement xNode)
        {
            string newType, newName;

            this.intNodeNum = intMaxNodeNum;
            //取出制定标签的Inner Text
            newType = GetText(xNode, "Type");
            newName = GetText(xNode, "Name");
            this.xmlPayload = GetNode(xNode, "Payload");
            if(this.xmlPayload == null)
            {
                    this.xmlPayload = new XmlDocument().CreateElement("Payload");
            }
            //赋值与初始化
            this.nodeType = newType;
            this.nodeName = newName;
            this.intSaveIndex = this.intNodeNum;
            OutLink = new List<Edge>();
            InLink = new List<Edge>();
        }
        //工具函数，从xml节点中读取某个标签的InnerText
        XmlElement GetNode(XmlElement curNode, string sLabel)
        {
            if (curNode == null)
            {
                return null;
            }
            //遍历子节点列表
            foreach (XmlElement xNode in curNode.ChildNodes)
            {
                if (xNode.Name == sLabel)
                {//查找和指定内容相同的标签，返回其Innner Text
                    return xNode;
                }
            }
            return null;
        }

        //工具函数，从xml节点中读取某个标签的InnerText
        string GetText(XmlElement curNode, string sLabel)
        {
            if (curNode == null)
            {
                return "";
            }
            //遍历子节点列表
            foreach (XmlElement xNode in curNode.ChildNodes)
            {
                if (xNode.Name == sLabel)
                {//查找和指定内容相同的标签，返回其Innner Text
                    return xNode.InnerText;
                }
            }
            return "";
        }
        //将节点数据保存为xml格式
        public virtual XmlElement ToXML(ref XmlDocument doc)
        {
            XmlElement curNode = doc.CreateElement("Node");
            XmlElement type_xml, name_xml;
            XmlText type_txt, name_txt;

            curNode.SetAttribute("num", this.SaveIndex.ToString());                   //创建各属性的Tag元素
            //节点类型
            name_xml = doc.CreateElement("Name");
            type_xml = doc.CreateElement("Type");
            //创建各属性的文本元素
            name_txt = doc.CreateTextNode(this.Name);
            type_txt = doc.CreateTextNode(this.Type);
            //将标题元素赋予文本内容
            name_xml.AppendChild(name_txt);
            type_xml.AppendChild(type_txt);
            //向当前节点中加入各属性节点
            curNode.AppendChild(name_xml);
            curNode.AppendChild(type_xml);
            curNode.AppendChild(doc.ImportNode(xmlPayload, true));
            return curNode;
        }

        //增加连边
        public bool AddEdge(Edge newEdge)
        {
            if (newEdge == null)
            {
                return false;
            }
            //检测条件：当前边的起始节点是本节点，且终止节点不是本节点
            if (newEdge.Start.Number != intNodeNum || newEdge.End.Number == intNodeNum)
            {
                return false;
            }
            //如果OutbOund已经包含该边
            if (OutBoundContainsEdge(newEdge) == true)
            {
                return false;
            }
            //向Links中加入新项目  
            OutLink.Add(newEdge);   
            return true;
        }
        //Inbound边注册
        public bool RegisterInbound(Edge newEdge)
        {
            if (newEdge == null)
            {
                return false;
            }
            //检测条件：当前边的起始节点不是本节点，且终止节点是本节点
            if (newEdge.End.Number != intNodeNum || newEdge.Start.Number == intNodeNum)
            {
                return false;
            }
            //如果Inbound包含该边则不注册
            if (InBoundContainsEdge(newEdge) == true)
            {
                return false;
            }
            //加入新边
            InLink.Add(newEdge);
            return true;
        }
        //去除连边
        public bool RemoveEdge(Edge curEdge)
        {
            if (curEdge == null)
            {
                return false;
            }
            //检测条件：当前边的起始节点是本节点，且终止节点不是本节点
            if (curEdge.Start.Number != intNodeNum || curEdge.End.Number == intNodeNum)
            {
                return false;
            }
            //如果OutbOund不包含该边则退出
            if (OutBoundContainsEdge(curEdge) == false)
            {
                return false;
            }
            OutLink.Remove(curEdge);
            return true;
        }
        //清除所有连边,返回被清除的边列表
        public List<Edge> ClearEdge()
        {
            List<Edge> EdgeList = new List<Edge>();
            //首先将OutBound中所有连边的终止节点中注销该边
            foreach (Edge edge in this.OutBound)
            {
                edge.End.UnRegisterInbound(edge);
                edge.Start = null;
                edge.End = null;
                //当前边加入返回结果列表
                EdgeList.Add(edge);
            }
            //从OutBound中清除所有边
            this.OutBound.Clear();
            //首先将InBound中所有连边的起始节点中去除该边
            foreach (Edge edge in this.InBound)
            {
                edge.Start.RemoveEdge(edge);
                edge.Start = null;
                edge.End = null;
                //当前边加入返回结果列表
                EdgeList.Add(edge);
            }
            //从InBound中清除所有边
            this.InBound.Clear();
            //返回本节点涉及的连边列表
            return EdgeList;
        }
        //Inbound注销
        public bool UnRegisterInbound(Edge curEdge)
        {
            if (curEdge == null)
            {
                return false;
            }
            //检测条件：当前边的起始节点不是本节点，且终止节点是本节点
            if (curEdge.End.Number != intNodeNum || curEdge.Start.Number == intNodeNum)//检测条件：当前节点与目标节点不相连，且目标节点不是当前节点
            {
                return false;
            }
            //如果Inbound不包含当前边则不注销
            if (InBoundContainsEdge(curEdge) == false)
            {
                return false;
            }
            InLink.Remove(curEdge);
            return true;

        }
        //返回OutBound是否包含和目标节点间的连边
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
        //返回InBound是否包含和目标节点间的连边
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

        //查找连边
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

        //查找连边
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
