// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.InlineFunctionGroup
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class InlineFunctionGroup : MetadataMember
  {
    internal readonly IList<InlineFunctionInfo> FunctionMetadata;

    internal InlineFunctionGroup(string name, IList<InlineFunctionInfo> functionMetadata)
      : base(MetadataMemberClass.InlineFunctionGroup, name)
    {
      this.FunctionMetadata = functionMetadata;
    }

    internal override string MetadataMemberClassName
    {
      get
      {
        return InlineFunctionGroup.InlineFunctionGroupClassName;
      }
    }

    internal static string InlineFunctionGroupClassName
    {
      get
      {
        return Strings.LocalizedInlineFunction;
      }
    }
  }
}
