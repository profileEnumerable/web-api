// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.ViewLoader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
  internal class ViewLoader
  {
    private readonly Dictionary<AssociationSet, AssociationSetMetadata> m_associationSetMetadata = new Dictionary<AssociationSet, AssociationSetMetadata>();
    private readonly Dictionary<EntitySetBase, Set<EntitySet>> m_affectedTables = new Dictionary<EntitySetBase, Set<EntitySet>>();
    private readonly Set<EdmMember> m_serverGenProperties = new Set<EdmMember>();
    private readonly Set<EdmMember> m_isNullConditionProperties = new Set<EdmMember>();
    private readonly Dictionary<EntitySetBase, ModificationFunctionMappingTranslator> m_functionMappingTranslators = new Dictionary<EntitySetBase, ModificationFunctionMappingTranslator>((IEqualityComparer<EntitySetBase>) EqualityComparer<EntitySetBase>.Default);
    private readonly ReaderWriterLockSlim m_readerWriterLock = new ReaderWriterLockSlim();
    private readonly StorageMappingItemCollection m_mappingCollection;

    internal ViewLoader(StorageMappingItemCollection mappingCollection)
    {
      this.m_mappingCollection = mappingCollection;
    }

    internal ModificationFunctionMappingTranslator GetFunctionMappingTranslator(
      EntitySetBase extent,
      MetadataWorkspace workspace)
    {
      return this.SyncGetValue<EntitySetBase, ModificationFunctionMappingTranslator>(extent, workspace, this.m_functionMappingTranslators, extent);
    }

    internal Set<EntitySet> GetAffectedTables(
      EntitySetBase extent,
      MetadataWorkspace workspace)
    {
      return this.SyncGetValue<EntitySetBase, Set<EntitySet>>(extent, workspace, this.m_affectedTables, extent);
    }

    internal AssociationSetMetadata GetAssociationSetMetadata(
      AssociationSet associationSet,
      MetadataWorkspace workspace)
    {
      return this.SyncGetValue<AssociationSet, AssociationSetMetadata>((EntitySetBase) associationSet, workspace, this.m_associationSetMetadata, associationSet);
    }

    internal bool IsServerGen(
      EntitySetBase entitySetBase,
      MetadataWorkspace workspace,
      EdmMember member)
    {
      return this.SyncContains<EdmMember>(entitySetBase, workspace, this.m_serverGenProperties, member);
    }

    internal bool IsNullConditionMember(
      EntitySetBase entitySetBase,
      MetadataWorkspace workspace,
      EdmMember member)
    {
      return this.SyncContains<EdmMember>(entitySetBase, workspace, this.m_isNullConditionProperties, member);
    }

    private T_Value SyncGetValue<T_Key, T_Value>(
      EntitySetBase entitySetBase,
      MetadataWorkspace workspace,
      Dictionary<T_Key, T_Value> dictionary,
      T_Key key)
    {
      return this.SyncInitializeEntitySet<T_Key, T_Value>(entitySetBase, workspace, (Func<T_Key, T_Value>) (k => dictionary[k]), key);
    }

    private bool SyncContains<T_Element>(
      EntitySetBase entitySetBase,
      MetadataWorkspace workspace,
      Set<T_Element> set,
      T_Element element)
    {
      return this.SyncInitializeEntitySet<T_Element, bool>(entitySetBase, workspace, new Func<T_Element, bool>(set.Contains), element);
    }

    private TResult SyncInitializeEntitySet<TArg, TResult>(
      EntitySetBase entitySetBase,
      MetadataWorkspace workspace,
      Func<TArg, TResult> evaluate,
      TArg arg)
    {
      this.m_readerWriterLock.EnterReadLock();
      try
      {
        if (this.m_affectedTables.ContainsKey(entitySetBase))
          return evaluate(arg);
      }
      finally
      {
        this.m_readerWriterLock.ExitReadLock();
      }
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        if (this.m_affectedTables.ContainsKey(entitySetBase))
          return evaluate(arg);
        this.InitializeEntitySet(entitySetBase, workspace);
        return evaluate(arg);
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
      }
    }

    private void InitializeEntitySet(EntitySetBase entitySetBase, MetadataWorkspace workspace)
    {
      EntityContainerMapping map = (EntityContainerMapping) this.m_mappingCollection.GetMap((GlobalItem) entitySetBase.EntityContainer);
      if (map.HasViews)
        this.m_mappingCollection.GetGeneratedView(entitySetBase, workspace);
      Set<EntitySet> set = new Set<EntitySet>();
      if (map != null)
      {
        Set<EdmMember> columns = new Set<EdmMember>();
        EntitySetBaseMapping setMapping;
        if (entitySetBase.BuiltInTypeKind == BuiltInTypeKind.EntitySet)
        {
          setMapping = map.GetEntitySetMapping(entitySetBase.Name);
          this.m_serverGenProperties.Unite(ViewLoader.GetMembersWithResultBinding((EntitySetMapping) setMapping));
        }
        else
        {
          if (entitySetBase.BuiltInTypeKind != BuiltInTypeKind.AssociationSet)
            throw new NotSupportedException();
          setMapping = map.GetAssociationSetMapping(entitySetBase.Name);
        }
        foreach (MappingFragment mappingFragment in ViewLoader.GetMappingFragments(setMapping))
        {
          set.Add(mappingFragment.TableSet);
          this.m_serverGenProperties.AddRange(ViewLoader.FindServerGenMembers(mappingFragment));
          columns.AddRange(ViewLoader.FindIsNullConditionColumns(mappingFragment));
        }
        if (0 < columns.Count)
        {
          foreach (MappingFragment mappingFragment in ViewLoader.GetMappingFragments(setMapping))
            this.m_isNullConditionProperties.AddRange(ViewLoader.FindPropertiesMappedToColumns(columns, mappingFragment));
        }
      }
      this.m_affectedTables.Add(entitySetBase, set.MakeReadOnly());
      this.InitializeFunctionMappingTranslators(entitySetBase, map);
      if (entitySetBase.BuiltInTypeKind != BuiltInTypeKind.AssociationSet)
        return;
      AssociationSet associationSet = (AssociationSet) entitySetBase;
      if (this.m_associationSetMetadata.ContainsKey(associationSet))
        return;
      this.m_associationSetMetadata.Add(associationSet, new AssociationSetMetadata(this.m_affectedTables[(EntitySetBase) associationSet], associationSet, workspace));
    }

    private static IEnumerable<EdmMember> GetMembersWithResultBinding(
      EntitySetMapping entitySetMapping)
    {
      foreach (EntityTypeModificationFunctionMapping modificationFunctionMapping in entitySetMapping.ModificationFunctionMappings)
      {
        if (modificationFunctionMapping.InsertFunctionMapping != null && modificationFunctionMapping.InsertFunctionMapping.ResultBindings != null)
        {
          foreach (ModificationFunctionResultBinding resultBinding in modificationFunctionMapping.InsertFunctionMapping.ResultBindings)
            yield return (EdmMember) resultBinding.Property;
        }
        if (modificationFunctionMapping.UpdateFunctionMapping != null && modificationFunctionMapping.UpdateFunctionMapping.ResultBindings != null)
        {
          foreach (ModificationFunctionResultBinding resultBinding in modificationFunctionMapping.UpdateFunctionMapping.ResultBindings)
            yield return (EdmMember) resultBinding.Property;
        }
      }
    }

    private void InitializeFunctionMappingTranslators(
      EntitySetBase entitySetBase,
      EntityContainerMapping mapping)
    {
      KeyToListMap<AssociationSet, AssociationEndMember> keyToListMap = new KeyToListMap<AssociationSet, AssociationEndMember>((IEqualityComparer<AssociationSet>) EqualityComparer<AssociationSet>.Default);
      if (!this.m_functionMappingTranslators.ContainsKey(entitySetBase))
      {
        foreach (EntitySetMapping entitySetMap in mapping.EntitySetMaps)
        {
          if (0 < entitySetMap.ModificationFunctionMappings.Count)
          {
            this.m_functionMappingTranslators.Add(entitySetMap.Set, ModificationFunctionMappingTranslator.CreateEntitySetTranslator(entitySetMap));
            foreach (AssociationSetEnd associationSetEnd in entitySetMap.ImplicitlyMappedAssociationSetEnds)
            {
              AssociationSet parentAssociationSet = associationSetEnd.ParentAssociationSet;
              if (!this.m_functionMappingTranslators.ContainsKey((EntitySetBase) parentAssociationSet))
                this.m_functionMappingTranslators.Add((EntitySetBase) parentAssociationSet, ModificationFunctionMappingTranslator.CreateAssociationSetTranslator((AssociationSetMapping) null));
              AssociationSetEnd oppositeEnd = MetadataHelper.GetOppositeEnd(associationSetEnd);
              keyToListMap.Add(parentAssociationSet, oppositeEnd.CorrespondingAssociationEndMember);
            }
          }
          else
            this.m_functionMappingTranslators.Add(entitySetMap.Set, (ModificationFunctionMappingTranslator) null);
        }
        foreach (AssociationSetMapping relationshipSetMap in mapping.RelationshipSetMaps)
        {
          if (relationshipSetMap.ModificationFunctionMapping != null)
          {
            AssociationSet set = (AssociationSet) relationshipSetMap.Set;
            this.m_functionMappingTranslators.Add((EntitySetBase) set, ModificationFunctionMappingTranslator.CreateAssociationSetTranslator(relationshipSetMap));
            keyToListMap.AddRange(set, Enumerable.Empty<AssociationEndMember>());
          }
          else if (!this.m_functionMappingTranslators.ContainsKey(relationshipSetMap.Set))
            this.m_functionMappingTranslators.Add(relationshipSetMap.Set, (ModificationFunctionMappingTranslator) null);
        }
      }
      foreach (AssociationSet key in keyToListMap.Keys)
        this.m_associationSetMetadata.Add(key, new AssociationSetMetadata(keyToListMap.EnumerateValues(key)));
    }

    private static IEnumerable<EdmMember> FindServerGenMembers(
      MappingFragment mappingFragment)
    {
      foreach (ScalarPropertyMapping scalarPropertyMapping in ViewLoader.FlattenPropertyMappings(mappingFragment.AllProperties).OfType<ScalarPropertyMapping>())
      {
        if (MetadataHelper.GetStoreGeneratedPattern((EdmMember) scalarPropertyMapping.Column) != StoreGeneratedPattern.None)
          yield return (EdmMember) scalarPropertyMapping.Property;
      }
    }

    private static IEnumerable<EdmMember> FindIsNullConditionColumns(
      MappingFragment mappingFragment)
    {
      foreach (ConditionPropertyMapping conditionPropertyMapping in ViewLoader.FlattenPropertyMappings(mappingFragment.AllProperties).OfType<ConditionPropertyMapping>())
      {
        if (conditionPropertyMapping.Column != null && conditionPropertyMapping.IsNull.HasValue)
          yield return (EdmMember) conditionPropertyMapping.Column;
      }
    }

    private static IEnumerable<EdmMember> FindPropertiesMappedToColumns(
      Set<EdmMember> columns,
      MappingFragment mappingFragment)
    {
      foreach (ScalarPropertyMapping scalarPropertyMapping in ViewLoader.FlattenPropertyMappings(mappingFragment.AllProperties).OfType<ScalarPropertyMapping>())
      {
        if (columns.Contains((EdmMember) scalarPropertyMapping.Column))
          yield return (EdmMember) scalarPropertyMapping.Property;
      }
    }

    private static IEnumerable<MappingFragment> GetMappingFragments(
      EntitySetBaseMapping setMapping)
    {
      foreach (TypeMapping typeMapping in setMapping.TypeMappings)
      {
        foreach (MappingFragment mappingFragment in typeMapping.MappingFragments)
          yield return mappingFragment;
      }
    }

    private static IEnumerable<PropertyMapping> FlattenPropertyMappings(
      ReadOnlyCollection<PropertyMapping> propertyMappings)
    {
      foreach (PropertyMapping propertyMapping in propertyMappings)
      {
        ComplexPropertyMapping complexPropertyMapping = propertyMapping as ComplexPropertyMapping;
        if (complexPropertyMapping != null)
        {
          foreach (ComplexTypeMapping typeMapping in complexPropertyMapping.TypeMappings)
          {
            foreach (PropertyMapping flattenPropertyMapping in ViewLoader.FlattenPropertyMappings(typeMapping.AllProperties))
              yield return flattenPropertyMapping;
          }
        }
        else
          yield return propertyMapping;
      }
    }
  }
}
