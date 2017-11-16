using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Utility;

namespace GraphDB.Core
{
    public class Edge: IEdge//图数据库连边类：负责存储网络连边信息
    {
        //成员变量
        private INode myFromNode;//连边起点
        private INode myToNode;//连边终点
        private readonly string myAttribute;//连边类型
        private string myValue;//连边取值

        //属性//////////////////////////
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

        //方法/////////////////////////
        //连边类Edge构造函数
        public Edge(string newAttribute, string newValue = "1")//构造函数 对三个变量进行赋值
        {
            myAttribute = newAttribute;
            myValue = newValue;
        }
        //连边类Edge构造函数
        public Edge( XmlElement xNode)//构造函数 对三个变量进行赋值
        {
            //取出制定标签的Inner Text
            var newType = xNode.GetText("Attribute");
            if (newType == "")
            {
                newType = "关联";
            }
            var newValue = xNode.GetText("Value");
            if (newValue == "")
            {
                newValue = "1";
            }
            //赋值与初始化
            myAttribute = newType;
            myValue = newValue;
        }

        //将连边数据保存为xml格式
        public virtual XmlElement ToXML(ref XmlDocument doc)
        {
            XmlElement curEdge = doc.CreateElement("Edge");         //创建连边元素
            XmlElement typeXML, valueXML, startXML, endXML;
            XmlText typeTxt, valueTxt, startTxt, endTxt;

            //节点类型
            typeXML = doc.CreateElement("Attribute");
            valueXML = doc.CreateElement("Value");
            //节点位置
            startXML = doc.CreateElement("Start");
            endXML = doc.CreateElement("End");
            //创建各属性的文本元素
            typeTxt = doc.CreateTextNode(Attribute);               
            valueTxt = doc.CreateTextNode(Value);
            startTxt = doc.CreateTextNode(From.Guid);
            endTxt = doc.CreateTextNode(To.Guid);
            //将标题元素赋予文本内容
            typeXML.AppendChild(typeTxt);                                    
            valueXML.AppendChild(valueTxt);
            startXML.AppendChild(startTxt);
            endXML.AppendChild(endTxt);
            //向当前节点中加入各属性节点
            curEdge.AppendChild(typeXML);                                   
            curEdge.AppendChild(valueXML);
            curEdge.AppendChild(startXML);
            curEdge.AppendChild(endXML);

            return curEdge;
        }
    }
}
