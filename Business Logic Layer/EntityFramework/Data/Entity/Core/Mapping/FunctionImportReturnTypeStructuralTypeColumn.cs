// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportReturnTypeStructuralTypeColumn
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Mapping
{
  internal sealed class FunctionImportReturnTypeStructuralTypeColumn
  {
    internal readonly StructuralType Type;
    internal readonly bool IsTypeOf;
    internal readonly string ColumnName;
    internal readonly LineInfo LineInfo;

    internal FunctionImportReturnTypeStructuralTypeColumn(
      string columnName,
      StructuralType type,
      bool isTypeOf,
      LineInfo lineInfo)
    {
      this.ColumnName = columnName;
      this.IsTypeOf = isTypeOf;
      this.Type = type;
      this.LineInfo = lineInfo;
    }
  }
}
