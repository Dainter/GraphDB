
using GraphDB.Contract;

namespace GraphDB.IO
{
    //XML�ļ���ʽʾ��
    /*
     * <Graph>
     *      <Nodes NodeNumber="2">
     *          <Node num="0">
     *              <Name>��</Name>
     *              <string>����</string>
     *              <Property>
     *                  <A>1</A>
     *                  <B>2</B>
     *              </Property>
     *          </Node>
     *          <Node num="1">
     *              <Name>����</Name>
     *              <string>����</string>
     *              <Property>
     *                  <A>1</A>
     *                  <C>3</C>
     *              </Property>
     *          </Node>
     *      </Nodes>
     *      <Edgs EdgeNumber="1">
     *          <Edge>
     *              <string>ͳ��</string>
     *              <Start>0</Start>
     *              <End>1</End>
     *          </Edge>
     *      </Edgs> 
     * </Graph>
     * */

    public class XMLStrategy:IIoStrategy//XML�ļ���д�㷨
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

       //XMLStrategy�㷨��ȡ����
        //public Graph ReadFile(ref ErrorCode err)
        //{
        //    FileStream stream = null;
        //    XmlDocument doc = new XmlDocument();
        //    Graph NewGraph;

        //    try
        //    {
        //        stream = new FileStream(myFilePath, FileMode.Open);
        //        doc.Load(stream);               //�����ļ�����xml�ĵ�
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
        //    //��������
        //    NewGraph = new Graph(doc, ref err);
        //    if (NewGraph == null)
        //    {
        //        return null;
        //    }
        //    return NewGraph;
        //}

        //XMLStrategy�㷨���溯��
        //public void SaveFile(XmlDocument doc, ref ErrorCode err)
        //{
        //    FileStream stream = null;
        //    try
        //    {
        //        stream = new FileStream(myFilePath, FileMode.Create);
        //        doc.Save(stream);               //����xml�ĵ�����
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
