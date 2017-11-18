using System.Collections.Generic;

namespace GraphDB.Contract.Core
{
    public interface INode
    {
        string Guid { get; }
        string Name { get; }
        int InDegree { get; }
        int OutDegree { get; }
        List<IEdge> OutBound { get; }
        List<IEdge> InBound { get; }
        //增加连边
        bool AddEdge( IEdge newEdge );
        //Inbound边注册
        bool RegisterInbound( IEdge newEdge );
        //去除连边
        bool RemoveEdge( IEdge curEdge );
        //清除所有连边,返回被清除的边列表
        List<IEdge> ClearEdge();
        //Inbound注销
        bool UnRegisterInbound( IEdge curEdge );
    }
}