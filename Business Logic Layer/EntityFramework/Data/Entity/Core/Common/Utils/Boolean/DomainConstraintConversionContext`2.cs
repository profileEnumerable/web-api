// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.DomainConstraintConversionContext`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal sealed class DomainConstraintConversionContext<T_Variable, T_Element> : ConversionContext<DomainConstraint<T_Variable, T_Element>>
  {
    private readonly Dictionary<DomainVariable<T_Variable, T_Element>, int> _domainVariableToRobddVariableMap = new Dictionary<DomainVariable<T_Variable, T_Element>, int>();
    private Dictionary<int, DomainVariable<T_Variable, T_Element>> _inverseMap;

    internal override Vertex TranslateTermToVertex(
      TermExpr<DomainConstraint<T_Variable, T_Element>> term)
    {
      Set<T_Element> range = term.Identifier.Range;
      DomainVariable<T_Variable, T_Element> variable1 = term.Identifier.Variable;
      Set<T_Element> domain = variable1.Domain;
      if (range.All<T_Element>((Func<T_Element, bool>) (element => !domain.Contains(element))))
        return Vertex.Zero;
      if (domain.All<T_Element>((Func<T_Element, bool>) (element => range.Contains(element))))
        return Vertex.One;
      Vertex[] array = domain.Select<T_Element, Vertex>((Func<T_Element, Vertex>) (element =>
      {
        if (!range.Contains(element))
          return Vertex.Zero;
        return Vertex.One;
      })).ToArray<Vertex>();
      int variable2;
      if (!this._domainVariableToRobddVariableMap.TryGetValue(variable1, out variable2))
      {
        variable2 = this.Solver.CreateVariable();
        this._domainVariableToRobddVariableMap[variable1] = variable2;
      }
      return this.Solver.CreateLeafVertex(variable2, array);
    }

    internal override IEnumerable<LiteralVertexPair<DomainConstraint<T_Variable, T_Element>>> GetSuccessors(
      Vertex vertex)
    {
      this.InitializeInverseMap();
      DomainVariable<T_Variable, T_Element> domainVariable = this._inverseMap[vertex.Variable];
      T_Element[] domain = domainVariable.Domain.ToArray();
      Dictionary<Vertex, Set<T_Element>> vertexToRange = new Dictionary<Vertex, Set<T_Element>>();
      for (int index = 0; index < vertex.Children.Length; ++index)
      {
        Vertex child = vertex.Children[index];
        Set<T_Element> set;
        if (!vertexToRange.TryGetValue(child, out set))
        {
          set = new Set<T_Element>(domainVariable.Domain.Comparer);
          vertexToRange.Add(child, set);
        }
        set.Add(domain[index]);
      }
      foreach (KeyValuePair<Vertex, Set<T_Element>> keyValuePair in vertexToRange)
      {
        Vertex successorVertex = keyValuePair.Key;
        Set<T_Element> range = keyValuePair.Value;
        DomainConstraint<T_Variable, T_Element> constraint = new DomainConstraint<T_Variable, T_Element>(domainVariable, range.MakeReadOnly());
        Literal<DomainConstraint<T_Variable, T_Element>> literal = new Literal<DomainConstraint<T_Variable, T_Element>>(new TermExpr<DomainConstraint<T_Variable, T_Element>>(constraint), true);
        yield return new LiteralVertexPair<DomainConstraint<T_Variable, T_Element>>(successorVertex, literal);
      }
    }

    private void InitializeInverseMap()
    {
      if (this._inverseMap != null)
        return;
      this._inverseMap = this._domainVariableToRobddVariableMap.ToDictionary<KeyValuePair<DomainVariable<T_Variable, T_Element>, int>, int, DomainVariable<T_Variable, T_Element>>((Func<KeyValuePair<DomainVariable<T_Variable, T_Element>, int>, int>) (kvp => kvp.Value), (Func<KeyValuePair<DomainVariable<T_Variable, T_Element>, int>, DomainVariable<T_Variable, T_Element>>) (kvp => kvp.Key));
    }
  }
}
