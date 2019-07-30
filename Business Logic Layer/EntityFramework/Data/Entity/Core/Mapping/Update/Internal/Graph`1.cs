// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.Graph`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal class Graph<TVertex>
  {
    private readonly Dictionary<TVertex, HashSet<TVertex>> m_successorMap;
    private readonly Dictionary<TVertex, int> m_predecessorCounts;
    private readonly HashSet<TVertex> m_vertices;
    private readonly IEqualityComparer<TVertex> m_comparer;

    internal Graph(IEqualityComparer<TVertex> comparer)
    {
      this.m_comparer = comparer;
      this.m_successorMap = new Dictionary<TVertex, HashSet<TVertex>>(comparer);
      this.m_predecessorCounts = new Dictionary<TVertex, int>(comparer);
      this.m_vertices = new HashSet<TVertex>(comparer);
    }

    internal IEnumerable<TVertex> Vertices
    {
      get
      {
        return (IEnumerable<TVertex>) this.m_vertices;
      }
    }

    internal IEnumerable<KeyValuePair<TVertex, TVertex>> Edges
    {
      get
      {
        foreach (KeyValuePair<TVertex, HashSet<TVertex>> successor in this.m_successorMap)
        {
          foreach (TVertex vertex in successor.Value)
            yield return new KeyValuePair<TVertex, TVertex>(successor.Key, vertex);
        }
      }
    }

    internal void AddVertex(TVertex vertex)
    {
      this.m_vertices.Add(vertex);
    }

    internal void AddEdge(TVertex from, TVertex to)
    {
      if (!this.m_vertices.Contains(from) || !this.m_vertices.Contains(to))
        return;
      HashSet<TVertex> vertexSet;
      if (!this.m_successorMap.TryGetValue(from, out vertexSet))
      {
        vertexSet = new HashSet<TVertex>(this.m_comparer);
        this.m_successorMap.Add(from, vertexSet);
      }
      if (!vertexSet.Add(to))
        return;
      int num1;
      int num2 = this.m_predecessorCounts.TryGetValue(to, out num1) ? num1 + 1 : 1;
      this.m_predecessorCounts[to] = num2;
    }

    internal bool TryTopologicalSort(
      out IEnumerable<TVertex> orderedVertices,
      out IEnumerable<TVertex> remainder)
    {
      SortedSet<TVertex> sortedSet = new SortedSet<TVertex>((IComparer<TVertex>) Comparer<TVertex>.Default);
      foreach (TVertex vertex in this.m_vertices)
      {
        int num;
        if (!this.m_predecessorCounts.TryGetValue(vertex, out num) || num == 0)
          sortedSet.Add(vertex);
      }
      TVertex[] vertexArray = new TVertex[this.m_vertices.Count];
      int count = 0;
      while (0 < sortedSet.Count)
      {
        TVertex min = sortedSet.Min;
        sortedSet.Remove(min);
        HashSet<TVertex> vertexSet;
        if (this.m_successorMap.TryGetValue(min, out vertexSet))
        {
          foreach (TVertex index in vertexSet)
          {
            if (--this.m_predecessorCounts[index] == 0)
              sortedSet.Add(index);
          }
          this.m_successorMap.Remove(min);
        }
        vertexArray[count++] = min;
        this.m_vertices.Remove(min);
      }
      if (this.m_vertices.Count == 0)
      {
        orderedVertices = (IEnumerable<TVertex>) vertexArray;
        remainder = Enumerable.Empty<TVertex>();
        return true;
      }
      orderedVertices = ((IEnumerable<TVertex>) vertexArray).Take<TVertex>(count);
      remainder = (IEnumerable<TVertex>) this.m_vertices;
      return false;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<TVertex, HashSet<TVertex>> successor in this.m_successorMap)
      {
        bool flag = true;
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "[{0}] --> ", (object) successor.Key);
        foreach (TVertex vertex in successor.Value)
        {
          if (flag)
            flag = false;
          else
            stringBuilder.Append(", ");
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) vertex);
        }
        stringBuilder.Append("; ");
      }
      return stringBuilder.ToString();
    }
  }
}
