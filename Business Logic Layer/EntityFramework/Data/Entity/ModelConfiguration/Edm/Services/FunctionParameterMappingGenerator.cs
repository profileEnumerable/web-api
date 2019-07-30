// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.Services.FunctionParameterMappingGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Mapping.Update.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Edm.Services
{
  internal class FunctionParameterMappingGenerator : StructuralTypeMappingGenerator
  {
    public FunctionParameterMappingGenerator(DbProviderManifest providerManifest)
      : base(providerManifest)
    {
    }

    public IEnumerable<ModificationFunctionParameterBinding> Generate(
      ModificationOperator modificationOperator,
      IEnumerable<EdmProperty> properties,
      IList<ColumnMappingBuilder> columnMappings,
      IList<EdmProperty> propertyPath,
      bool useOriginalValues = false)
    {
      foreach (EdmProperty property1 in properties)
      {
        EdmProperty property = property1;
        if (property.IsComplexType && propertyPath.Any<EdmProperty>((Func<EdmProperty, bool>) (p =>
        {
          if (p.IsComplexType)
            return p.ComplexType == property.ComplexType;
          return false;
        })))
          throw Error.CircularComplexTypeHierarchy();
        propertyPath.Add(property);
        if (property.IsComplexType)
        {
          foreach (ModificationFunctionParameterBinding parameterBinding in this.Generate(modificationOperator, (IEnumerable<EdmProperty>) property.ComplexType.Properties, columnMappings, propertyPath, useOriginalValues))
            yield return parameterBinding;
        }
        else
        {
          StoreGeneratedPattern? generatedPattern1 = property.GetStoreGeneratedPattern();
          if ((generatedPattern1.GetValueOrDefault() != StoreGeneratedPattern.Identity ? 1 : (!generatedPattern1.HasValue ? 1 : 0)) != 0 || modificationOperator != ModificationOperator.Insert)
          {
            EdmProperty columnProperty = columnMappings.First<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (cm => cm.PropertyPath.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) propertyPath))).ColumnProperty;
            StoreGeneratedPattern? generatedPattern2 = property.GetStoreGeneratedPattern();
            if ((generatedPattern2.GetValueOrDefault() != StoreGeneratedPattern.Computed ? 1 : (!generatedPattern2.HasValue ? 1 : 0)) != 0 && (modificationOperator != ModificationOperator.Delete || property.IsKeyMember))
              yield return new ModificationFunctionParameterBinding(new FunctionParameter(columnProperty.Name, columnProperty.TypeUsage, ParameterMode.In), new ModificationFunctionMemberPath((IEnumerable<EdmMember>) propertyPath, (AssociationSet) null), !useOriginalValues);
            if (modificationOperator != ModificationOperator.Insert && property.ConcurrencyMode == ConcurrencyMode.Fixed)
              yield return new ModificationFunctionParameterBinding(new FunctionParameter(columnProperty.Name + "_Original", columnProperty.TypeUsage, ParameterMode.In), new ModificationFunctionMemberPath((IEnumerable<EdmMember>) propertyPath, (AssociationSet) null), false);
          }
        }
        propertyPath.Remove(property);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public IEnumerable<ModificationFunctionParameterBinding> Generate(
      IEnumerable<Tuple<ModificationFunctionMemberPath, EdmProperty>> iaFkProperties,
      bool useOriginalValues = false)
    {
      return iaFkProperties.Select(iaFkProperty => new
      {
        iaFkProperty = iaFkProperty,
        functionParameter = new FunctionParameter(iaFkProperty.Item2.Name, iaFkProperty.Item2.TypeUsage, ParameterMode.In)
      }).Select(_param1 => new ModificationFunctionParameterBinding(_param1.functionParameter, _param1.iaFkProperty.Item1, !useOriginalValues));
    }
  }
}
