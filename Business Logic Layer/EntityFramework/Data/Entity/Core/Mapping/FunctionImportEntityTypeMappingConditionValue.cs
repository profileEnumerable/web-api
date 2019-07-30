// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportEntityTypeMappingConditionValue
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Xml.XPath;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Represents a mapping condition for the result of a function import,
  /// evaluated by comparison with a specified value.
  /// </summary>
  public sealed class FunctionImportEntityTypeMappingConditionValue : FunctionImportEntityTypeMappingCondition
  {
    private readonly object _value;
    private readonly XPathNavigator _xPathValue;
    private readonly Memoizer<Type, object> _convertedValues;

    /// <summary>
    /// Initializes a new FunctionImportEntityTypeMappingConditionValue instance.
    /// </summary>
    /// <param name="columnName">The name of the column used to evaluate the condition.</param>
    /// <param name="value">The value to compare with.</param>
    public FunctionImportEntityTypeMappingConditionValue(string columnName, object value)
      : base(Check.NotNull<string>(columnName, nameof (columnName)), LineInfo.Empty)
    {
      Check.NotNull<object>(value, nameof (value));
      this._value = value;
      this._convertedValues = new Memoizer<Type, object>(new Func<Type, object>(this.GetConditionValue), (IEqualityComparer<Type>) null);
    }

    internal FunctionImportEntityTypeMappingConditionValue(
      string columnName,
      XPathNavigator columnValue,
      LineInfo lineInfo)
      : base(columnName, lineInfo)
    {
      this._xPathValue = columnValue;
      this._convertedValues = new Memoizer<Type, object>(new Func<Type, object>(this.GetConditionValue), (IEqualityComparer<Type>) null);
    }

    /// <summary>Gets the value used for comparison.</summary>
    public object Value
    {
      get
      {
        return this._value;
      }
    }

    internal override ValueCondition ConditionValue
    {
      get
      {
        return new ValueCondition(this._value != null ? this._value.ToString() : this._xPathValue.Value);
      }
    }

    internal override bool ColumnValueMatchesCondition(object columnValue)
    {
      if (columnValue == null || Convert.IsDBNull(columnValue))
        return false;
      object y = this._convertedValues.Evaluate(columnValue.GetType());
      return ByValueEqualityComparer.Default.Equals(columnValue, y);
    }

    private object GetConditionValue(Type columnValueType)
    {
      return this.GetConditionValue(columnValueType, (Action) (() =>
      {
        throw new EntityCommandExecutionException(Strings.Mapping_FunctionImport_UnsupportedType((object) this.ColumnName, (object) columnValueType.FullName));
      }), (Action) (() =>
      {
        throw new EntityCommandExecutionException(Strings.Mapping_FunctionImport_ConditionValueTypeMismatch((object) "FunctionImportMapping", (object) this.ColumnName, (object) columnValueType.FullName));
      }));
    }

    internal object GetConditionValue(
      Type columnValueType,
      Action handleTypeNotComparable,
      Action handleInvalidConditionValue)
    {
      PrimitiveType primitiveType;
      if (!ClrProviderManifest.Instance.TryGetPrimitiveType(columnValueType, out primitiveType) || !MappingItemLoader.IsTypeSupportedForCondition(primitiveType.PrimitiveTypeKind))
      {
        handleTypeNotComparable();
        return (object) null;
      }
      if (this._value != null)
      {
        if (this._value.GetType() == columnValueType)
          return this._value;
        handleInvalidConditionValue();
        return (object) null;
      }
      try
      {
        return this._xPathValue.ValueAs(columnValueType);
      }
      catch (FormatException ex)
      {
        handleInvalidConditionValue();
        return (object) null;
      }
    }
  }
}
