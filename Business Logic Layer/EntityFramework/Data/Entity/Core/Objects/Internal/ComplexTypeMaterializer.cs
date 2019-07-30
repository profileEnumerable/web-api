// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.ComplexTypeMaterializer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class ComplexTypeMaterializer
  {
    private const int MaxPlanCount = 4;
    private readonly MetadataWorkspace _workspace;
    private ComplexTypeMaterializer.Plan[] _lastPlans;
    private int _lastPlanIndex;

    internal ComplexTypeMaterializer(MetadataWorkspace workspace)
    {
      this._workspace = workspace;
    }

    internal object CreateComplex(
      IExtendedDataRecord record,
      DataRecordInfo recordInfo,
      object result)
    {
      ComplexTypeMaterializer.Plan plan = this.GetPlan(recordInfo);
      if (result == null)
        result = plan.ClrType();
      this.SetProperties(record, result, plan.Properties);
      return result;
    }

    private void SetProperties(
      IExtendedDataRecord record,
      object result,
      ComplexTypeMaterializer.PlanEdmProperty[] properties)
    {
      for (int index = 0; index < properties.Length; ++index)
      {
        if (properties[index].GetExistingComplex != null)
        {
          object existing = properties[index].GetExistingComplex(result);
          object complexRecursive = this.CreateComplexRecursive(record.GetValue(properties[index].Ordinal), existing);
          if (existing == null)
            properties[index].ClrProperty(result, complexRecursive);
        }
        else
          properties[index].ClrProperty(result, ComplexTypeMaterializer.ConvertDBNull(record.GetValue(properties[index].Ordinal)));
      }
    }

    private static object ConvertDBNull(object value)
    {
      if (DBNull.Value == value)
        return (object) null;
      return value;
    }

    private object CreateComplexRecursive(object record, object existing)
    {
      if (DBNull.Value == record)
        return existing;
      return this.CreateComplexRecursive((IExtendedDataRecord) record, existing);
    }

    private object CreateComplexRecursive(IExtendedDataRecord record, object existing)
    {
      return this.CreateComplex(record, record.DataRecordInfo, existing);
    }

    private ComplexTypeMaterializer.Plan GetPlan(DataRecordInfo recordInfo)
    {
      ComplexTypeMaterializer.Plan[] planArray = this._lastPlans ?? (this._lastPlans = new ComplexTypeMaterializer.Plan[4]);
      int index1 = this._lastPlanIndex - 1;
      for (int index2 = 0; index2 < 4; ++index2)
      {
        index1 = (index1 + 1) % 4;
        if (planArray[index1] != null)
        {
          if (planArray[index1].Key == recordInfo.RecordType)
          {
            this._lastPlanIndex = index1;
            return planArray[index1];
          }
        }
        else
          break;
      }
      ObjectTypeMapping objectMapping = Util.GetObjectMapping(recordInfo.RecordType.EdmType, this._workspace);
      this._lastPlanIndex = index1;
      planArray[index1] = new ComplexTypeMaterializer.Plan(recordInfo.RecordType, objectMapping, recordInfo.FieldMetadata);
      return planArray[index1];
    }

    private sealed class Plan
    {
      internal readonly TypeUsage Key;
      internal readonly Func<object> ClrType;
      internal readonly ComplexTypeMaterializer.PlanEdmProperty[] Properties;

      internal Plan(
        TypeUsage key,
        ObjectTypeMapping mapping,
        ReadOnlyCollection<FieldMetadata> fields)
      {
        this.Key = key;
        this.ClrType = DelegateFactory.GetConstructorDelegateForType((ClrComplexType) mapping.ClrType);
        this.Properties = new ComplexTypeMaterializer.PlanEdmProperty[fields.Count];
        for (int index = 0; index < this.Properties.Length; ++index)
        {
          FieldMetadata field = fields[index];
          int ordinal = field.Ordinal;
          this.Properties[index] = new ComplexTypeMaterializer.PlanEdmProperty(ordinal, mapping.GetPropertyMap(field.FieldType.Name).ClrProperty);
        }
      }
    }

    private struct PlanEdmProperty
    {
      internal readonly int Ordinal;
      internal readonly Func<object, object> GetExistingComplex;
      internal readonly Action<object, object> ClrProperty;

      internal PlanEdmProperty(int ordinal, EdmProperty property)
      {
        this.Ordinal = ordinal;
        this.GetExistingComplex = Helper.IsComplexType(property.TypeUsage.EdmType) ? DelegateFactory.GetGetterDelegateForProperty(property) : (Func<object, object>) null;
        this.ClrProperty = DelegateFactory.GetSetterDelegateForProperty(property);
      }
    }
  }
}
