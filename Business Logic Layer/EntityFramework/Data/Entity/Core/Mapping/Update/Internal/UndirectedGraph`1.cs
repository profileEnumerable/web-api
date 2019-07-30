// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.UndirectedGraph`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Text;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal class UndirectedGraph<TVertex> : InternalBase
  {
    private readonly Graph<TVertex> m_graph;
    private readonly IEqualityComparer<TVertex> m_comparer;

    internal UndirectedGraph(IEqualityComparer<TVertex> comparer)
    {
      this.m_graph = new Graph<TVertex>(comparer);
      this.m_comparer = comparer;
    }

    internal IEnumerable<TVertex> Vertices
    {
      get
      {
        return this.m_graph.Vertices;
      }
    }

    internal IEnumerable<KeyValuePair<TVertex, TVertex>> Edges
    {
      get
      {
        return this.m_graph.Edges;
      }
    }

    internal void AddVertex(TVertex vertex)
    {
      this.m_graph.AddVertex(vertex);
    }

    internal void AddEdge(TVertex first, TVertex second)
    {
      this.m_graph.AddEdge(first, second);
      this.m_graph.AddEdge(second, first);
    }

    internal KeyToListMap<int, TVertex> GenerateConnectedComponents()
    {
      int compNum = 0;
      Dictionary<TVertex, UndirectedGraph<TVertex>.ComponentNum> dictionary = new Dictionary<TVertex, UndirectedGraph<TVertex>.ComponentNum>(this.m_comparer);
      foreach (TVertex vertex in this.Vertices)
      {
        dictionary.Add(vertex, new UndirectedGraph<TVertex>.ComponentNum(compNum));
        ++compNum;
      }
      foreach (KeyValuePair<TVertex, TVertex> edge in this.Edges)
      {
        if (dictionary[edge.Key].componentNum != dictionary[edge.Value].componentNum)
        {
          int componentNum1 = dictionary[edge.Value].componentNum;
          int componentNum2 = dictionary[edge.Key].componentNum;
          dictionary[edge.Value].componentNum = componentNum2;
          foreach (TVertex key in dictionary.Keys)
          {
            if (dictionary[key].componentNum == componentNum1)
              dictionary[key].componentNum = componentNum2;
          }
        }
      }
      KeyToListMap<int, TVertex> keyToListMap = new KeyToListMap<int, TVertex>((IEqualityComparer<int>) EqualityComparer<int>.Default);
      foreach (TVertex vertex in this.Vertices)
      {
        int componentNum = dictionary[vertex].componentNum;
        keyToListMap.Add(componentNum, vertex);
      }
      return keyToListMap;
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append((object) this.m_graph);
    }

    private class ComponentNum
    {
      internal int componentNum;

      internal ComponentNum(int compNum)
      {
        this.componentNum = compNum;
      }

      public override string ToString()
      {
        return StringUtil.FormatInvariant("{0}", (object) this.componentNum);
      }
    }
  }
}
