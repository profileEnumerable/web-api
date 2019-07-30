// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.EntitySql.MetadataFunctionGroup
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.Common.EntitySql
{
  internal sealed class MetadataFunctionGroup : MetadataMember
  {
    internal readonly IList<EdmFunction> FunctionMetadata;

    internal MetadataFunctionGroup(string name, IList<EdmFunction> functionMetadata)
      : base(MetadataMemberClass.FunctionGroup, name)
    {
      this.FunctionMetadata = functionMetadata;
    }

    internal override string MetadataMemberClassName
    {
      get
      {
        return MetadataFunctionGroup.FunctionGroupClassName;
      }
    }

    internal static string FunctionGroupClassName
    {
      get
      {
        return Strings.LocalizedFunction;
      }
    }
  }
}
