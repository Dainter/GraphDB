using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using GraphDB.Contract.Enum;
using GraphDB.Contract.Serial;
using GraphDB.Core;
using GraphDB.IO;

namespace GraphDB.Utility
{
    internal static class SerializableHelper
    {
        public static XmlElement Serialize(XmlDocument doc, object obj)
        {
            PropertyInfo[] pInfos = obj.GetType().GetProperties();

            XmlElement xmlNode = doc.CreateElement(GetBaseType(obj));
            xmlNode.SetAttribute( XmlNames.Class, obj.GetType().FullName);
            foreach (var curItem in pInfos)
            {
                if (!IsSerialableProperty(curItem))
                {
                    continue;
                }
                XmlElement tag = doc.CreateElement(curItem.Name);
                string txt = (string)curItem.GetValue(obj);
                XmlText value = doc.CreateTextNode(txt);
                tag.AppendChild(value);
                xmlNode.AppendChild(tag);
            }
            return xmlNode;
        }

        public static object Deserialize(XmlElement xmlNode)
        {
            object[] parameters = new object[1];
            parameters[0] = xmlNode;
            string className = xmlNode.GetAttribute(XmlNames.Class);
            Assembly asm = GetAssembly(className);
            return asm?.CreateInstance(className, true, BindingFlags.Default, null, parameters, null, null);
        }

        private static bool IsSerialableProperty(PropertyInfo pInfo)
        {
            if (pInfo.CustomAttributes.Any(x => x.AttributeType.Name == typeof(XmlSerializableAttribute).Name))
            {
                return true;
            }
            return false;
        }

        private static string GetBaseType(object obj)
        {
            if( obj is Node )
            {
                return XmlNames.Node;
            }
            if (obj is Edge)
            {
                return XmlNames.Edge;
            }
            return "";
        }

        private static Assembly GetAssembly( string typeName )
        {
            string path = Assembly.GetExecutingAssembly().Location;
            string asmName = Path.GetFileName( path );
            List<string> assemList = GetAssemblyList();
            foreach ( string curItem in assemList)
            {
                Assembly asm = Assembly.LoadFile(path.Replace( asmName, curItem ));
                if( asm.ExportedTypes.Any( x => x.FullName == typeName ) )
                {
                    return asm;
                }
            }
            throw new FileLoadException("No valid assembly has been found.");
        }

        private static List<string> GetAssemblyList()
        {
            ErrorCode err;
            IIoStrategy xmlReader = new XMLStrategy( Properties.Settings.Default.GraphDBConfigPath );
            XmlElement root = xmlReader.ReadFile( out err );

            List<string> assemList = new List<string>();
            XmlElement setting = root.GetNode( "SerialAssemblyList" );
            foreach( XmlNode curItem in setting.ChildNodes)
            {
                assemList.Add( curItem.InnerText );
            }
            return assemList;
        }
    }
}