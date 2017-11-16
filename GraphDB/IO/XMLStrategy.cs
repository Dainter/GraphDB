
using GraphDB.Contract;

namespace GraphDB.IO
{
    //XML文件格式示例
    /*
     * <Graph>
     *      <Nodes NodeNumber="2">
     *          <Node num="0">
     *              <Name>秦</Name>
     *              <string>国家</string>
     *              <Property>
     *                  <A>1</A>
     *                  <B>2</B>
     *              </Property>
     *          </Node>
     *          <Node num="1">
     *              <Name>关中</Name>
     *              <string>地区</string>
     *              <Property>
     *                  <A>1</A>
     *                  <C>3</C>
     *              </Property>
     *          </Node>
     *      </Nodes>
     *      <Edgs EdgeNumber="1">
     *          <Edge>
     *              <string>统治</string>
     *              <Start>0</Start>
     *              <End>1</End>
     *          </Edge>
     *      </Edgs> 
     * </Graph>
     * */

    public class XMLStrategy:IIoStrategy//XML文件读写算法
    {
        string myFilePath;

        public string Path
        {
            get
            {
                return myFilePath;
            }
            set
            {
                myFilePath = value;
            }
        }

        public XMLStrategy(string sPath)
        {
            myFilePath = sPath;
        }

       //XMLStrategy算法读取函数
        //public Graph ReadFile(ref ErrorCode err)
        //{
        //    FileStream stream = null;
        //    XmlDocument doc = new XmlDocument();
        //    Graph NewGraph;

        //    try
        //    {
        //        stream = new FileStream(myFilePath, FileMode.Open);
        //        doc.Load(stream);               //从流文件读入xml文档
        //        stream.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (stream != null)
        //        {
        //            ex.ToString();
        //        }
        //        err = ErrorCode.OpenFileFailed;
        //        return null;
        //    }
        //    //创建网络
        //    NewGraph = new Graph(doc, ref err);
        //    if (NewGraph == null)
        //    {
        //        return null;
        //    }
        //    return NewGraph;
        //}

        //XMLStrategy算法保存函数
        //public void SaveFile(XmlDocument doc, ref ErrorCode err)
        //{
        //    FileStream stream = null;
        //    try
        //    {
        //        stream = new FileStream(myFilePath, FileMode.Create);
        //        doc.Save(stream);               //保存xml文档到流
        //        stream.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (stream != null)
        //        {
        //            ex.ToString();
        //        }
        //        err = ErrorCode.SaveFileFailed;
        //    }
        //}
    }
}
