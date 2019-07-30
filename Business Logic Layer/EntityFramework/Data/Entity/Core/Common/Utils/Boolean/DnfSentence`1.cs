// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Utils.Boolean.DnfSentence`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.Utils.Boolean
{
  internal sealed class DnfSentence<T_Identifier> : Sentence<T_Identifier, DnfClause<T_Identifier>>
  {
    internal DnfSentence(Set<DnfClause<T_Identifier>> clauses)
      : base(clauses, ExprType.Or)
    {
    }
  }
}
