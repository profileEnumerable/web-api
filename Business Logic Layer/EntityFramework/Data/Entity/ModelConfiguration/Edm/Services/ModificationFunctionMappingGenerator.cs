// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Edm.Services.ModificationFunctionMappingGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Mapping.Update.Internal;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Edm.Services
{
  internal class ModificationFunctionMappingGenerator : StructuralTypeMappingGenerator
  {
    public ModificationFunctionMappingGenerator(DbProviderManifest providerManifest)
      : base(providerManifest)
    {
    }

    public void Generate(EntityType entityType, DbDatabaseMapping databaseMapping)
    {
      if (entityType.Abstract)
        return;
      EntitySet entitySet = databaseMapping.Model.GetEntitySet(entityType);
      EntitySetMapping entitySetMapping = databaseMapping.GetEntitySetMapping(entitySet);
      List<ColumnMappingBuilder> list1 = ModificationFunctionMappingGenerator.GetColumnMappings(entityType, entitySetMapping).ToList<ColumnMappingBuilder>();
      List<Tuple<ModificationFunctionMemberPath, EdmProperty>> list2 = ModificationFunctionMappingGenerator.GetIndependentFkColumns(entityType, databaseMapping).ToList<Tuple<ModificationFunctionMemberPath, EdmProperty>>();
      ModificationFunctionMapping functionMapping1 = this.GenerateFunctionMapping(ModificationOperator.Insert, (EntitySetBase) entitySetMapping.EntitySet, (EntityTypeBase) entityType, databaseMapping, (IEnumerable<EdmProperty>) entityType.Properties, (IEnumerable<Tuple<ModificationFunctionMemberPath, EdmProperty>>) list2, (IList<ColumnMappingBuilder>) list1, entityType.Properties.Where<EdmProperty>((Func<EdmProperty, bool>) (p => p.HasStoreGeneratedPattern())), (string) null);
      ModificationFunctionMapping functionMapping2 = this.GenerateFunctionMapping(ModificationOperator.Update, (EntitySetBase) entitySetMapping.EntitySet, (EntityTypeBase) entityType, databaseMapping, (IEnumerable<EdmProperty>) entityType.Properties, (IEnumerable<Tuple<ModificationFunctionMemberPath, EdmProperty>>) list2, (IList<ColumnMappingBuilder>) list1, entityType.Properties.Where<EdmProperty>((Func<EdmProperty, bool>) (p =>
      {
        StoreGeneratedPattern? generatedPattern = p.GetStoreGeneratedPattern();
        if (generatedPattern.GetValueOrDefault() == StoreGeneratedPattern.Computed)
          return generatedPattern.HasValue;
        return false;
      })), (string) null);
      ModificationFunctionMapping functionMapping3 = this.GenerateFunctionMapping(ModificationOperator.Delete, (EntitySetBase) entitySetMapping.EntitySet, (EntityTypeBase) entityType, databaseMapping, (IEnumerable<EdmProperty>) entityType.Properties, (IEnumerable<Tuple<ModificationFunctionMemberPath, EdmProperty>>) list2, (IList<ColumnMappingBuilder>) list1, (IEnumerable<EdmProperty>) null, (string) null);
      EntityTypeModificationFunctionMapping modificationFunctionMapping = new EntityTypeModificationFunctionMapping(entityType, functionMapping3, functionMapping1, functionMapping2);
      entitySetMapping.AddModificationFunctionMapping(modificationFunctionMapping);
    }

    private static IEnumerable<ColumnMappingBuilder> GetColumnMappings(
      EntityType entityType,
      EntitySetMapping entitySetMapping)
    {
      return ((IEnumerable<EntityType>) new EntityType[1]
      {
        entityType
      }).Concat<EntityType>(ModificationFunctionMappingGenerator.GetParents(entityType)).SelectMany<EntityType, ColumnMappingBuilder>((Func<EntityType, IEnumerable<ColumnMappingBuilder>>) (et => entitySetMapping.TypeMappings.Where<TypeMapping>((Func<TypeMapping, bool>) (stm => stm.Types.Contains((EntityTypeBase) et))).SelectMany<TypeMapping, MappingFragment>((Func<TypeMapping, IEnumerable<MappingFragment>>) (stm => (IEnumerable<MappingFragment>) stm.MappingFragments)).SelectMany<MappingFragment, ColumnMappingBuilder>((Func<MappingFragment, IEnumerable<ColumnMappingBuilder>>) (mf => mf.ColumnMappings))));
    }

    public void Generate(
      AssociationSetMapping associationSetMapping,
      DbDatabaseMapping databaseMapping)
    {
      List<Tuple<ModificationFunctionMemberPath, EdmProperty>> list = ModificationFunctionMappingGenerator.GetIndependentFkColumns(associationSetMapping).ToList<Tuple<ModificationFunctionMemberPath, EdmProperty>>();
      string functionNamePrefix = associationSetMapping.AssociationSet.ElementType.SourceEnd.GetEntityType().Name + associationSetMapping.AssociationSet.ElementType.TargetEnd.GetEntityType().Name;
      ModificationFunctionMapping functionMapping1 = this.GenerateFunctionMapping(ModificationOperator.Insert, (EntitySetBase) associationSetMapping.AssociationSet, (EntityTypeBase) associationSetMapping.AssociationSet.ElementType, databaseMapping, Enumerable.Empty<EdmProperty>(), (IEnumerable<Tuple<ModificationFunctionMemberPath, EdmProperty>>) list, (IList<ColumnMappingBuilder>) new ColumnMappingBuilder[0], (IEnumerable<EdmProperty>) null, functionNamePrefix);
      ModificationFunctionMapping functionMapping2 = this.GenerateFunctionMapping(ModificationOperator.Delete, (EntitySetBase) associationSetMapping.AssociationSet, (EntityTypeBase) associationSetMapping.AssociationSet.ElementType, databaseMapping, Enumerable.Empty<EdmProperty>(), (IEnumerable<Tuple<ModificationFunctionMemberPath, EdmProperty>>) list, (IList<ColumnMappingBuilder>) new ColumnMappingBuilder[0], (IEnumerable<EdmProperty>) null, functionNamePrefix);
      associationSetMapping.ModificationFunctionMapping = new AssociationSetModificationFunctionMapping(associationSetMapping.AssociationSet, functionMapping2, functionMapping1);
    }

    private static IEnumerable<Tuple<ModificationFunctionMemberPath, EdmProperty>> GetIndependentFkColumns(
      AssociationSetMapping associationSetMapping)
    {
      foreach (ScalarPropertyMapping propertyMapping in associationSetMapping.SourceEndMapping.PropertyMappings)
        yield return Tuple.Create<ModificationFunctionMemberPath, EdmProperty>(new ModificationFunctionMemberPath((IEnumerable<EdmMember>) new EdmMember[2]
        {
          (EdmMember) propertyMapping.Property,
          (EdmMember) associationSetMapping.SourceEndMapping.AssociationEnd
        }, associationSetMapping.AssociationSet), propertyMapping.Column);
      foreach (ScalarPropertyMapping propertyMapping in associationSetMapping.TargetEndMapping.PropertyMappings)
        yield return Tuple.Create<ModificationFunctionMemberPath, EdmProperty>(new ModificationFunctionMemberPath((IEnumerable<EdmMember>) new EdmMember[2]
        {
          (EdmMember) propertyMapping.Property,
          (EdmMember) associationSetMapping.TargetEndMapping.AssociationEnd
        }, associationSetMapping.AssociationSet), propertyMapping.Column);
    }

    private static IEnumerable<Tuple<ModificationFunctionMemberPath, EdmProperty>> GetIndependentFkColumns(
      EntityType entityType,
      DbDatabaseMapping databaseMapping)
    {
      foreach (AssociationSetMapping associationSetMapping in databaseMapping.GetAssociationSetMappings())
      {
        AssociationType associationType = associationSetMapping.AssociationSet.ElementType;
        if (!associationType.IsManyToMany())
        {
          AssociationEndMember _;
          AssociationEndMember dependentEnd;
          if (!associationType.TryGuessPrincipalAndDependentEnds(out _, out dependentEnd))
            dependentEnd = associationType.TargetEnd;
          EntityType dependentEntityType = dependentEnd.GetEntityType();
          if (dependentEntityType == entityType || ModificationFunctionMappingGenerator.GetParents(entityType).Contains<EntityType>(dependentEntityType))
          {
            EndPropertyMapping endPropertyMapping = associationSetMapping.TargetEndMapping.AssociationEnd != dependentEnd ? associationSetMapping.TargetEndMapping : associationSetMapping.SourceEndMapping;
            foreach (ScalarPropertyMapping propertyMapping in endPropertyMapping.PropertyMappings)
              yield return Tuple.Create<ModificationFunctionMemberPath, EdmProperty>(new ModificationFunctionMemberPath((IEnumerable<EdmMember>) new EdmMember[2]
              {
                (EdmMember) propertyMapping.Property,
                (EdmMember) dependentEnd
              }, associationSetMapping.AssociationSet), propertyMapping.Column);
          }
        }
      }
    }

    private static IEnumerable<EntityType> GetParents(EntityType entityType)
    {
      for (; entityType.BaseType != null; entityType = (EntityType) entityType.BaseType)
        yield return (EntityType) entityType.BaseType;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    private ModificationFunctionMapping GenerateFunctionMapping(
      ModificationOperator modificationOperator,
      EntitySetBase entitySetBase,
      EntityTypeBase entityTypeBase,
      DbDatabaseMapping databaseMapping,
      IEnumerable<EdmProperty> parameterProperties,
      IEnumerable<Tuple<ModificationFunctionMemberPath, EdmProperty>> iaFkProperties,
      IList<ColumnMappingBuilder> columnMappings,
      IEnumerable<EdmProperty> resultProperties = null,
      string functionNamePrefix = null)
    {
      bool useOriginalValues = modificationOperator == ModificationOperator.Delete;
      FunctionParameterMappingGenerator mappingGenerator = new FunctionParameterMappingGenerator(this._providerManifest);
      List<ModificationFunctionParameterBinding> list1 = mappingGenerator.Generate(modificationOperator != ModificationOperator.Insert || !ModificationFunctionMappingGenerator.IsTableSplitDependent(entityTypeBase, databaseMapping) ? modificationOperator : ModificationOperator.Update, parameterProperties, columnMappings, (IList<EdmProperty>) new List<EdmProperty>(), useOriginalValues).Concat<ModificationFunctionParameterBinding>(mappingGenerator.Generate(iaFkProperties, useOriginalValues)).ToList<ModificationFunctionParameterBinding>();
      List<FunctionParameter> list2 = list1.Select<ModificationFunctionParameterBinding, FunctionParameter>((Func<ModificationFunctionParameterBinding, FunctionParameter>) (b => b.Parameter)).ToList<FunctionParameter>();
      ModificationFunctionMappingGenerator.UniquifyParameterNames((IList<FunctionParameter>) list2);
      EdmFunctionPayload functionPayload = new EdmFunctionPayload()
      {
        ReturnParameters = (IList<FunctionParameter>) new FunctionParameter[0],
        Parameters = (IList<FunctionParameter>) list2.ToArray(),
        IsComposable = new bool?(false)
      };
      EdmFunction function = databaseMapping.Database.AddFunction((functionNamePrefix ?? entityTypeBase.Name) + "_" + modificationOperator.ToString(), functionPayload);
      return new ModificationFunctionMapping(entitySetBase, entityTypeBase, function, (IEnumerable<ModificationFunctionParameterBinding>) list1, (FunctionParameter) null, resultProperties != null ? resultProperties.Select<EdmProperty, ModificationFunctionResultBinding>((Func<EdmProperty, ModificationFunctionResultBinding>) (p => new ModificationFunctionResultBinding(columnMappings.First<ColumnMappingBuilder>((Func<ColumnMappingBuilder, bool>) (cm => cm.PropertyPath.SequenceEqual<EdmProperty>((IEnumerable<EdmProperty>) new EdmProperty[1]
      {
        p
      }))).ColumnProperty.Name, p))) : (IEnumerable<ModificationFunctionResultBinding>) null);
    }

    private static bool IsTableSplitDependent(
      EntityTypeBase entityTypeBase,
      DbDatabaseMapping databaseMapping)
    {
      AssociationType associationType = databaseMapping.Model.AssociationTypes.SingleOrDefault<AssociationType>((Func<AssociationType, bool>) (at =>
      {
        if (at.IsForeignKey && at.IsRequiredToRequired() && !at.IsSelfReferencing() && (at.SourceEnd.GetEntityType().IsAssignableFrom((EdmType) entityTypeBase) || at.TargetEnd.GetEntityType().IsAssignableFrom((EdmType) entityTypeBase)))
          return databaseMapping.Database.AssociationTypes.All<AssociationType>((Func<AssociationType, bool>) (fk => fk.Name != at.Name));
        return false;
      }));
      if (associationType != null)
        return associationType.TargetEnd.GetEntityType() == entityTypeBase;
      return false;
    }

    private static void UniquifyParameterNames(IList<FunctionParameter> parameters)
    {
      foreach (FunctionParameter parameter in (IEnumerable<FunctionParameter>) parameters)
        parameter.Name = ((IEnumerable<INamedDataModelItem>) parameters.Except<FunctionParameter>((IEnumerable<FunctionParameter>) new FunctionParameter[1]
        {
          parameter
        })).UniquifyName(parameter.Name);
    }
  }
}
