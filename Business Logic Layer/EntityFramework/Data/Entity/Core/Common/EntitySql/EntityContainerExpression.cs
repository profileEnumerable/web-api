// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.EntityContainerExpression
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class EntityContainerExpression : ExpressionResolution
  {
    internal readonly EntityContainer EntityContainer;

    internal EntityContainerExpression(EntityContainer entityContainer)
      : base(ExpressionResolutionClass.EntityContainer)
    {
      this.EntityContainer = entityContainer;
    }

    internal override string ExpressionClassName
    {
      get
      {
        return EntityContainerExpression.EntityContainerClassName;
      }
    }

    internal static string EntityContainerClassName
    {
      get
      {
        return Strings.LocalizedEntityContainerExpression;
      }
    }
  }
}
