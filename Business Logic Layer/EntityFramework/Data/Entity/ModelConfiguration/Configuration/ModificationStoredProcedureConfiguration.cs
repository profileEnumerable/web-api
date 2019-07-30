// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ModificationStoredProcedureConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  internal class ModificationStoredProcedureConfiguration
  {
    private readonly Dictionary<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>> _parameterNames = new Dictionary<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>();
    private readonly Dictionary<PropertyInfo, string> _resultBindings = new Dictionary<PropertyInfo, string>();
    private string _name;
    private string _schema;
    private string _rowsAffectedParameter;
    private List<FunctionParameter> _configuredParameters;

    public ModificationStoredProcedureConfiguration()
    {
    }

    private ModificationStoredProcedureConfiguration(ModificationStoredProcedureConfiguration source)
    {
      this._name = source._name;
      this._schema = source._schema;
      this._rowsAffectedParameter = source._rowsAffectedParameter;
      source._parameterNames.Each<KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>>((Action<KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>>) (c => this._parameterNames.Add(c.Key, Tuple.Create<string, string>(c.Value.Item1, c.Value.Item2))));
      source._resultBindings.Each<KeyValuePair<PropertyInfo, string>>((Action<KeyValuePair<PropertyInfo, string>>) (r => this._resultBindings.Add(r.Key, r.Value)));
    }

    public virtual ModificationStoredProcedureConfiguration Clone()
    {
      return new ModificationStoredProcedureConfiguration(this);
    }

    public void HasName(string name)
    {
      DatabaseName databaseName = DatabaseName.Parse(name);
      this._name = databaseName.Name;
      this._schema = databaseName.Schema;
    }

    public void HasName(string name, string schema)
    {
      this._name = name;
      this._schema = schema;
    }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public string Schema
    {
      get
      {
        return this._schema;
      }
    }

    public void RowsAffectedParameter(string name)
    {
      this._rowsAffectedParameter = name;
    }

    public string RowsAffectedParameterName
    {
      get
      {
        return this._rowsAffectedParameter;
      }
    }

    public IEnumerable<Tuple<string, string>> ParameterNames
    {
      get
      {
        return (IEnumerable<Tuple<string, string>>) this._parameterNames.Values;
      }
    }

    public void ClearParameterNames()
    {
      this._parameterNames.Clear();
    }

    public Dictionary<PropertyInfo, string> ResultBindings
    {
      get
      {
        return this._resultBindings;
      }
    }

    public void Parameter(
      PropertyPath propertyPath,
      string parameterName,
      string originalValueParameterName = null,
      bool rightKey = false)
    {
      this._parameterNames[new ModificationStoredProcedureConfiguration.ParameterKey(propertyPath, rightKey)] = Tuple.Create<string, string>(parameterName, originalValueParameterName);
    }

    public void Result(PropertyPath propertyPath, string columnName)
    {
      this._resultBindings[propertyPath.Single<PropertyInfo>()] = columnName;
    }

    public virtual void Configure(
      ModificationFunctionMapping modificationStoredProcedureMapping,
      DbProviderManifest providerManifest)
    {
      this._configuredParameters = new List<FunctionParameter>();
      this.ConfigureName(modificationStoredProcedureMapping);
      this.ConfigureSchema(modificationStoredProcedureMapping);
      this.ConfigureRowsAffectedParameter(modificationStoredProcedureMapping, providerManifest);
      this.ConfigureParameters(modificationStoredProcedureMapping);
      this.ConfigureResultBindings(modificationStoredProcedureMapping);
    }

    private void ConfigureName(
      ModificationFunctionMapping modificationStoredProcedureMapping)
    {
      if (string.IsNullOrWhiteSpace(this._name))
        return;
      modificationStoredProcedureMapping.Function.StoreFunctionNameAttribute = this._name;
    }

    private void ConfigureSchema(
      ModificationFunctionMapping modificationStoredProcedureMapping)
    {
      if (string.IsNullOrWhiteSpace(this._schema))
        return;
      modificationStoredProcedureMapping.Function.Schema = this._schema;
    }

    private void ConfigureRowsAffectedParameter(
      ModificationFunctionMapping modificationStoredProcedureMapping,
      DbProviderManifest providerManifest)
    {
      if (string.IsNullOrWhiteSpace(this._rowsAffectedParameter))
        return;
      if (modificationStoredProcedureMapping.RowsAffectedParameter == null)
      {
        FunctionParameter functionParameter = new FunctionParameter("_RowsAffected_", providerManifest.GetStoreType(TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int32))), ParameterMode.Out);
        modificationStoredProcedureMapping.Function.AddParameter(functionParameter);
        modificationStoredProcedureMapping.RowsAffectedParameter = functionParameter;
      }
      modificationStoredProcedureMapping.RowsAffectedParameter.Name = this._rowsAffectedParameter;
      this._configuredParameters.Add(modificationStoredProcedureMapping.RowsAffectedParameter);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private void ConfigureParameters(
      ModificationFunctionMapping modificationStoredProcedureMapping)
    {
      foreach (KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>> parameterName in this._parameterNames)
      {
        PropertyPath propertyPath = parameterName.Key.PropertyPath;
        string str1 = parameterName.Value.Item1;
        string str2 = parameterName.Value.Item2;
        List<ModificationFunctionParameterBinding> list = modificationStoredProcedureMapping.ParameterBindings.Where<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (pb =>
        {
          if ((pb.MemberPath.AssociationSetEnd == null || pb.MemberPath.AssociationSetEnd.ParentAssociationSet.ElementType.IsManyToMany()) && propertyPath.Equals(new PropertyPath(pb.MemberPath.Members.OfType<EdmProperty>().Select<EdmProperty, PropertyInfo>((Func<EdmProperty, PropertyInfo>) (m => m.GetClrPropertyInfo())))))
            return true;
          if (propertyPath.Count == 2 && pb.MemberPath.AssociationSetEnd != null && pb.MemberPath.Members.First<EdmMember>().GetClrPropertyInfo().IsSameAs(propertyPath.Last<PropertyInfo>()))
            return pb.MemberPath.AssociationSetEnd.ParentAssociationSet.AssociationSetEnds.Select<AssociationSetEnd, PropertyInfo>((Func<AssociationSetEnd, PropertyInfo>) (ae => ae.CorrespondingAssociationEndMember.GetClrPropertyInfo())).Where<PropertyInfo>((Func<PropertyInfo, bool>) (pi => pi != (PropertyInfo) null)).Any<PropertyInfo>((Func<PropertyInfo, bool>) (pi => pi.IsSameAs(propertyPath.First<PropertyInfo>())));
          return false;
        })).ToList<ModificationFunctionParameterBinding>();
        if (list.Count == 1)
        {
          ModificationFunctionParameterBinding parameterBinding = list.Single<ModificationFunctionParameterBinding>();
          if (!string.IsNullOrWhiteSpace(str2) && parameterBinding.IsCurrent)
            throw Error.ModificationFunctionParameterNotFoundOriginal((object) propertyPath, (object) modificationStoredProcedureMapping.Function.FunctionName);
          parameterBinding.Parameter.Name = str1;
          this._configuredParameters.Add(parameterBinding.Parameter);
        }
        else
        {
          if (list.Count != 2)
            throw Error.ModificationFunctionParameterNotFound((object) propertyPath, (object) modificationStoredProcedureMapping.Function.FunctionName);
          ModificationFunctionParameterBinding parameterBinding1 = list.Select<ModificationFunctionParameterBinding, bool>((Func<ModificationFunctionParameterBinding, bool>) (pb => pb.IsCurrent)).Distinct<bool>().Count<bool>() != 1 || !list.All<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (pb => pb.MemberPath.AssociationSetEnd != null)) ? list.Single<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (pb => pb.IsCurrent)) : (!parameterName.Key.IsRightKey ? list.First<ModificationFunctionParameterBinding>() : list.Last<ModificationFunctionParameterBinding>());
          parameterBinding1.Parameter.Name = str1;
          this._configuredParameters.Add(parameterBinding1.Parameter);
          if (!string.IsNullOrWhiteSpace(str2))
          {
            ModificationFunctionParameterBinding parameterBinding2 = list.Single<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (pb => !pb.IsCurrent));
            parameterBinding2.Parameter.Name = str2;
            this._configuredParameters.Add(parameterBinding2.Parameter);
          }
        }
      }
      foreach (FunctionParameter functionParameter in modificationStoredProcedureMapping.Function.Parameters.Except<FunctionParameter>((IEnumerable<FunctionParameter>) this._configuredParameters))
        functionParameter.Name = ((IEnumerable<INamedDataModelItem>) modificationStoredProcedureMapping.Function.Parameters.Except<FunctionParameter>((IEnumerable<FunctionParameter>) new FunctionParameter[1]
        {
          functionParameter
        })).UniquifyName(functionParameter.Name);
    }

    private void ConfigureResultBindings(
      ModificationFunctionMapping modificationStoredProcedureMapping)
    {
      foreach (KeyValuePair<PropertyInfo, string> resultBinding in this._resultBindings)
      {
        PropertyInfo propertyInfo = resultBinding.Key;
        string str = resultBinding.Value;
        ModificationFunctionResultBinding functionResultBinding = ((IEnumerable<ModificationFunctionResultBinding>) modificationStoredProcedureMapping.ResultBindings ?? Enumerable.Empty<ModificationFunctionResultBinding>()).SingleOrDefault<ModificationFunctionResultBinding>((Func<ModificationFunctionResultBinding, bool>) (rb => propertyInfo.IsSameAs(rb.Property.GetClrPropertyInfo())));
        if (functionResultBinding == null)
          throw Error.ResultBindingNotFound((object) propertyInfo.Name, (object) modificationStoredProcedureMapping.Function.FunctionName);
        functionResultBinding.ColumnName = str;
      }
    }

    public bool IsCompatibleWith(ModificationStoredProcedureConfiguration other)
    {
      if (this._name != null && other._name != null && !string.Equals(this._name, other._name, StringComparison.OrdinalIgnoreCase) || this._schema != null && other._schema != null && !string.Equals(this._schema, other._schema, StringComparison.OrdinalIgnoreCase))
        return false;
      return !this._parameterNames.Join<KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>, KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>, ModificationStoredProcedureConfiguration.ParameterKey, bool>((IEnumerable<KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>>) other._parameterNames, (Func<KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>, ModificationStoredProcedureConfiguration.ParameterKey>) (kv1 => kv1.Key), (Func<KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>, ModificationStoredProcedureConfiguration.ParameterKey>) (kv2 => kv2.Key), (Func<KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>, KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>, bool>) ((kv1, kv2) => !object.Equals((object) kv1.Value, (object) kv2.Value))).Any<bool>((Func<bool, bool>) (j => j));
    }

    public void Merge(
      ModificationStoredProcedureConfiguration modificationStoredProcedureConfiguration,
      bool allowOverride)
    {
      if (allowOverride || string.IsNullOrWhiteSpace(this._name))
        this._name = modificationStoredProcedureConfiguration.Name ?? this._name;
      if (allowOverride || string.IsNullOrWhiteSpace(this._schema))
        this._schema = modificationStoredProcedureConfiguration.Schema ?? this._schema;
      if (allowOverride || string.IsNullOrWhiteSpace(this._rowsAffectedParameter))
        this._rowsAffectedParameter = modificationStoredProcedureConfiguration.RowsAffectedParameterName ?? this._rowsAffectedParameter;
      foreach (KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>> keyValuePair in modificationStoredProcedureConfiguration._parameterNames.Where<KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>>((Func<KeyValuePair<ModificationStoredProcedureConfiguration.ParameterKey, Tuple<string, string>>, bool>) (parameterName =>
      {
        if (!allowOverride)
          return !this._parameterNames.ContainsKey(parameterName.Key);
        return true;
      })))
        this._parameterNames[keyValuePair.Key] = keyValuePair.Value;
      foreach (KeyValuePair<PropertyInfo, string> keyValuePair in modificationStoredProcedureConfiguration.ResultBindings.Where<KeyValuePair<PropertyInfo, string>>((Func<KeyValuePair<PropertyInfo, string>, bool>) (resultBinding =>
      {
        if (!allowOverride)
          return !this._resultBindings.ContainsKey(resultBinding.Key);
        return true;
      })))
        this._resultBindings[keyValuePair.Key] = keyValuePair.Value;
    }

    private sealed class ParameterKey
    {
      private readonly PropertyPath _propertyPath;
      private readonly bool _rightKey;

      public ParameterKey(PropertyPath propertyPath, bool rightKey)
      {
        this._propertyPath = propertyPath;
        this._rightKey = rightKey;
      }

      public PropertyPath PropertyPath
      {
        get
        {
          return this._propertyPath;
        }
      }

      public bool IsRightKey
      {
        get
        {
          return this._rightKey;
        }
      }

      public override bool Equals(object obj)
      {
        if (object.ReferenceEquals((object) null, obj))
          return false;
        if (object.ReferenceEquals((object) this, obj))
          return true;
        ModificationStoredProcedureConfiguration.ParameterKey parameterKey = (ModificationStoredProcedureConfiguration.ParameterKey) obj;
        if (this._propertyPath.Equals(parameterKey._propertyPath))
          return this._rightKey.Equals(parameterKey._rightKey);
        return false;
      }

      public override int GetHashCode()
      {
        return this._propertyPath.GetHashCode() * 397 ^ this._rightKey.GetHashCode();
      }
    }
  }
}
