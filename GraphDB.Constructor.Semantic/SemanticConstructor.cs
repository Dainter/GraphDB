using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using GraphDB.Constructor.Semantic.Model;
using GraphDB.Constructor.Semantic.Utility;
using GraphDB.Contract.Core;
using GraphDB.Contract.Enum;
using GraphDB.Core;

namespace GraphDB.Constructor.Semantic
{
    public class SemanticConstructor
    {
        private readonly Graph mySemanticGraph;

        public Graph Database => mySemanticGraph;

        public SemanticConstructor(string dbName)
        {
            mySemanticGraph = new Graph(dbName);
        }

        public void ImportData(string content)
        {
            var lines = SplitLine( content );
            foreach( var curItem in lines)
            {
                var chars = SplitChar( curItem );
                BuildNetwork(chars);
            }
            ErrorCode err;
            mySemanticGraph.SaveDataBase(out err);
            return;
        }

        private IEnumerable<string> SplitLine( string block )
        {
            //定义匹配模式字符串
            string pattern = @"[\u4e00-\u9fa5]+";
            //正则表达式初始化，载入匹配模式
            Regex regObj = new Regex(pattern);
            //正则表达式对目标进行匹配
            MatchCollection matches = regObj.Matches(block);
            List<string> lines = new List<string>();
            foreach( var curItem in matches)
            {
                lines.Add( curItem.ToString() );
            }
            return lines;
        }

        private IEnumerable<string> SplitChar(string line)
        {
            //定义匹配模式字符串
            string pattern = @"[\u4e00-\u9fa5]";
            //正则表达式初始化，载入匹配模式
            Regex regObj = new Regex(pattern);
            //正则表达式对目标进行匹配
            MatchCollection matches = regObj.Matches(line);
            List<string> chars = new List<string>();
            foreach (var curItem in matches)
            {
                chars.Add(curItem.ToString());
            }
            return chars;
        }

        private void BuildNetwork(IEnumerable<string> chars)
        {
            var items = chars as IList<string> ?? chars.ToList();
            if( !items.Any() )
            {
                return;
            }
            string lastHanzi = "";
            int index = 0;
            foreach( string curItem in items)
            {
                AddNewHanzi(curItem);
                if (index == 00 )
                {
                    lastHanzi = curItem;
                    index ++;
                    continue;
                }
                AddRelation(lastHanzi, curItem);
                lastHanzi = curItem;
                index++;
            }
            return;
        }

        private void AddNewHanzi( string newChar )
        {
            if( newChar == null )
            {
                return;
            }
            if( mySemanticGraph.ContainsNode( newChar ) )
            {
                return;
            }
            Hanzi newHanzi = new Hanzi( newChar );
            ErrorCode err;
            mySemanticGraph.AddNode( newHanzi, out err );
        }

        private void AddRelation(string lastHanzi, string curHanzi)
        {
            ErrorCode err;

            //Next
            IEdge next = mySemanticGraph.GetEdgeByType( lastHanzi, curHanzi, CommonStrings.Next );
            if( next == null )
            {
                next = new Next();
                mySemanticGraph.AddEdge(lastHanzi, curHanzi, next, out err);
            }
            else
            {
                ((Next) next).AddWeight();
            }
            //Previous
            IEdge previous = mySemanticGraph.GetEdgeByType(curHanzi, lastHanzi, CommonStrings.Previous);
            if (previous == null)
            {
                previous = new Previous();
                mySemanticGraph.AddEdge(curHanzi, lastHanzi, previous, out err);
            }
            else
            {
                ((Previous)previous).AddWeight();
            }
            return;
        }
    }
}