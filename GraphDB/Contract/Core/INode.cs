using System.Collections.Generic;
using System.Xml;

namespace GraphDB.Contract.Core
{
    public interface INode
    {
        string Guid { get; }
        string Name { get; }
        XmlElement Payload { get; set; }
        int InDegree { get; }
        int OutDegree { get; }
        List<IEdge> OutBound { get; }
        List<IEdge> InBound { get; }
    }
}