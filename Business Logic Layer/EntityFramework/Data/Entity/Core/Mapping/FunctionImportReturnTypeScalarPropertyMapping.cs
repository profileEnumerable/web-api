// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportReturnTypeScalarPropertyMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Maps a function import return type property to a table column.
  /// </summary>
  public sealed class FunctionImportReturnTypeScalarPropertyMapping : FunctionImportReturnTypePropertyMapping
  {
    private readonly string _propertyName;
    private readonly string _columnName;

    /// <summary>
    /// Initializes a new FunctionImportReturnTypeScalarPropertyMapping instance.
    /// </summary>
    /// <param name="propertyName">The mapped property name.</param>
    /// <param name="columnName">The mapped column name.</param>
    public FunctionImportReturnTypeScalarPropertyMapping(string propertyName, string columnName)
      : this(Check.NotNull<string>(propertyName, nameof (propertyName)), Check.NotNull<string>(columnName, nameof (columnName)), LineInfo.Empty)
    {
    }

    internal FunctionImportReturnTypeScalarPropertyMapping(
      string propertyName,
      string columnName,
      LineInfo lineInfo)
      : base(lineInfo)
    {
      this._propertyName = propertyName;
      this._columnName = columnName;
    }

    /// <summary>Gets the mapped property name.</summary>
    public string PropertyName
    {
      get
      {
        return this._propertyName;
      }
    }

    internal override string CMember
    {
      get
      {
        return this.PropertyName;
      }
    }

    /// <summary>Gets the mapped column name.</summary>
    public string ColumnName
    {
      get
      {
        return this._columnName;
      }
    }

    internal override string SColumn
    {
      get
      {
        return this.ColumnName;
      }
    }
  }
}
