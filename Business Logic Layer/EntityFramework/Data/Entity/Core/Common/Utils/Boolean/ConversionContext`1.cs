// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.ConversionContext`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal abstract class ConversionContext<T_Identifier>
  {
    internal readonly Solver Solver = new Solver();

    internal abstract Vertex TranslateTermToVertex(TermExpr<T_Identifier> term);

    internal abstract IEnumerable<LiteralVertexPair<T_Identifier>> GetSuccessors(
      Vertex vertex);
  }
}
