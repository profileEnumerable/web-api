// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportEntityTypeMappingCondition
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Represents a mapping condition for a function import result.
  /// </summary>
  public abstract class FunctionImportEntityTypeMappingCondition : MappingItem
  {
    private readonly string _columnName;
    internal readonly LineInfo LineInfo;

    internal FunctionImportEntityTypeMappingCondition(string columnName, LineInfo lineInfo)
    {
      this._columnName = columnName;
      this.LineInfo = lineInfo;
    }

    /// <summary>
    /// Gets the name of the column used to evaluate the condition.
    /// </summary>
    public string ColumnName
    {
      get
      {
        return this._columnName;
      }
    }

    internal abstract ValueCondition ConditionValue { get; }

    internal abstract bool ColumnValueMatchesCondition(object columnValue);

    /// <inheritdoc />
    public override string ToString()
    {
      return this.ConditionValue.ToString();
    }
  }
}
