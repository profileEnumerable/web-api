// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.ValueExpression
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class ValueExpression : ExpressionResolution
  {
    internal readonly DbExpression Value;

    internal ValueExpression(DbExpression value)
      : base(ExpressionResolutionClass.Value)
    {
      this.Value = value;
    }

    internal override string ExpressionClassName
    {
      get
      {
        return ValueExpression.ValueClassName;
      }
    }

    internal static string ValueClassName
    {
      get
      {
        return Strings.LocalizedValueExpression;
      }
    }
  }
}
