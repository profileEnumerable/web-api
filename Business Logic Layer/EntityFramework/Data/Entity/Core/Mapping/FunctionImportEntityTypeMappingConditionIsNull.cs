// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportEntityTypeMappingConditionIsNull
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Represents a mapping condition for the result of a function import
  /// evaluated by checking null or not null.
  /// </summary>
  public sealed class FunctionImportEntityTypeMappingConditionIsNull : FunctionImportEntityTypeMappingCondition
  {
    private readonly bool _isNull;

    /// <summary>
    /// Initializes a new FunctionImportEntityTypeMappingConditionIsNull instance.
    /// </summary>
    /// <param name="columnName">The name of the column used to evaluate the condition.</param>
    /// <param name="isNull">Flag that indicates whether a null or not null check is performed.</param>
    public FunctionImportEntityTypeMappingConditionIsNull(string columnName, bool isNull)
      : this(Check.NotNull<string>(columnName, nameof (columnName)), isNull, LineInfo.Empty)
    {
    }

    internal FunctionImportEntityTypeMappingConditionIsNull(
      string columnName,
      bool isNull,
      LineInfo lineInfo)
      : base(columnName, lineInfo)
    {
      this._isNull = isNull;
    }

    /// <summary>
    /// Gets a flag that indicates whether a null or not null check is performed.
    /// </summary>
    public bool IsNull
    {
      get
      {
        return this._isNull;
      }
    }

    internal override ValueCondition ConditionValue
    {
      get
      {
        if (!this.IsNull)
          return ValueCondition.IsNotNull;
        return ValueCondition.IsNull;
      }
    }

    internal override bool ColumnValueMatchesCondition(object columnValue)
    {
      return (columnValue == null || Convert.IsDBNull(columnValue)) == this.IsNull;
    }
  }
}
