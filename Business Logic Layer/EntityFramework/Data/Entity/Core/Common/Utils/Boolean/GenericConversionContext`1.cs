// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.GenericConversionContext`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal sealed class GenericConversionContext<T_Identifier> : ConversionContext<T_Identifier>
  {
    private readonly Dictionary<TermExpr<T_Identifier>, int> _variableMap = new Dictionary<TermExpr<T_Identifier>, int>();
    private Dictionary<int, TermExpr<T_Identifier>> _inverseVariableMap;

    internal override Vertex TranslateTermToVertex(TermExpr<T_Identifier> term)
    {
      int variable;
      if (!this._variableMap.TryGetValue(term, out variable))
      {
        variable = this.Solver.CreateVariable();
        this._variableMap.Add(term, variable);
      }
      return this.Solver.CreateLeafVertex(variable, Solver.BooleanVariableChildren);
    }

    internal override IEnumerable<LiteralVertexPair<T_Identifier>> GetSuccessors(
      Vertex vertex)
    {
      LiteralVertexPair<T_Identifier>[] literalVertexPairArray = new LiteralVertexPair<T_Identifier>[2];
      Vertex child1 = vertex.Children[0];
      Vertex child2 = vertex.Children[1];
      this.InitializeInverseVariableMap();
      Literal<T_Identifier> literal1 = new Literal<T_Identifier>(this._inverseVariableMap[vertex.Variable], true);
      literalVertexPairArray[0] = new LiteralVertexPair<T_Identifier>(child1, literal1);
      Literal<T_Identifier> literal2 = literal1.MakeNegated();
      literalVertexPairArray[1] = new LiteralVertexPair<T_Identifier>(child2, literal2);
      return (IEnumerable<LiteralVertexPair<T_Identifier>>) literalVertexPairArray;
    }

    private void InitializeInverseVariableMap()
    {
      if (this._inverseVariableMap != null)
        return;
      this._inverseVariableMap = this._variableMap.ToDictionary<KeyValuePair<TermExpr<T_Identifier>, int>, int, TermExpr<T_Identifier>>((Func<KeyValuePair<TermExpr<T_Identifier>, int>, int>) (kvp => kvp.Value), (Func<KeyValuePair<TermExpr<T_Identifier>, int>, TermExpr<T_Identifier>>) (kvp => kvp.Key));
    }
  }
}
