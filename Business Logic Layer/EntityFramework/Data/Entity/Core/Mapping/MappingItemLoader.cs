// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.MappingItemLoader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Data.Entity.Core.Mapping
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  internal class MappingItemLoader
  {
    private readonly Dictionary<string, string> m_alias;
    private readonly StorageMappingItemCollection m_storageMappingItemCollection;
    private readonly string m_sourceLocation;
    private readonly List<EdmSchemaError> m_parsingErrors;
    private readonly Dictionary<EdmMember, KeyValuePair<TypeUsage, TypeUsage>> m_scalarMemberMappings;
    private bool m_hasQueryViews;
    private string m_currentNamespaceUri;
    private readonly EntityContainerMapping m_containerMapping;
    private readonly double m_version;
    private static XmlSchemaSet s_mappingXmlSchema;

    internal MappingItemLoader(
      XmlReader reader,
      StorageMappingItemCollection storageMappingItemCollection,
      string fileName,
      Dictionary<EdmMember, KeyValuePair<TypeUsage, TypeUsage>> scalarMemberMappings)
    {
      this.m_storageMappingItemCollection = storageMappingItemCollection;
      this.m_alias = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.m_sourceLocation = fileName == null ? (string) null : fileName;
      this.m_parsingErrors = new List<EdmSchemaError>();
      this.m_scalarMemberMappings = scalarMemberMappings;
      this.m_containerMapping = this.LoadMappingItems(reader);
      if (this.m_currentNamespaceUri == null)
        return;
      if (this.m_currentNamespaceUri == "urn:schemas-microsoft-com:windows:storage:mapping:CS")
        this.m_version = 1.0;
      else if (this.m_currentNamespaceUri == "http://schemas.microsoft.com/ado/2008/09/mapping/cs")
        this.m_version = 2.0;
      else
        this.m_version = 3.0;
    }

    internal double MappingVersion
    {
      get
      {
        return this.m_version;
      }
    }

    internal IList<EdmSchemaError> ParsingErrors
    {
      get
      {
        return (IList<EdmSchemaError>) this.m_parsingErrors;
      }
    }

    internal bool HasQueryViews
    {
      get
      {
        return this.m_hasQueryViews;
      }
    }

    internal EntityContainerMapping ContainerMapping
    {
      get
      {
        return this.m_containerMapping;
      }
    }

    private EdmItemCollection EdmItemCollection
    {
      get
      {
        return this.m_storageMappingItemCollection.EdmItemCollection;
      }
    }

    private StoreItemCollection StoreItemCollection
    {
      get
      {
        return this.m_storageMappingItemCollection.StoreItemCollection;
      }
    }

    private EntityContainerMapping LoadMappingItems(XmlReader innerReader)
    {
      XmlReader validatingReader = this.GetSchemaValidatingReader(innerReader);
      try
      {
        XPathDocument xpathDocument = new XPathDocument(validatingReader);
        if (this.m_parsingErrors.Count != 0 && !MetadataHelper.CheckIfAllErrorsAreWarnings((IList<EdmSchemaError>) this.m_parsingErrors))
          return (EntityContainerMapping) null;
        return this.LoadMappingItems(xpathDocument.CreateNavigator().Clone());
      }
      catch (XmlException ex)
      {
        this.m_parsingErrors.Add(new EdmSchemaError(Strings.Mapping_InvalidMappingSchema_Parsing((object) ex.Message), 2024, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, ex.LineNumber, ex.LinePosition));
      }
      return (EntityContainerMapping) null;
    }

    private EntityContainerMapping LoadMappingItems(XPathNavigator nav)
    {
      if (!this.MoveToRootElement(nav) || nav.NodeType != XPathNodeType.Element)
      {
        MappingItemLoader.AddToSchemaErrors(Strings.Mapping_Invalid_CSRootElementMissing((object) "urn:schemas-microsoft-com:windows:storage:mapping:CS", (object) "http://schemas.microsoft.com/ado/2008/09/mapping/cs", (object) "http://schemas.microsoft.com/ado/2009/11/mapping/cs"), MappingErrorCode.RootMappingElementMissing, this.m_sourceLocation, (IXmlLineInfo) nav, (IList<EdmSchemaError>) this.m_parsingErrors);
        return (EntityContainerMapping) null;
      }
      EntityContainerMapping containerMapping = this.LoadMappingChildNodes(nav.Clone());
      if (this.m_parsingErrors.Count != 0 && !MetadataHelper.CheckIfAllErrorsAreWarnings((IList<EdmSchemaError>) this.m_parsingErrors))
        containerMapping = (EntityContainerMapping) null;
      return containerMapping;
    }

    private bool MoveToRootElement(XPathNavigator nav)
    {
      if (nav.MoveToChild("Mapping", "http://schemas.microsoft.com/ado/2009/11/mapping/cs"))
      {
        this.m_currentNamespaceUri = "http://schemas.microsoft.com/ado/2009/11/mapping/cs";
        return true;
      }
      if (nav.MoveToChild("Mapping", "http://schemas.microsoft.com/ado/2008/09/mapping/cs"))
      {
        this.m_currentNamespaceUri = "http://schemas.microsoft.com/ado/2008/09/mapping/cs";
        return true;
      }
      if (!nav.MoveToChild("Mapping", "urn:schemas-microsoft-com:windows:storage:mapping:CS"))
        return false;
      this.m_currentNamespaceUri = "urn:schemas-microsoft-com:windows:storage:mapping:CS";
      return true;
    }

    private EntityContainerMapping LoadMappingChildNodes(XPathNavigator nav)
    {
      bool flag;
      if (nav.MoveToChild("Alias", this.m_currentNamespaceUri))
      {
        do
        {
          this.m_alias.Add(MappingItemLoader.GetAttributeValue(nav.Clone(), "Key"), MappingItemLoader.GetAttributeValue(nav.Clone(), "Value"));
        }
        while (nav.MoveToNext("Alias", this.m_currentNamespaceUri));
        flag = nav.MoveToNext(XPathNodeType.Element);
      }
      else
        flag = nav.MoveToChild(XPathNodeType.Element);
      return flag ? this.LoadEntityContainerMapping(nav.Clone()) : (EntityContainerMapping) null;
    }

    private EntityContainerMapping LoadEntityContainerMapping(
      XPathNavigator nav)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      string attributeValue1 = MappingItemLoader.GetAttributeValue(nav.Clone(), "CdmEntityContainer");
      string attributeValue2 = MappingItemLoader.GetAttributeValue(nav.Clone(), "StorageEntityContainer");
      bool boolAttributeValue = MappingItemLoader.GetBoolAttributeValue(nav.Clone(), "GenerateUpdateViews", true);
      EntityContainerMapping entityContainerMapping;
      System.Data.Entity.Core.Metadata.Edm.EntityContainer entityContainer1;
      if (this.m_storageMappingItemCollection.TryGetItem<EntityContainerMapping>(attributeValue1, out entityContainerMapping))
      {
        System.Data.Entity.Core.Metadata.Edm.EntityContainer edmEntityContainer = entityContainerMapping.EdmEntityContainer;
        entityContainer1 = entityContainerMapping.StorageEntityContainer;
        if (attributeValue2 != entityContainer1.Name)
        {
          MappingItemLoader.AddToSchemaErrors(Strings.StorageEntityContainerNameMismatchWhileSpecifyingPartialMapping((object) attributeValue2, (object) entityContainer1.Name, (object) edmEntityContainer.Name), MappingErrorCode.StorageEntityContainerNameMismatchWhileSpecifyingPartialMapping, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          return (EntityContainerMapping) null;
        }
      }
      else
      {
        if (this.m_storageMappingItemCollection.ContainsStorageEntityContainer(attributeValue2))
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_AlreadyMapped_StorageEntityContainer), attributeValue2, MappingErrorCode.AlreadyMappedStorageEntityContainer, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          return (EntityContainerMapping) null;
        }
        System.Data.Entity.Core.Metadata.Edm.EntityContainer entityContainer2;
        this.EdmItemCollection.TryGetEntityContainer(attributeValue1, out entityContainer2);
        if (entityContainer2 == null)
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_EntityContainer), attributeValue1, MappingErrorCode.InvalidEntityContainer, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        this.StoreItemCollection.TryGetEntityContainer(attributeValue2, out entityContainer1);
        if (entityContainer1 == null)
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_StorageEntityContainer), attributeValue2, MappingErrorCode.InvalidEntityContainer, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        if (entityContainer2 == null || entityContainer1 == null)
          return (EntityContainerMapping) null;
        entityContainerMapping = new EntityContainerMapping(entityContainer2, entityContainer1, this.m_storageMappingItemCollection, boolAttributeValue, boolAttributeValue);
        entityContainerMapping.StartLineNumber = lineInfo.LineNumber;
        entityContainerMapping.StartLinePosition = lineInfo.LinePosition;
      }
      this.LoadEntityContainerMappingChildNodes(nav.Clone(), entityContainerMapping, entityContainer1);
      return entityContainerMapping;
    }

    private void LoadEntityContainerMappingChildNodes(
      XPathNavigator nav,
      EntityContainerMapping entityContainerMapping,
      System.Data.Entity.Core.Metadata.Edm.EntityContainer storageEntityContainerType)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      bool flag = false;
      if (nav.MoveToChild(XPathNodeType.Element))
      {
        do
        {
          switch (nav.LocalName)
          {
            case "EntitySetMapping":
              this.LoadEntitySetMapping(nav.Clone(), entityContainerMapping, storageEntityContainerType);
              flag = true;
              break;
            case "AssociationSetMapping":
              this.LoadAssociationSetMapping(nav.Clone(), entityContainerMapping, storageEntityContainerType);
              break;
            case "FunctionImportMapping":
              this.LoadFunctionImportMapping(nav.Clone(), entityContainerMapping);
              break;
            default:
              MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_Container_SubElement, MappingErrorCode.SetMappingExpected, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
              break;
          }
        }
        while (nav.MoveToNext(XPathNodeType.Element));
      }
      if (entityContainerMapping.EdmEntityContainer.BaseEntitySets.Count != 0 && !flag)
      {
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.ViewGen_Missing_Sets_Mapping), entityContainerMapping.EdmEntityContainer.Name, MappingErrorCode.EmptyContainerMapping, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
      else
      {
        this.ValidateFunctionAssociationFunctionMappingUnique(nav.Clone(), entityContainerMapping);
        this.ValidateModificationFunctionMappingConsistentForAssociations(nav.Clone(), entityContainerMapping);
        this.ValidateQueryViewsClosure(nav.Clone(), entityContainerMapping);
        this.ValidateEntitySetFunctionMappingClosure(nav.Clone(), entityContainerMapping);
        entityContainerMapping.SourceLocation = this.m_sourceLocation;
      }
    }

    private void ValidateModificationFunctionMappingConsistentForAssociations(
      XPathNavigator nav,
      EntityContainerMapping entityContainerMapping)
    {
      foreach (EntitySetMapping entitySetMap in entityContainerMapping.EntitySetMaps)
      {
        if (entitySetMap.ModificationFunctionMappings.Count > 0)
        {
          Set<AssociationSetEnd> expectedEnds = new Set<AssociationSetEnd>(entitySetMap.ImplicitlyMappedAssociationSetEnds).MakeReadOnly();
          foreach (EntityTypeModificationFunctionMapping modificationFunctionMapping in entitySetMap.ModificationFunctionMappings)
          {
            if (modificationFunctionMapping.DeleteFunctionMapping != null)
              this.ValidateModificationFunctionMappingConsistentForAssociations(nav, entitySetMap, modificationFunctionMapping, modificationFunctionMapping.DeleteFunctionMapping, expectedEnds, "DeleteFunction");
            if (modificationFunctionMapping.InsertFunctionMapping != null)
              this.ValidateModificationFunctionMappingConsistentForAssociations(nav, entitySetMap, modificationFunctionMapping, modificationFunctionMapping.InsertFunctionMapping, expectedEnds, "InsertFunction");
            if (modificationFunctionMapping.UpdateFunctionMapping != null)
              this.ValidateModificationFunctionMappingConsistentForAssociations(nav, entitySetMap, modificationFunctionMapping, modificationFunctionMapping.UpdateFunctionMapping, expectedEnds, "UpdateFunction");
          }
        }
      }
    }

    private void ValidateModificationFunctionMappingConsistentForAssociations(
      XPathNavigator nav,
      EntitySetMapping entitySetMapping,
      EntityTypeModificationFunctionMapping entityTypeMapping,
      ModificationFunctionMapping functionMapping,
      Set<AssociationSetEnd> expectedEnds,
      string elementName)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      Set<AssociationSetEnd> set = new Set<AssociationSetEnd>((IEnumerable<AssociationSetEnd>) functionMapping.CollocatedAssociationSetEnds);
      set.MakeReadOnly();
      foreach (AssociationSetEnd expectedEnd in expectedEnds)
      {
        if (MetadataHelper.IsAssociationValidForEntityType(expectedEnd, entityTypeMapping.EntityType) && !set.Contains(expectedEnd))
          MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_ModificationFunction_AssociationSetNotMappedForOperation((object) entitySetMapping.Set.Name, (object) expectedEnd.ParentAssociationSet.Name, (object) elementName, (object) entityTypeMapping.EntityType.FullName), MappingErrorCode.InvalidModificationFunctionMappingAssociationSetNotMappedForOperation, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
      foreach (AssociationSetEnd associationSetEnd in set)
      {
        if (!MetadataHelper.IsAssociationValidForEntityType(associationSetEnd, entityTypeMapping.EntityType))
          MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_ModificationFunction_AssociationEndMappingInvalidForEntityType((object) entityTypeMapping.EntityType.FullName, (object) associationSetEnd.ParentAssociationSet.Name, (object) MetadataHelper.GetEntityTypeForEnd(MetadataHelper.GetOppositeEnd(associationSetEnd).CorrespondingAssociationEndMember).FullName), MappingErrorCode.InvalidModificationFunctionMappingAssociationEndMappingInvalidForEntityType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
    }

    private void ValidateFunctionAssociationFunctionMappingUnique(
      XPathNavigator nav,
      EntityContainerMapping entityContainerMapping)
    {
      Dictionary<EntitySetBase, int> counts = new Dictionary<EntitySetBase, int>();
      foreach (EntitySetMapping entitySetMap in entityContainerMapping.EntitySetMaps)
      {
        if (entitySetMap.ModificationFunctionMappings.Count > 0)
        {
          Set<EntitySetBase> set = new Set<EntitySetBase>();
          foreach (AssociationSetEnd associationSetEnd in entitySetMap.ImplicitlyMappedAssociationSetEnds)
            set.Add((EntitySetBase) associationSetEnd.ParentAssociationSet);
          foreach (EntitySetBase key in set)
            MappingItemLoader.IncrementCount<EntitySetBase>(counts, key);
        }
      }
      foreach (AssociationSetMapping relationshipSetMap in entityContainerMapping.RelationshipSetMaps)
      {
        if (relationshipSetMap.ModificationFunctionMapping != null)
          MappingItemLoader.IncrementCount<EntitySetBase>(counts, relationshipSetMap.Set);
      }
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<EntitySetBase, int> keyValuePair in counts)
      {
        if (keyValuePair.Value > 1)
          stringList.Add(keyValuePair.Key.Name);
      }
      if (0 >= stringList.Count)
        return;
      MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_AssociationSetAmbiguous), StringUtil.ToCommaSeparatedString((IEnumerable) stringList), MappingErrorCode.AmbiguousModificationFunctionMappingForAssociationSet, this.m_sourceLocation, (IXmlLineInfo) nav, (IList<EdmSchemaError>) this.m_parsingErrors);
    }

    private static void IncrementCount<T>(Dictionary<T, int> counts, T key)
    {
      int num1;
      int num2 = !counts.TryGetValue(key, out num1) ? 1 : num1 + 1;
      counts[key] = num2;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private void ValidateEntitySetFunctionMappingClosure(
      XPathNavigator nav,
      EntityContainerMapping entityContainerMapping)
    {
      KeyToListMap<EntitySet, EntitySetBaseMapping> keyToListMap = new KeyToListMap<EntitySet, EntitySetBaseMapping>((IEqualityComparer<EntitySet>) EqualityComparer<EntitySet>.Default);
      foreach (EntitySetBaseMapping allSetMap in entityContainerMapping.AllSetMaps)
      {
        foreach (TypeMapping typeMapping in allSetMap.TypeMappings)
        {
          foreach (MappingFragment mappingFragment in typeMapping.MappingFragments)
            keyToListMap.Add(mappingFragment.TableSet, allSetMap);
        }
      }
      Set<EntitySetBase> implicitMappedAssociationSets = new Set<EntitySetBase>();
      foreach (EntitySetMapping entitySetMap in entityContainerMapping.EntitySetMaps)
      {
        if (entitySetMap.ModificationFunctionMappings.Count > 0)
        {
          foreach (AssociationSetEnd associationSetEnd in entitySetMap.ImplicitlyMappedAssociationSetEnds)
            implicitMappedAssociationSets.Add((EntitySetBase) associationSetEnd.ParentAssociationSet);
        }
      }
      foreach (EntitySet key in keyToListMap.Keys)
      {
        if (keyToListMap.ListForKey(key).Any<EntitySetBaseMapping>((Func<EntitySetBaseMapping, bool>) (s =>
        {
          if (!s.HasModificationFunctionMapping)
            return implicitMappedAssociationSets.Any<EntitySetBase>((Func<EntitySetBase, bool>) (aset => aset == s.Set));
          return true;
        })) && keyToListMap.ListForKey(key).Any<EntitySetBaseMapping>((Func<EntitySetBaseMapping, bool>) (s =>
        {
          if (!s.HasModificationFunctionMapping)
            return !implicitMappedAssociationSets.Any<EntitySetBase>((Func<EntitySetBase, bool>) (aset => aset == s.Set));
          return false;
        })))
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_MissingSetClosure), StringUtil.ToCommaSeparatedString((IEnumerable) keyToListMap.ListForKey(key).Where<EntitySetBaseMapping>((Func<EntitySetBaseMapping, bool>) (s => !s.HasModificationFunctionMapping)).Select<EntitySetBaseMapping, string>((Func<EntitySetBaseMapping, string>) (s => s.Set.Name))), MappingErrorCode.MissingSetClosureInModificationFunctionMapping, this.m_sourceLocation, (IXmlLineInfo) nav, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
    }

    private static void ValidateClosureAmongSets(
      EntityContainerMapping entityContainerMapping,
      Set<EntitySetBase> sets,
      Set<EntitySetBase> additionalSetsInClosure)
    {
      bool flag;
      do
      {
        flag = false;
        List<EntitySetBase> entitySetBaseList = new List<EntitySetBase>();
        foreach (EntitySetBase entitySetBase in additionalSetsInClosure)
        {
          AssociationSet associationSet = entitySetBase as AssociationSet;
          if (associationSet != null && !associationSet.ElementType.IsForeignKey)
          {
            foreach (AssociationSetEnd associationSetEnd in associationSet.AssociationSetEnds)
            {
              if (!additionalSetsInClosure.Contains((EntitySetBase) associationSetEnd.EntitySet))
                entitySetBaseList.Add((EntitySetBase) associationSetEnd.EntitySet);
            }
          }
        }
        foreach (EntitySetBase baseEntitySet in entityContainerMapping.EdmEntityContainer.BaseEntitySets)
        {
          AssociationSet associationSet = baseEntitySet as AssociationSet;
          if (associationSet != null && !associationSet.ElementType.IsForeignKey && !additionalSetsInClosure.Contains((EntitySetBase) associationSet))
          {
            foreach (AssociationSetEnd associationSetEnd in associationSet.AssociationSetEnds)
            {
              if (additionalSetsInClosure.Contains((EntitySetBase) associationSetEnd.EntitySet))
              {
                entitySetBaseList.Add((EntitySetBase) associationSet);
                break;
              }
            }
          }
        }
        if (0 < entitySetBaseList.Count)
        {
          flag = true;
          additionalSetsInClosure.AddRange((IEnumerable<EntitySetBase>) entitySetBaseList);
        }
      }
      while (flag);
      additionalSetsInClosure.Subtract((IEnumerable<EntitySetBase>) sets);
    }

    private void ValidateQueryViewsClosure(
      XPathNavigator nav,
      EntityContainerMapping entityContainerMapping)
    {
      if (!this.m_hasQueryViews)
        return;
      Set<EntitySetBase> sets = new Set<EntitySetBase>();
      Set<EntitySetBase> additionalSetsInClosure = new Set<EntitySetBase>();
      foreach (EntitySetBaseMapping allSetMap in entityContainerMapping.AllSetMaps)
      {
        if (allSetMap.QueryView != null)
          sets.Add(allSetMap.Set);
      }
      additionalSetsInClosure.AddRange((IEnumerable<EntitySetBase>) sets);
      MappingItemLoader.ValidateClosureAmongSets(entityContainerMapping, sets, additionalSetsInClosure);
      if (0 >= additionalSetsInClosure.Count)
        return;
      MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_Invalid_Query_Views_MissingSetClosure), StringUtil.ToCommaSeparatedString((IEnumerable) additionalSetsInClosure), MappingErrorCode.MissingSetClosureInQueryViews, this.m_sourceLocation, (IXmlLineInfo) nav, (IList<EdmSchemaError>) this.m_parsingErrors);
    }

    private void LoadEntitySetMapping(
      XPathNavigator nav,
      EntityContainerMapping entityContainerMapping,
      System.Data.Entity.Core.Metadata.Edm.EntityContainer storageEntityContainerType)
    {
      string resolvedAttributeValue1 = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
      string attributeValue = MappingItemLoader.GetAttributeValue(nav.Clone(), "TypeName");
      string resolvedAttributeValue2 = this.GetAliasResolvedAttributeValue(nav.Clone(), "StoreEntitySet");
      bool boolAttributeValue = MappingItemLoader.GetBoolAttributeValue(nav.Clone(), "MakeColumnsDistinct", false);
      EntitySetMapping entitySetMapping = (EntitySetMapping) entityContainerMapping.GetEntitySetMapping(resolvedAttributeValue1);
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      EntitySet entitySet;
      if (entitySetMapping == null)
      {
        if (!entityContainerMapping.EdmEntityContainer.TryGetEntitySetByName(resolvedAttributeValue1, false, out entitySet))
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Entity_Set), resolvedAttributeValue1, MappingErrorCode.InvalidEntitySet, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          return;
        }
        entitySetMapping = new EntitySetMapping(entitySet, entityContainerMapping);
      }
      else
        entitySet = (EntitySet) entitySetMapping.Set;
      entitySetMapping.StartLineNumber = lineInfo.LineNumber;
      entitySetMapping.StartLinePosition = lineInfo.LinePosition;
      entityContainerMapping.AddSetMapping(entitySetMapping);
      if (string.IsNullOrEmpty(attributeValue))
      {
        if (nav.MoveToChild(XPathNodeType.Element))
        {
          do
          {
            switch (nav.LocalName)
            {
              case "EntityTypeMapping":
                resolvedAttributeValue2 = this.GetAliasResolvedAttributeValue(nav.Clone(), "StoreEntitySet");
                this.LoadEntityTypeMapping(nav.Clone(), entitySetMapping, resolvedAttributeValue2, storageEntityContainerType, false, entityContainerMapping.GenerateUpdateViews);
                break;
              case "QueryView":
                if (!string.IsNullOrEmpty(resolvedAttributeValue2))
                {
                  MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_TableName_QueryView), resolvedAttributeValue1, MappingErrorCode.TableNameAttributeWithQueryView, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
                  return;
                }
                if (!this.LoadQueryView(nav.Clone(), (EntitySetBaseMapping) entitySetMapping))
                  return;
                break;
              default:
                MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_TypeMapping_QueryView, MappingErrorCode.InvalidContent, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
                break;
            }
          }
          while (nav.MoveToNext(XPathNodeType.Element));
        }
      }
      else
        this.LoadEntityTypeMapping(nav.Clone(), entitySetMapping, resolvedAttributeValue2, storageEntityContainerType, boolAttributeValue, entityContainerMapping.GenerateUpdateViews);
      this.ValidateAllEntityTypesHaveFunctionMapping(nav.Clone(), entitySetMapping);
      if (!entitySetMapping.HasNoContent)
        return;
      MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Emtpty_SetMap), entitySet.Name, MappingErrorCode.EmptySetMapping, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
    }

    private void ValidateAllEntityTypesHaveFunctionMapping(
      XPathNavigator nav,
      EntitySetMapping setMapping)
    {
      Set<EdmType> set1 = new Set<EdmType>();
      foreach (EntityTypeModificationFunctionMapping modificationFunctionMapping in setMapping.ModificationFunctionMappings)
        set1.Add((EdmType) modificationFunctionMapping.EntityType);
      if (0 >= set1.Count)
        return;
      Set<EdmType> set2 = new Set<EdmType>(MetadataHelper.GetTypeAndSubtypesOf((EdmType) setMapping.Set.ElementType, (ItemCollection) this.EdmItemCollection, false));
      set2.Subtract((IEnumerable<EdmType>) set1);
      Set<EdmType> set3 = new Set<EdmType>();
      foreach (System.Data.Entity.Core.Metadata.Edm.EntityType entityType in set2)
      {
        if (entityType.Abstract)
          set3.Add((EdmType) entityType);
      }
      set2.Subtract((IEnumerable<EdmType>) set3);
      if (0 >= set2.Count)
        return;
      MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_MissingEntityType), StringUtil.ToCommaSeparatedString((IEnumerable) set2), MappingErrorCode.MissingModificationFunctionMappingForEntityType, this.m_sourceLocation, (IXmlLineInfo) nav, (IList<EdmSchemaError>) this.m_parsingErrors);
    }

    private bool TryParseEntityTypeAttribute(
      XPathNavigator nav,
      System.Data.Entity.Core.Metadata.Edm.EntityType rootEntityType,
      Func<System.Data.Entity.Core.Metadata.Edm.EntityType, string> typeNotAssignableMessage,
      out Set<System.Data.Entity.Core.Metadata.Edm.EntityType> isOfTypeEntityTypes,
      out Set<System.Data.Entity.Core.Metadata.Edm.EntityType> entityTypes)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      string attributeValue = MappingItemLoader.GetAttributeValue(nav.Clone(), "TypeName");
      isOfTypeEntityTypes = new Set<System.Data.Entity.Core.Metadata.Edm.EntityType>();
      entityTypes = new Set<System.Data.Entity.Core.Metadata.Edm.EntityType>();
      foreach (string str1 in ((IEnumerable<string>) attributeValue.Split(';')).Select<string, string>((Func<string, string>) (s => s.Trim())))
      {
        bool flag = str1.StartsWith("IsTypeOf(", StringComparison.Ordinal);
        string aliasedString;
        if (flag)
        {
          if (!str1.EndsWith(")", StringComparison.Ordinal))
          {
            MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_InvalidContent_IsTypeOfNotTerminated, MappingErrorCode.InvalidEntityType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
            return false;
          }
          string str2 = str1.Substring("IsTypeOf(".Length);
          aliasedString = str2.Substring(0, str2.Length - ")".Length).Trim();
        }
        else
          aliasedString = str1;
        string aliasResolvedValue = this.GetAliasResolvedValue(aliasedString);
        System.Data.Entity.Core.Metadata.Edm.EntityType element;
        if (!this.EdmItemCollection.TryGetItem<System.Data.Entity.Core.Metadata.Edm.EntityType>(aliasResolvedValue, out element))
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Entity_Type), aliasResolvedValue, MappingErrorCode.InvalidEntityType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          return false;
        }
        if (!Helper.IsAssignableFrom((EdmType) rootEntityType, (EdmType) element))
        {
          MappingItemLoader.AddToSchemaErrorWithMessage(typeNotAssignableMessage(element), MappingErrorCode.InvalidEntityType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          return false;
        }
        if (element.Abstract)
        {
          if (flag)
          {
            if (!MetadataHelper.GetTypeAndSubtypesOf((EdmType) element, (ItemCollection) this.EdmItemCollection, false).GetEnumerator().MoveNext())
            {
              MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_AbstractEntity_IsOfType), element.FullName, MappingErrorCode.MappingOfAbstractType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
              return false;
            }
          }
          else
          {
            MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_AbstractEntity_Type), element.FullName, MappingErrorCode.MappingOfAbstractType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
            return false;
          }
        }
        if (flag)
          isOfTypeEntityTypes.Add(element);
        else
          entityTypes.Add(element);
      }
      return true;
    }

    private void LoadEntityTypeMapping(
      XPathNavigator nav,
      EntitySetMapping entitySetMapping,
      string tableName,
      System.Data.Entity.Core.Metadata.Edm.EntityContainer storageEntityContainerType,
      bool distinctFlagAboveType,
      bool generateUpdateViews)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      EntityTypeMapping entityTypeMapping = new EntityTypeMapping(entitySetMapping);
      System.Data.Entity.Core.Metadata.Edm.EntityType rootEntityType = (System.Data.Entity.Core.Metadata.Edm.EntityType) entitySetMapping.Set.ElementType;
      Set<System.Data.Entity.Core.Metadata.Edm.EntityType> isOfTypeEntityTypes;
      Set<System.Data.Entity.Core.Metadata.Edm.EntityType> entityTypes;
      if (!this.TryParseEntityTypeAttribute(nav.Clone(), rootEntityType, (Func<System.Data.Entity.Core.Metadata.Edm.EntityType, string>) (e => Strings.Mapping_InvalidContent_Entity_Type_For_Entity_Set((object) e.FullName, (object) rootEntityType.FullName, (object) entitySetMapping.Set.Name)), out isOfTypeEntityTypes, out entityTypes))
        return;
      foreach (System.Data.Entity.Core.Metadata.Edm.EntityType type in entityTypes)
        entityTypeMapping.AddType(type);
      foreach (System.Data.Entity.Core.Metadata.Edm.EntityType type in isOfTypeEntityTypes)
        entityTypeMapping.AddIsOfType(type);
      if (string.IsNullOrEmpty(tableName))
      {
        if (!nav.MoveToChild(XPathNodeType.Element))
          return;
        do
        {
          if (nav.LocalName == "ModificationFunctionMapping")
          {
            entitySetMapping.HasModificationFunctionMapping = true;
            this.LoadEntityTypeModificationFunctionMapping(nav.Clone(), entitySetMapping, entityTypeMapping);
          }
          else if (nav.LocalName != "MappingFragment")
          {
            MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_Table_Expected, MappingErrorCode.TableMappingFragmentExpected, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          }
          else
          {
            bool boolAttributeValue = MappingItemLoader.GetBoolAttributeValue(nav.Clone(), "MakeColumnsDistinct", false);
            if (generateUpdateViews && boolAttributeValue)
              MappingItemLoader.AddToSchemaErrors(Strings.Mapping_DistinctFlagInReadWriteContainer, MappingErrorCode.DistinctFragmentInReadWriteContainer, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
            tableName = this.GetAliasResolvedAttributeValue(nav.Clone(), "StoreEntitySet");
            MappingFragment fragment = this.LoadMappingFragment(nav.Clone(), entityTypeMapping, tableName, storageEntityContainerType, boolAttributeValue);
            if (fragment != null)
              entityTypeMapping.AddFragment(fragment);
          }
        }
        while (nav.MoveToNext(XPathNodeType.Element));
      }
      else
      {
        if (nav.LocalName == "ModificationFunctionMapping")
          MappingItemLoader.AddToSchemaErrors(Strings.Mapping_ModificationFunction_In_Table_Context, MappingErrorCode.InvalidTableNameAttributeWithModificationFunctionMapping, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        if (generateUpdateViews && distinctFlagAboveType)
          MappingItemLoader.AddToSchemaErrors(Strings.Mapping_DistinctFlagInReadWriteContainer, MappingErrorCode.DistinctFragmentInReadWriteContainer, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        MappingFragment fragment = this.LoadMappingFragment(nav.Clone(), entityTypeMapping, tableName, storageEntityContainerType, distinctFlagAboveType);
        if (fragment != null)
          entityTypeMapping.AddFragment(fragment);
      }
      entitySetMapping.AddTypeMapping(entityTypeMapping);
    }

    private void LoadEntityTypeModificationFunctionMapping(
      XPathNavigator nav,
      EntitySetMapping entitySetMapping,
      EntityTypeMapping entityTypeMapping)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      if (entityTypeMapping.IsOfTypes.Count != 0 || entityTypeMapping.Types.Count != 1)
      {
        MappingItemLoader.AddToSchemaErrors(Strings.Mapping_ModificationFunction_Multiple_Types, MappingErrorCode.InvalidModificationFunctionMappingForMultipleTypes, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
      else
      {
        System.Data.Entity.Core.Metadata.Edm.EntityType type = (System.Data.Entity.Core.Metadata.Edm.EntityType) entityTypeMapping.Types[0];
        if (type.Abstract)
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_AbstractEntity_FunctionMapping), type.FullName, MappingErrorCode.MappingOfAbstractType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        }
        else
        {
          foreach (EntityTypeModificationFunctionMapping modificationFunctionMapping in entitySetMapping.ModificationFunctionMappings)
          {
            if (modificationFunctionMapping.EntityType.Equals((object) type))
            {
              MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_RedundantEntityTypeMapping), type.Name, MappingErrorCode.RedundantEntityTypeMappingInModificationFunctionMapping, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
              return;
            }
          }
          MappingItemLoader.ModificationFunctionMappingLoader functionMappingLoader = new MappingItemLoader.ModificationFunctionMappingLoader(this, entitySetMapping.Set);
          ModificationFunctionMapping deleteFunctionMapping = (ModificationFunctionMapping) null;
          ModificationFunctionMapping insertFunctionMapping = (ModificationFunctionMapping) null;
          ModificationFunctionMapping updateFunctionMapping = (ModificationFunctionMapping) null;
          if (nav.MoveToChild(XPathNodeType.Element))
          {
            do
            {
              switch (nav.LocalName)
              {
                case "DeleteFunction":
                  deleteFunctionMapping = functionMappingLoader.LoadEntityTypeModificationFunctionMapping(nav.Clone(), entitySetMapping.Set, false, true, type);
                  break;
                case "InsertFunction":
                  insertFunctionMapping = functionMappingLoader.LoadEntityTypeModificationFunctionMapping(nav.Clone(), entitySetMapping.Set, true, false, type);
                  break;
                case "UpdateFunction":
                  updateFunctionMapping = functionMappingLoader.LoadEntityTypeModificationFunctionMapping(nav.Clone(), entitySetMapping.Set, true, true, type);
                  break;
              }
            }
            while (nav.MoveToNext(XPathNodeType.Element));
          }
          IEnumerable<ModificationFunctionParameterBinding> parameterBindings = (IEnumerable<ModificationFunctionParameterBinding>) new List<ModificationFunctionParameterBinding>();
          if (deleteFunctionMapping != null)
            parameterBindings = Helper.Concat<ModificationFunctionParameterBinding>(new IEnumerable<ModificationFunctionParameterBinding>[2]
            {
              parameterBindings,
              (IEnumerable<ModificationFunctionParameterBinding>) deleteFunctionMapping.ParameterBindings
            });
          if (insertFunctionMapping != null)
            parameterBindings = Helper.Concat<ModificationFunctionParameterBinding>(new IEnumerable<ModificationFunctionParameterBinding>[2]
            {
              parameterBindings,
              (IEnumerable<ModificationFunctionParameterBinding>) insertFunctionMapping.ParameterBindings
            });
          if (updateFunctionMapping != null)
            parameterBindings = Helper.Concat<ModificationFunctionParameterBinding>(new IEnumerable<ModificationFunctionParameterBinding>[2]
            {
              parameterBindings,
              (IEnumerable<ModificationFunctionParameterBinding>) updateFunctionMapping.ParameterBindings
            });
          Dictionary<AssociationSet, AssociationEndMember> dictionary = new Dictionary<AssociationSet, AssociationEndMember>();
          foreach (ModificationFunctionParameterBinding parameterBinding in parameterBindings)
          {
            if (parameterBinding.MemberPath.AssociationSetEnd != null)
            {
              AssociationSet parentAssociationSet = parameterBinding.MemberPath.AssociationSetEnd.ParentAssociationSet;
              AssociationEndMember associationEndMember1 = parameterBinding.MemberPath.AssociationSetEnd.CorrespondingAssociationEndMember;
              AssociationEndMember associationEndMember2;
              if (dictionary.TryGetValue(parentAssociationSet, out associationEndMember2) && associationEndMember2 != associationEndMember1)
              {
                MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_ModificationFunction_MultipleEndsOfAssociationMapped((object) associationEndMember1.Name, (object) associationEndMember2.Name, (object) parentAssociationSet.Name), MappingErrorCode.InvalidModificationFunctionMappingMultipleEndsOfAssociationMapped, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
                return;
              }
              dictionary[parentAssociationSet] = associationEndMember1;
            }
          }
          EntityTypeModificationFunctionMapping modificationFunctionMapping1 = new EntityTypeModificationFunctionMapping(type, deleteFunctionMapping, insertFunctionMapping, updateFunctionMapping);
          entitySetMapping.AddModificationFunctionMapping(modificationFunctionMapping1);
        }
      }
    }

    private bool LoadQueryView(XPathNavigator nav, EntitySetBaseMapping setMapping)
    {
      string viewString = nav.Value;
      string str = MappingItemLoader.GetAttributeValue(nav.Clone(), "TypeName");
      if (str != null)
        str = str.Trim();
      IXmlLineInfo lineInfo = nav as IXmlLineInfo;
      if (setMapping.QueryView == null)
      {
        if (str != null)
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo((Func<object, string>) (val => Strings.Mapping_TypeName_For_First_QueryView), setMapping.Set.Name, MappingErrorCode.TypeNameForFirstQueryView, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          return false;
        }
        if (string.IsNullOrEmpty(viewString))
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_Empty_QueryView), setMapping.Set.Name, MappingErrorCode.EmptyQueryView, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          return false;
        }
        setMapping.QueryView = viewString;
        this.m_hasQueryViews = true;
        return true;
      }
      if (str == null || str.Trim().Length == 0)
      {
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_QueryView_TypeName_Not_Defined), setMapping.Set.Name, MappingErrorCode.NoTypeNameForTypeSpecificQueryView, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return false;
      }
      System.Data.Entity.Core.Metadata.Edm.EntityType rootEntityType = (System.Data.Entity.Core.Metadata.Edm.EntityType) setMapping.Set.ElementType;
      Set<System.Data.Entity.Core.Metadata.Edm.EntityType> isOfTypeEntityTypes;
      Set<System.Data.Entity.Core.Metadata.Edm.EntityType> entityTypes;
      if (!this.TryParseEntityTypeAttribute(nav.Clone(), rootEntityType, (Func<System.Data.Entity.Core.Metadata.Edm.EntityType, string>) (e => Strings.Mapping_InvalidContent_Entity_Type_For_Entity_Set((object) e.FullName, (object) rootEntityType.FullName, (object) setMapping.Set.Name)), out isOfTypeEntityTypes, out entityTypes))
        return false;
      System.Data.Entity.Core.Metadata.Edm.EntityType entityType;
      bool second;
      if (isOfTypeEntityTypes.Count == 1)
      {
        entityType = isOfTypeEntityTypes.First<System.Data.Entity.Core.Metadata.Edm.EntityType>();
        second = true;
      }
      else if (entityTypes.Count == 1)
      {
        entityType = entityTypes.First<System.Data.Entity.Core.Metadata.Edm.EntityType>();
        second = false;
      }
      else
      {
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_QueryViewMultipleTypeInTypeName), setMapping.Set.ToString(), MappingErrorCode.TypeNameContainsMultipleTypesForQueryView, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return false;
      }
      if (second && setMapping.Set.ElementType.EdmEquals((MetadataItem) entityType))
      {
        MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_QueryView_For_Base_Type), entityType.ToString(), setMapping.Set.ToString(), MappingErrorCode.IsTypeOfQueryViewForBaseType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return false;
      }
      if (string.IsNullOrEmpty(viewString))
      {
        if (second)
        {
          MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_Empty_QueryView_OfType), entityType.Name, setMapping.Set.Name, MappingErrorCode.EmptyQueryView, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          return false;
        }
        MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_Empty_QueryView_OfTypeOnly), setMapping.Set.Name, entityType.Name, MappingErrorCode.EmptyQueryView, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return false;
      }
      Pair<EntitySetBase, Pair<EntityTypeBase, bool>> key = new Pair<EntitySetBase, Pair<EntityTypeBase, bool>>(setMapping.Set, new Pair<EntityTypeBase, bool>((EntityTypeBase) entityType, second));
      if (setMapping.ContainsTypeSpecificQueryView(key))
      {
        this.m_parsingErrors.Add(!second ? new EdmSchemaError(Strings.Mapping_QueryView_Duplicate_OfTypeOnly((object) setMapping.Set, (object) entityType), 2082, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition) : new EdmSchemaError(Strings.Mapping_QueryView_Duplicate_OfType((object) setMapping.Set, (object) entityType), 2082, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition));
        return false;
      }
      setMapping.AddTypeSpecificQueryView(key, viewString);
      return true;
    }

    private void LoadAssociationSetMapping(
      XPathNavigator nav,
      EntityContainerMapping entityContainerMapping,
      System.Data.Entity.Core.Metadata.Edm.EntityContainer storageEntityContainerType)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      string resolvedAttributeValue1 = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
      string resolvedAttributeValue2 = this.GetAliasResolvedAttributeValue(nav.Clone(), "TypeName");
      string resolvedAttributeValue3 = this.GetAliasResolvedAttributeValue(nav.Clone(), "StoreEntitySet");
      RelationshipSet relationshipSet;
      entityContainerMapping.EdmEntityContainer.TryGetRelationshipSetByName(resolvedAttributeValue1, false, out relationshipSet);
      AssociationSet associationSet = relationshipSet as AssociationSet;
      if (associationSet == null)
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Association_Set), resolvedAttributeValue1, MappingErrorCode.InvalidAssociationSet, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      else if (associationSet.ElementType.IsForeignKey)
      {
        IEnumerable<EdmMember> dependentKeys = (IEnumerable<EdmMember>) MetadataHelper.GetEntityTypeForEnd((AssociationEndMember) associationSet.ElementType.ReferentialConstraints.Single<System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint>().ToRole).KeyMembers;
        if (associationSet.ElementType.ReferentialConstraints.Single<System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint>().ToProperties.All<EdmProperty>((Func<EdmProperty, bool>) (p => dependentKeys.Contains<EdmMember>((EdmMember) p))))
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_ForeignKey_Association_Set_PKtoPK), resolvedAttributeValue1, MappingErrorCode.InvalidAssociationSet, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors).Severity = EdmSchemaErrorSeverity.Warning;
        else
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_ForeignKey_Association_Set), resolvedAttributeValue1, MappingErrorCode.InvalidAssociationSet, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
      else if (entityContainerMapping.ContainsAssociationSetMapping(associationSet))
      {
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_Duplicate_CdmAssociationSet_StorageMap), resolvedAttributeValue1, MappingErrorCode.DuplicateSetMapping, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
      else
      {
        AssociationSetMapping associationSetMapping = new AssociationSetMapping(associationSet, entityContainerMapping);
        associationSetMapping.StartLineNumber = lineInfo.LineNumber;
        associationSetMapping.StartLinePosition = lineInfo.LinePosition;
        if (!nav.MoveToChild(XPathNodeType.Element))
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Emtpty_SetMap), associationSet.Name, MappingErrorCode.EmptySetMapping, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        }
        else
        {
          entityContainerMapping.AddSetMapping(associationSetMapping);
          if (nav.LocalName == "QueryView")
          {
            if (!string.IsNullOrEmpty(resolvedAttributeValue3))
            {
              MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_TableName_QueryView), resolvedAttributeValue1, MappingErrorCode.TableNameAttributeWithQueryView, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
              return;
            }
            if (!this.LoadQueryView(nav.Clone(), (EntitySetBaseMapping) associationSetMapping) || !nav.MoveToNext(XPathNodeType.Element))
              return;
          }
          if (nav.LocalName == "EndProperty" || nav.LocalName == "ModificationFunctionMapping")
          {
            if (string.IsNullOrEmpty(resolvedAttributeValue2))
              MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_Association_Type_Empty, MappingErrorCode.InvalidAssociationType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
            else
              this.LoadAssociationTypeMapping(nav.Clone(), associationSetMapping, resolvedAttributeValue2, resolvedAttributeValue3, storageEntityContainerType);
          }
          else
          {
            if (!(nav.LocalName == "Condition"))
              return;
            MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_AssociationSet_Condition), resolvedAttributeValue1, MappingErrorCode.InvalidContent, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          }
        }
      }
    }

    private void LoadFunctionImportMapping(
      XPathNavigator nav,
      EntityContainerMapping entityContainerMapping)
    {
      IXmlLineInfo xmlLineInfo = (IXmlLineInfo) nav.Clone();
      EdmFunction targetFunction;
      EdmFunction functionImport;
      if (!this.TryGetFunctionImportStoreFunction(nav, out targetFunction) || !this.TryGetFunctionImportModelFunction(nav, entityContainerMapping, out functionImport))
        return;
      if (!functionImport.IsComposableAttribute && targetFunction.IsComposableAttribute)
        MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_TargetFunctionMustBeNonComposable((object) functionImport.FullName, (object) targetFunction.FullName), MappingErrorCode.MappingFunctionImportTargetFunctionMustBeNonComposable, this.m_sourceLocation, xmlLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      else if (functionImport.IsComposableAttribute && !targetFunction.IsComposableAttribute)
      {
        MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_TargetFunctionMustBeComposable((object) functionImport.FullName, (object) targetFunction.FullName), MappingErrorCode.MappingFunctionImportTargetFunctionMustBeComposable, this.m_sourceLocation, xmlLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
      else
      {
        this.ValidateFunctionImportMappingParameters(nav, targetFunction, functionImport);
        List<List<FunctionImportStructuralTypeMapping>> structuralTypeMappingsList = new List<List<FunctionImportStructuralTypeMapping>>();
        if (nav.MoveToChild(XPathNodeType.Element))
        {
          int resultSetIndex = 0;
          do
          {
            if (nav.LocalName == "ResultMapping")
            {
              List<FunctionImportStructuralTypeMapping> mappingResultMapping = this.GetFunctionImportMappingResultMapping(nav.Clone(), xmlLineInfo, functionImport, resultSetIndex);
              structuralTypeMappingsList.Add(mappingResultMapping);
            }
            ++resultSetIndex;
          }
          while (nav.MoveToNext(XPathNodeType.Element));
        }
        if (structuralTypeMappingsList.Count > 0 && structuralTypeMappingsList.Count != functionImport.ReturnParameters.Count)
          MappingItemLoader.AddToSchemaErrors(Strings.Mapping_FunctionImport_ResultMappingCountDoesNotMatchResultCount((object) functionImport.Identity), MappingErrorCode.FunctionResultMappingCountMismatch, this.m_sourceLocation, xmlLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        else if (functionImport.IsComposableAttribute)
        {
          EdmFunction ctypeFunction = this.StoreItemCollection.ConvertToCTypeFunction(targetFunction);
          RowType tvfReturnType1 = TypeHelpers.GetTvfReturnType(ctypeFunction);
          RowType tvfReturnType2 = TypeHelpers.GetTvfReturnType(targetFunction);
          if (tvfReturnType1 == null)
          {
            MappingItemLoader.AddToSchemaErrors(Strings.Mapping_FunctionImport_ResultMapping_InvalidSType((object) functionImport.Identity), MappingErrorCode.MappingFunctionImportTVFExpected, this.m_sourceLocation, xmlLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          }
          else
          {
            List<FunctionImportStructuralTypeMapping> typeMappings = structuralTypeMappingsList.Count > 0 ? structuralTypeMappingsList[0] : new List<FunctionImportStructuralTypeMapping>();
            FunctionImportMappingComposable mapping = (FunctionImportMappingComposable) null;
            EdmType returnType;
            if (MetadataHelper.TryGetFunctionImportReturnType<EdmType>(functionImport, 0, out returnType))
            {
              FunctionImportMappingComposableHelper composableHelper = new FunctionImportMappingComposableHelper(entityContainerMapping, this.m_sourceLocation, this.m_parsingErrors);
              if (Helper.IsStructuralType(returnType))
              {
                if (!composableHelper.TryCreateFunctionImportMappingComposableWithStructuralResult(functionImport, ctypeFunction, typeMappings, tvfReturnType1, tvfReturnType2, xmlLineInfo, out mapping))
                  return;
              }
              else if (!composableHelper.TryCreateFunctionImportMappingComposableWithScalarResult(functionImport, ctypeFunction, targetFunction, returnType, tvfReturnType1, xmlLineInfo, out mapping))
                return;
            }
            entityContainerMapping.AddFunctionImportMapping((FunctionImportMapping) mapping);
          }
        }
        else
        {
          FunctionImportMappingNonComposable mappingNonComposable = new FunctionImportMappingNonComposable(functionImport, targetFunction, structuralTypeMappingsList, (ItemCollection) this.EdmItemCollection);
          foreach (FunctionImportStructuralTypeMappingKB internalResultMapping in mappingNonComposable.InternalResultMappings)
            internalResultMapping.ValidateTypeConditions(false, (IList<EdmSchemaError>) this.m_parsingErrors, this.m_sourceLocation);
          for (int resultSetIndex = 0; resultSetIndex < mappingNonComposable.InternalResultMappings.Count; ++resultSetIndex)
          {
            System.Data.Entity.Core.Metadata.Edm.EntityType returnType;
            if (MetadataHelper.TryGetFunctionImportReturnType<System.Data.Entity.Core.Metadata.Edm.EntityType>(functionImport, resultSetIndex, out returnType) && returnType.Abstract && mappingNonComposable.GetResultMapping(resultSetIndex).NormalizedEntityTypeMappings.Count == 0)
              MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_FunctionImport_ImplicitMappingForAbstractReturnType), returnType.FullName, functionImport.Identity, MappingErrorCode.MappingOfAbstractType, this.m_sourceLocation, xmlLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          }
          entityContainerMapping.AddFunctionImportMapping((FunctionImportMapping) mappingNonComposable);
        }
      }
    }

    private bool TryGetFunctionImportStoreFunction(
      XPathNavigator nav,
      out EdmFunction targetFunction)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      targetFunction = (EdmFunction) null;
      string resolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "FunctionName");
      ReadOnlyCollection<EdmFunction> functions = this.StoreItemCollection.GetFunctions(resolvedAttributeValue);
      if (functions.Count == 0)
      {
        MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_StoreFunctionDoesNotExist((object) resolvedAttributeValue), MappingErrorCode.MappingFunctionImportStoreFunctionDoesNotExist, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return false;
      }
      if (functions.Count > 1)
      {
        MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_FunctionAmbiguous((object) resolvedAttributeValue), MappingErrorCode.MappingFunctionImportStoreFunctionAmbiguous, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return false;
      }
      targetFunction = functions.Single<EdmFunction>();
      return true;
    }

    private bool TryGetFunctionImportModelFunction(
      XPathNavigator nav,
      EntityContainerMapping entityContainerMapping,
      out EdmFunction functionImport)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      string resolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "FunctionImportName");
      System.Data.Entity.Core.Metadata.Edm.EntityContainer edmEntityContainer = entityContainerMapping.EdmEntityContainer;
      functionImport = (EdmFunction) null;
      foreach (EdmFunction functionImport1 in edmEntityContainer.FunctionImports)
      {
        if (functionImport1.Name == resolvedAttributeValue)
        {
          functionImport = functionImport1;
          break;
        }
      }
      if (functionImport == null)
      {
        MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_FunctionImportDoesNotExist((object) resolvedAttributeValue, (object) entityContainerMapping.EdmEntityContainer.Name), MappingErrorCode.MappingFunctionImportFunctionImportDoesNotExist, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return false;
      }
      FunctionImportMapping mapping;
      if (!entityContainerMapping.TryGetFunctionImportMapping(functionImport, out mapping))
        return true;
      MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_FunctionImportMappedMultipleTimes((object) resolvedAttributeValue), MappingErrorCode.MappingFunctionImportFunctionImportMappedMultipleTimes, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      return false;
    }

    private void ValidateFunctionImportMappingParameters(
      XPathNavigator nav,
      EdmFunction targetFunction,
      EdmFunction functionImport)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      foreach (FunctionParameter parameter in targetFunction.Parameters)
      {
        FunctionParameter functionParameter;
        if (!functionImport.Parameters.TryGetValue(parameter.Name, false, out functionParameter))
        {
          MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_TargetParameterHasNoCorrespondingImportParameter((object) parameter.Name), MappingErrorCode.MappingFunctionImportTargetParameterHasNoCorrespondingImportParameter, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        }
        else
        {
          if (parameter.Mode != functionParameter.Mode)
            MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_IncompatibleParameterMode((object) parameter.Name, (object) parameter.Mode, (object) functionParameter.Mode), MappingErrorCode.MappingFunctionImportIncompatibleParameterMode, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          PrimitiveType type = Helper.AsPrimitive(functionParameter.TypeUsage.EdmType);
          if (Helper.IsSpatialType(type))
            type = Helper.GetSpatialNormalizedPrimitiveType((EdmType) type);
          PrimitiveType edmType = (PrimitiveType) this.StoreItemCollection.ProviderManifest.GetEdmType(parameter.TypeUsage).EdmType;
          if (edmType == null)
          {
            MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_ProviderReturnsNullType((object) parameter.Name), MappingErrorCode.MappingStoreProviderReturnsNullEdmType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
            return;
          }
          if (edmType.PrimitiveTypeKind != type.PrimitiveTypeKind)
            MappingItemLoader.AddToSchemaErrorWithMessage(Helper.IsEnumType(functionParameter.TypeUsage.EdmType) ? Strings.Mapping_FunctionImport_IncompatibleEnumParameterType((object) parameter.Name, (object) edmType.Name, (object) functionParameter.TypeUsage.EdmType.FullName, (object) Helper.GetUnderlyingEdmTypeForEnumType(functionParameter.TypeUsage.EdmType).Name) : Strings.Mapping_FunctionImport_IncompatibleParameterType((object) parameter.Name, (object) edmType.Name, (object) type.Name), MappingErrorCode.MappingFunctionImportIncompatibleParameterType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        }
      }
      foreach (FunctionParameter parameter in functionImport.Parameters)
      {
        FunctionParameter functionParameter;
        if (!targetFunction.Parameters.TryGetValue(parameter.Name, false, out functionParameter))
          MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_ImportParameterHasNoCorrespondingTargetParameter((object) parameter.Name), MappingErrorCode.MappingFunctionImportImportParameterHasNoCorrespondingTargetParameter, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
    }

    private List<FunctionImportStructuralTypeMapping> GetFunctionImportMappingResultMapping(
      XPathNavigator nav,
      IXmlLineInfo functionImportMappingLineInfo,
      EdmFunction functionImport,
      int resultSetIndex)
    {
      List<FunctionImportStructuralTypeMapping> structuralTypeMappingList = new List<FunctionImportStructuralTypeMapping>();
      if (nav.MoveToChild(XPathNodeType.Element))
      {
        do
        {
          EntitySet entitySet = functionImport.EntitySets.Count > resultSetIndex ? functionImport.EntitySets[resultSetIndex] : (EntitySet) null;
          if (nav.LocalName == "EntityTypeMapping")
          {
            System.Data.Entity.Core.Metadata.Edm.EntityType resultEntityType;
            if (MetadataHelper.TryGetFunctionImportReturnType<System.Data.Entity.Core.Metadata.Edm.EntityType>(functionImport, resultSetIndex, out resultEntityType))
            {
              if (entitySet == null)
                MappingItemLoader.AddToSchemaErrors(Strings.Mapping_FunctionImport_EntityTypeMappingForFunctionNotReturningEntitySet((object) "EntityTypeMapping", (object) functionImport.Identity), MappingErrorCode.MappingFunctionImportEntityTypeMappingForFunctionNotReturningEntitySet, this.m_sourceLocation, functionImportMappingLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
              FunctionImportEntityTypeMapping typeMapping;
              if (this.TryLoadFunctionImportEntityTypeMapping(nav.Clone(), resultEntityType, (Func<System.Data.Entity.Core.Metadata.Edm.EntityType, string>) (e => Strings.Mapping_FunctionImport_InvalidContentEntityTypeForEntitySet((object) e.FullName, (object) resultEntityType.FullName, (object) entitySet.Name, (object) functionImport.Identity)), out typeMapping))
                structuralTypeMappingList.Add((FunctionImportStructuralTypeMapping) typeMapping);
            }
            else
              MappingItemLoader.AddToSchemaErrors(Strings.Mapping_FunctionImport_ResultMapping_InvalidCTypeETExpected((object) functionImport.Identity), MappingErrorCode.MappingFunctionImportUnexpectedEntityTypeMapping, this.m_sourceLocation, functionImportMappingLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          }
          else if (nav.LocalName == "ComplexTypeMapping")
          {
            ComplexType returnType;
            if (MetadataHelper.TryGetFunctionImportReturnType<ComplexType>(functionImport, resultSetIndex, out returnType))
            {
              FunctionImportComplexTypeMapping typeMapping;
              if (this.TryLoadFunctionImportComplexTypeMapping(nav.Clone(), returnType, functionImport, out typeMapping))
                structuralTypeMappingList.Add((FunctionImportStructuralTypeMapping) typeMapping);
            }
            else
              MappingItemLoader.AddToSchemaErrors(Strings.Mapping_FunctionImport_ResultMapping_InvalidCTypeCTExpected((object) functionImport.Identity), MappingErrorCode.MappingFunctionImportUnexpectedComplexTypeMapping, this.m_sourceLocation, functionImportMappingLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          }
        }
        while (nav.MoveToNext(XPathNodeType.Element));
      }
      return structuralTypeMappingList;
    }

    private bool TryLoadFunctionImportComplexTypeMapping(
      XPathNavigator nav,
      ComplexType resultComplexType,
      EdmFunction functionImport,
      out FunctionImportComplexTypeMapping typeMapping)
    {
      typeMapping = (FunctionImportComplexTypeMapping) null;
      LineInfo lineInfo = new LineInfo(nav);
      ComplexType complexType;
      if (!this.TryParseComplexTypeAttribute(nav, resultComplexType, functionImport, out complexType))
        return false;
      Collection<FunctionImportReturnTypePropertyMapping> collection = new Collection<FunctionImportReturnTypePropertyMapping>();
      if (!this.LoadFunctionImportStructuralType(nav.Clone(), (IEnumerable<StructuralType>) new List<StructuralType>()
      {
        (StructuralType) complexType
      }, collection, (List<FunctionImportEntityTypeMappingCondition>) null))
        return false;
      typeMapping = new FunctionImportComplexTypeMapping(complexType, collection, lineInfo);
      return true;
    }

    private bool TryParseComplexTypeAttribute(
      XPathNavigator nav,
      ComplexType resultComplexType,
      EdmFunction functionImport,
      out ComplexType complexType)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      string aliasResolvedValue = this.GetAliasResolvedValue(MappingItemLoader.GetAttributeValue(nav.Clone(), "TypeName"));
      if (!this.EdmItemCollection.TryGetItem<ComplexType>(aliasResolvedValue, out complexType))
      {
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Complex_Type), aliasResolvedValue, MappingErrorCode.InvalidComplexType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return false;
      }
      if (Helper.IsAssignableFrom((EdmType) resultComplexType, (EdmType) complexType))
        return true;
      MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_ResultMapping_MappedTypeDoesNotMatchReturnType((object) functionImport.Identity, (object) complexType.FullName), MappingErrorCode.InvalidComplexType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      return false;
    }

    private bool TryLoadFunctionImportEntityTypeMapping(
      XPathNavigator nav,
      System.Data.Entity.Core.Metadata.Edm.EntityType resultEntityType,
      Func<System.Data.Entity.Core.Metadata.Edm.EntityType, string> registerEntityTypeMismatchError,
      out FunctionImportEntityTypeMapping typeMapping)
    {
      typeMapping = (FunctionImportEntityTypeMapping) null;
      LineInfo lineInfo = new LineInfo(nav);
      MappingItemLoader.GetAttributeValue(nav.Clone(), "TypeName");
      Set<System.Data.Entity.Core.Metadata.Edm.EntityType> isOfTypeEntityTypes;
      Set<System.Data.Entity.Core.Metadata.Edm.EntityType> entityTypes;
      if (!this.TryParseEntityTypeAttribute(nav.Clone(), resultEntityType, registerEntityTypeMismatchError, out isOfTypeEntityTypes, out entityTypes))
        return false;
      IEnumerable<StructuralType> currentTypes = isOfTypeEntityTypes.Concat<System.Data.Entity.Core.Metadata.Edm.EntityType>((IEnumerable<System.Data.Entity.Core.Metadata.Edm.EntityType>) entityTypes).Distinct<System.Data.Entity.Core.Metadata.Edm.EntityType>().OfType<StructuralType>();
      Collection<FunctionImportReturnTypePropertyMapping> collection = new Collection<FunctionImportReturnTypePropertyMapping>();
      List<FunctionImportEntityTypeMappingCondition> conditions = new List<FunctionImportEntityTypeMappingCondition>();
      if (!this.LoadFunctionImportStructuralType(nav.Clone(), currentTypes, collection, conditions))
        return false;
      typeMapping = new FunctionImportEntityTypeMapping((IEnumerable<System.Data.Entity.Core.Metadata.Edm.EntityType>) isOfTypeEntityTypes, (IEnumerable<System.Data.Entity.Core.Metadata.Edm.EntityType>) entityTypes, (IEnumerable<FunctionImportEntityTypeMappingCondition>) conditions, collection, lineInfo);
      return true;
    }

    private bool LoadFunctionImportStructuralType(
      XPathNavigator nav,
      IEnumerable<StructuralType> currentTypes,
      Collection<FunctionImportReturnTypePropertyMapping> columnRenameMappings,
      List<FunctionImportEntityTypeMappingCondition> conditions)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav.Clone();
      if (nav.MoveToChild(XPathNodeType.Element))
      {
        do
        {
          if (nav.LocalName == "ScalarProperty")
            this.LoadFunctionImportStructuralTypeMappingScalarProperty(nav, columnRenameMappings, currentTypes);
          if (nav.LocalName == "Condition")
            this.LoadFunctionImportEntityTypeMappingCondition(nav, conditions);
        }
        while (nav.MoveToNext(XPathNodeType.Element));
      }
      bool flag = false;
      if (conditions != null)
      {
        HashSet<string> stringSet = new HashSet<string>();
        foreach (FunctionImportEntityTypeMappingCondition condition in conditions)
        {
          if (!stringSet.Add(condition.ColumnName))
          {
            MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_InvalidContent_Duplicate_Condition_Member((object) condition.ColumnName), MappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
            flag = true;
          }
        }
      }
      return !flag;
    }

    private void LoadFunctionImportStructuralTypeMappingScalarProperty(
      XPathNavigator nav,
      Collection<FunctionImportReturnTypePropertyMapping> columnRenameMappings,
      IEnumerable<StructuralType> currentTypes)
    {
      LineInfo lineInfo = new LineInfo(nav);
      string memberName = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
      string resolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "ColumnName");
      if (!currentTypes.All<StructuralType>((Func<StructuralType, bool>) (t => t.Members.Contains(memberName))))
        MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_InvalidContent_Cdm_Member((object) memberName), MappingErrorCode.InvalidEdmMember, this.m_sourceLocation, (IXmlLineInfo) lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      if (columnRenameMappings.Any<FunctionImportReturnTypePropertyMapping>((Func<FunctionImportReturnTypePropertyMapping, bool>) (m => m.CMember == memberName)))
        MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_InvalidContent_Duplicate_Cdm_Member((object) memberName), MappingErrorCode.DuplicateMemberMapping, this.m_sourceLocation, (IXmlLineInfo) lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      else
        columnRenameMappings.Add((FunctionImportReturnTypePropertyMapping) new FunctionImportReturnTypeScalarPropertyMapping(memberName, resolvedAttributeValue, lineInfo));
    }

    private void LoadFunctionImportEntityTypeMappingCondition(
      XPathNavigator nav,
      List<FunctionImportEntityTypeMappingCondition> conditions)
    {
      LineInfo lineInfo = new LineInfo(nav);
      string resolvedAttributeValue1 = this.GetAliasResolvedAttributeValue(nav.Clone(), "ColumnName");
      string resolvedAttributeValue2 = this.GetAliasResolvedAttributeValue(nav.Clone(), "Value");
      string resolvedAttributeValue3 = this.GetAliasResolvedAttributeValue(nav.Clone(), "IsNull");
      if (resolvedAttributeValue3 != null && resolvedAttributeValue2 != null)
        MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_ConditionMapping_Both_Values, MappingErrorCode.ConditionError, this.m_sourceLocation, (IXmlLineInfo) lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      else if (resolvedAttributeValue3 == null && resolvedAttributeValue2 == null)
        MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_ConditionMapping_Either_Values, MappingErrorCode.ConditionError, this.m_sourceLocation, (IXmlLineInfo) lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      else if (resolvedAttributeValue3 != null)
      {
        bool boolean = Convert.ToBoolean(resolvedAttributeValue3, (IFormatProvider) CultureInfo.InvariantCulture);
        conditions.Add((FunctionImportEntityTypeMappingCondition) new FunctionImportEntityTypeMappingConditionIsNull(resolvedAttributeValue1, boolean, lineInfo));
      }
      else
      {
        XPathNavigator columnValue = nav.Clone();
        columnValue.MoveToAttribute("Value", string.Empty);
        conditions.Add((FunctionImportEntityTypeMappingCondition) new FunctionImportEntityTypeMappingConditionValue(resolvedAttributeValue1, columnValue, lineInfo));
      }
    }

    private void LoadAssociationTypeMapping(
      XPathNavigator nav,
      AssociationSetMapping associationSetMapping,
      string associationTypeName,
      string tableName,
      System.Data.Entity.Core.Metadata.Edm.EntityContainer storageEntityContainerType)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      AssociationType relation;
      this.EdmItemCollection.TryGetItem<AssociationType>(associationTypeName, out relation);
      if (relation == null)
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Association_Type), associationTypeName, MappingErrorCode.InvalidAssociationType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      else if (!associationSetMapping.Set.ElementType.Equals((object) relation))
      {
        MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_Invalid_Association_Type_For_Association_Set((object) associationTypeName, (object) associationSetMapping.Set.ElementType.FullName, (object) associationSetMapping.Set.Name), MappingErrorCode.DuplicateTypeMapping, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
      else
      {
        AssociationTypeMapping typeMapping = new AssociationTypeMapping(relation, associationSetMapping);
        associationSetMapping.AssociationTypeMapping = typeMapping;
        if (string.IsNullOrEmpty(tableName) && associationSetMapping.QueryView == null)
        {
          MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_Table_Expected, MappingErrorCode.InvalidTable, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        }
        else
        {
          MappingFragment mappingFragment = this.LoadAssociationMappingFragment(nav.Clone(), associationSetMapping, typeMapping, tableName, storageEntityContainerType);
          if (mappingFragment == null)
            return;
          typeMapping.MappingFragment = mappingFragment;
        }
      }
    }

    private void LoadAssociationTypeModificationFunctionMapping(
      XPathNavigator nav,
      AssociationSetMapping associationSetMapping)
    {
      MappingItemLoader.ModificationFunctionMappingLoader functionMappingLoader = new MappingItemLoader.ModificationFunctionMappingLoader(this, associationSetMapping.Set);
      ModificationFunctionMapping deleteFunctionMapping = (ModificationFunctionMapping) null;
      ModificationFunctionMapping insertFunctionMapping = (ModificationFunctionMapping) null;
      if (nav.MoveToChild(XPathNodeType.Element))
      {
        do
        {
          switch (nav.LocalName)
          {
            case "DeleteFunction":
              deleteFunctionMapping = functionMappingLoader.LoadAssociationSetModificationFunctionMapping(nav.Clone(), associationSetMapping.Set, false);
              break;
            case "InsertFunction":
              insertFunctionMapping = functionMappingLoader.LoadAssociationSetModificationFunctionMapping(nav.Clone(), associationSetMapping.Set, true);
              break;
          }
        }
        while (nav.MoveToNext(XPathNodeType.Element));
      }
      associationSetMapping.ModificationFunctionMapping = new AssociationSetModificationFunctionMapping((AssociationSet) associationSetMapping.Set, deleteFunctionMapping, insertFunctionMapping);
    }

    private MappingFragment LoadMappingFragment(
      XPathNavigator nav,
      EntityTypeMapping typeMapping,
      string tableName,
      System.Data.Entity.Core.Metadata.Edm.EntityContainer storageEntityContainerType,
      bool distinctFlag)
    {
      IXmlLineInfo navLineInfo = (IXmlLineInfo) nav;
      if (typeMapping.SetMapping.QueryView != null)
      {
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_QueryView_PropertyMaps), typeMapping.SetMapping.Set.Name, MappingErrorCode.PropertyMapsWithQueryView, this.m_sourceLocation, navLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return (MappingFragment) null;
      }
      EntitySet entitySet;
      storageEntityContainerType.TryGetEntitySetByName(tableName, false, out entitySet);
      if (entitySet == null)
      {
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Table), tableName, MappingErrorCode.InvalidTable, this.m_sourceLocation, navLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return (MappingFragment) null;
      }
      System.Data.Entity.Core.Metadata.Edm.EntityType elementType = entitySet.ElementType;
      MappingFragment mappingFragment = new MappingFragment(entitySet, (TypeMapping) typeMapping, distinctFlag);
      mappingFragment.StartLineNumber = navLineInfo.LineNumber;
      mappingFragment.StartLinePosition = navLineInfo.LinePosition;
      if (nav.MoveToChild(XPathNodeType.Element))
      {
        do
        {
          EdmType containerType = (EdmType) null;
          string attributeValue = MappingItemLoader.GetAttributeValue(nav.Clone(), "Name");
          if (attributeValue != null)
            containerType = (EdmType) typeMapping.GetContainerType(attributeValue);
          switch (nav.LocalName)
          {
            case "ScalarProperty":
              ScalarPropertyMapping scalarPropertyMapping = this.LoadScalarPropertyMapping(nav.Clone(), containerType, elementType.Properties);
              if (scalarPropertyMapping != null)
              {
                mappingFragment.AddPropertyMapping((PropertyMapping) scalarPropertyMapping);
                break;
              }
              break;
            case "ComplexProperty":
              ComplexPropertyMapping complexPropertyMapping = this.LoadComplexPropertyMapping(nav.Clone(), containerType, elementType.Properties);
              if (complexPropertyMapping != null)
              {
                mappingFragment.AddPropertyMapping((PropertyMapping) complexPropertyMapping);
                break;
              }
              break;
            case "Condition":
              ConditionPropertyMapping conditionPropertyMap = this.LoadConditionPropertyMapping(nav.Clone(), containerType, elementType.Properties);
              if (conditionPropertyMap != null)
              {
                mappingFragment.AddConditionProperty(conditionPropertyMap, (System.Action<EdmMember>) (member => MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Duplicate_Condition_Member), member.Name, MappingErrorCode.ConditionError, this.m_sourceLocation, navLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors)));
                break;
              }
              break;
            default:
              MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_General, MappingErrorCode.InvalidContent, this.m_sourceLocation, navLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
              break;
          }
        }
        while (nav.MoveToNext(XPathNodeType.Element));
      }
      nav.MoveToChild(XPathNodeType.Element);
      return mappingFragment;
    }

    private MappingFragment LoadAssociationMappingFragment(
      XPathNavigator nav,
      AssociationSetMapping setMapping,
      AssociationTypeMapping typeMapping,
      string tableName,
      System.Data.Entity.Core.Metadata.Edm.EntityContainer storageEntityContainerType)
    {
      IXmlLineInfo navLineInfo = (IXmlLineInfo) nav;
      MappingFragment mappingFragment = (MappingFragment) null;
      System.Data.Entity.Core.Metadata.Edm.EntityType tableType = (System.Data.Entity.Core.Metadata.Edm.EntityType) null;
      if (setMapping.QueryView == null)
      {
        EntitySet entitySet;
        storageEntityContainerType.TryGetEntitySetByName(tableName, false, out entitySet);
        if (entitySet == null)
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Table), tableName, MappingErrorCode.InvalidTable, this.m_sourceLocation, navLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          return (MappingFragment) null;
        }
        tableType = entitySet.ElementType;
        mappingFragment = new MappingFragment(entitySet, (TypeMapping) typeMapping, false);
        mappingFragment.StartLineNumber = setMapping.StartLineNumber;
        mappingFragment.StartLinePosition = setMapping.StartLinePosition;
      }
      do
      {
        switch (nav.LocalName)
        {
          case "EndProperty":
            if (setMapping.QueryView != null)
            {
              MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_QueryView_PropertyMaps), setMapping.Set.Name, MappingErrorCode.PropertyMapsWithQueryView, this.m_sourceLocation, navLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
              return (MappingFragment) null;
            }
            string resolvedAttributeValue = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
            EdmMember edmMember = (EdmMember) null;
            typeMapping.AssociationType.Members.TryGetValue(resolvedAttributeValue, false, out edmMember);
            AssociationEndMember end = edmMember as AssociationEndMember;
            if (end == null)
            {
              MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_End), resolvedAttributeValue, MappingErrorCode.InvalidEdmMember, this.m_sourceLocation, navLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
              break;
            }
            mappingFragment.AddPropertyMapping((PropertyMapping) this.LoadEndPropertyMapping(nav.Clone(), end, tableType));
            break;
          case "Condition":
            if (setMapping.QueryView != null)
            {
              MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_QueryView_PropertyMaps), setMapping.Set.Name, MappingErrorCode.PropertyMapsWithQueryView, this.m_sourceLocation, navLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
              return (MappingFragment) null;
            }
            ConditionPropertyMapping conditionPropertyMap = this.LoadConditionPropertyMapping(nav.Clone(), (EdmType) null, tableType.Properties);
            if (conditionPropertyMap != null)
            {
              mappingFragment.AddConditionProperty(conditionPropertyMap, (System.Action<EdmMember>) (member => MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Duplicate_Condition_Member), member.Name, MappingErrorCode.ConditionError, this.m_sourceLocation, navLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors)));
              break;
            }
            break;
          case "ModificationFunctionMapping":
            setMapping.HasModificationFunctionMapping = true;
            this.LoadAssociationTypeModificationFunctionMapping(nav.Clone(), setMapping);
            break;
          default:
            MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_General, MappingErrorCode.InvalidContent, this.m_sourceLocation, navLineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
            break;
        }
      }
      while (nav.MoveToNext(XPathNodeType.Element));
      return mappingFragment;
    }

    private ScalarPropertyMapping LoadScalarPropertyMapping(
      XPathNavigator nav,
      EdmType containerType,
      ReadOnlyMetadataCollection<EdmProperty> tableProperties)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      string resolvedAttributeValue1 = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
      EdmProperty edmProperty1 = (EdmProperty) null;
      if (!string.IsNullOrEmpty(resolvedAttributeValue1) && (containerType == null || !Helper.IsCollectionType((GlobalItem) containerType)))
      {
        if (containerType != null)
        {
          if (Helper.IsRefType((GlobalItem) containerType))
          {
            ((System.Data.Entity.Core.Metadata.Edm.EntityType) ((RefType) containerType).ElementType).Properties.TryGetValue(resolvedAttributeValue1, false, out edmProperty1);
          }
          else
          {
            EdmMember edmMember;
            (containerType as StructuralType).Members.TryGetValue(resolvedAttributeValue1, false, out edmMember);
            edmProperty1 = edmMember as EdmProperty;
          }
        }
        if (edmProperty1 == null)
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Cdm_Member), resolvedAttributeValue1, MappingErrorCode.InvalidEdmMember, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
      string resolvedAttributeValue2 = this.GetAliasResolvedAttributeValue(nav.Clone(), "ColumnName");
      EdmProperty edmProperty2;
      tableProperties.TryGetValue(resolvedAttributeValue2, false, out edmProperty2);
      if (edmProperty2 == null)
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Column), resolvedAttributeValue2, MappingErrorCode.InvalidStorageMember, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      if (edmProperty1 == null || edmProperty2 == null)
        return (ScalarPropertyMapping) null;
      if (!Helper.IsScalarType(edmProperty1.TypeUsage.EdmType))
      {
        this.m_parsingErrors.Add(new EdmSchemaError(Strings.Mapping_Invalid_CSide_ScalarProperty((object) edmProperty1.Name), 2085, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition));
        return (ScalarPropertyMapping) null;
      }
      this.ValidateAndUpdateScalarMemberMapping(edmProperty1, edmProperty2, lineInfo);
      return new ScalarPropertyMapping(edmProperty1, edmProperty2);
    }

    private ComplexPropertyMapping LoadComplexPropertyMapping(
      XPathNavigator nav,
      EdmType containerType,
      ReadOnlyMetadataCollection<EdmProperty> tableProperties)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      CollectionType collectionType = containerType as CollectionType;
      string resolvedAttributeValue1 = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
      EdmProperty property = (EdmProperty) null;
      EdmType type = (EdmType) null;
      string resolvedAttributeValue2 = this.GetAliasResolvedAttributeValue(nav.Clone(), "TypeName");
      StructuralType structuralType = containerType as StructuralType;
      if (string.IsNullOrEmpty(resolvedAttributeValue2))
      {
        if (collectionType == null)
        {
          if (structuralType != null)
          {
            EdmMember edmMember;
            structuralType.Members.TryGetValue(resolvedAttributeValue1, false, out edmMember);
            property = edmMember as EdmProperty;
            if (property == null)
              MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Cdm_Member), resolvedAttributeValue1, MappingErrorCode.InvalidEdmMember, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
            type = property.TypeUsage.EdmType;
          }
          else
            MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Cdm_Member), resolvedAttributeValue1, MappingErrorCode.InvalidEdmMember, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        }
        else
          type = collectionType.TypeUsage.EdmType;
      }
      else
      {
        if (containerType != null)
        {
          EdmMember edmMember;
          structuralType.Members.TryGetValue(resolvedAttributeValue1, false, out edmMember);
          property = edmMember as EdmProperty;
        }
        if (property == null)
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Cdm_Member), resolvedAttributeValue1, MappingErrorCode.InvalidEdmMember, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        this.EdmItemCollection.TryGetItem<EdmType>(resolvedAttributeValue2, out type);
        type = (EdmType) (type as ComplexType);
        if (type == null)
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Complex_Type), resolvedAttributeValue2, MappingErrorCode.InvalidComplexType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
      ComplexPropertyMapping complexPropertyMapping = new ComplexPropertyMapping(property);
      XPathNavigator xpathNavigator = nav.Clone();
      bool flag = false;
      if (xpathNavigator.MoveToChild(XPathNodeType.Element) && xpathNavigator.LocalName == "ComplexTypeMapping")
        flag = true;
      if (property == null || type == null)
        return (ComplexPropertyMapping) null;
      if (flag)
      {
        nav.MoveToChild(XPathNodeType.Element);
        do
        {
          complexPropertyMapping.AddTypeMapping(this.LoadComplexTypeMapping(nav.Clone(), (EdmType) null, tableProperties));
        }
        while (nav.MoveToNext(XPathNodeType.Element));
      }
      else
        complexPropertyMapping.AddTypeMapping(this.LoadComplexTypeMapping(nav.Clone(), type, tableProperties));
      return complexPropertyMapping;
    }

    private ComplexTypeMapping LoadComplexTypeMapping(
      XPathNavigator nav,
      EdmType type,
      ReadOnlyMetadataCollection<EdmProperty> tableType)
    {
      bool isPartial = false;
      string attributeValue = MappingItemLoader.GetAttributeValue(nav.Clone(), "IsPartial");
      if (!string.IsNullOrEmpty(attributeValue))
        isPartial = Convert.ToBoolean(attributeValue, (IFormatProvider) CultureInfo.InvariantCulture);
      ComplexTypeMapping complexTypeMapping = new ComplexTypeMapping(isPartial);
      if (type != null)
      {
        complexTypeMapping.AddType(type as ComplexType);
      }
      else
      {
        string str1 = this.GetAliasResolvedAttributeValue(nav.Clone(), "TypeName");
        int length = str1.IndexOf(';');
        do
        {
          string aliasedString;
          if (length != -1)
          {
            aliasedString = str1.Substring(0, length);
            str1 = str1.Substring(length + 1, str1.Length - (length + 1));
          }
          else
          {
            aliasedString = str1;
            str1 = string.Empty;
          }
          int num = aliasedString.IndexOf("IsTypeOf(", StringComparison.Ordinal);
          string str2 = num != 0 ? this.GetAliasResolvedValue(aliasedString) : this.GetAliasResolvedValue(aliasedString.Substring("IsTypeOf(".Length, aliasedString.Length - ("IsTypeOf(".Length + 1)));
          ComplexType type1;
          this.EdmItemCollection.TryGetItem<ComplexType>(str2, out type1);
          if (type1 == null)
          {
            MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Complex_Type), str2, MappingErrorCode.InvalidComplexType, this.m_sourceLocation, (IXmlLineInfo) nav, (IList<EdmSchemaError>) this.m_parsingErrors);
            length = str1.IndexOf(';');
          }
          else
          {
            if (num == 0)
              complexTypeMapping.AddIsOfType(type1);
            else
              complexTypeMapping.AddType(type1);
            length = str1.IndexOf(';');
          }
        }
        while (str1.Length != 0);
      }
      if (nav.MoveToChild(XPathNodeType.Element))
      {
        do
        {
          EdmType ownerType = (EdmType) complexTypeMapping.GetOwnerType(MappingItemLoader.GetAttributeValue(nav.Clone(), "Name"));
          switch (nav.LocalName)
          {
            case "ScalarProperty":
              ScalarPropertyMapping scalarPropertyMapping = this.LoadScalarPropertyMapping(nav.Clone(), ownerType, tableType);
              if (scalarPropertyMapping != null)
              {
                complexTypeMapping.AddPropertyMapping((PropertyMapping) scalarPropertyMapping);
                break;
              }
              break;
            case "ComplexProperty":
              ComplexPropertyMapping complexPropertyMapping = this.LoadComplexPropertyMapping(nav.Clone(), ownerType, tableType);
              if (complexPropertyMapping != null)
              {
                complexTypeMapping.AddPropertyMapping((PropertyMapping) complexPropertyMapping);
                break;
              }
              break;
            case "Condition":
              ConditionPropertyMapping conditionPropertyMap = this.LoadConditionPropertyMapping(nav.Clone(), ownerType, tableType);
              if (conditionPropertyMap != null)
              {
                complexTypeMapping.AddConditionProperty(conditionPropertyMap, (System.Action<EdmMember>) (member => MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_Duplicate_Condition_Member), member.Name, MappingErrorCode.ConditionError, this.m_sourceLocation, (IXmlLineInfo) nav, (IList<EdmSchemaError>) this.m_parsingErrors)));
                break;
              }
              break;
            default:
              throw Error.NotSupported();
          }
        }
        while (nav.MoveToNext(XPathNodeType.Element));
      }
      return complexTypeMapping;
    }

    private EndPropertyMapping LoadEndPropertyMapping(
      XPathNavigator nav,
      AssociationEndMember end,
      System.Data.Entity.Core.Metadata.Edm.EntityType tableType)
    {
      EndPropertyMapping endPropertyMapping = new EndPropertyMapping()
      {
        AssociationEnd = end
      };
      nav.MoveToChild(XPathNodeType.Element);
      do
      {
        switch (nav.LocalName)
        {
          case "ScalarProperty":
            EntityTypeBase elementType = (end.TypeUsage.EdmType as RefType).ElementType;
            ScalarPropertyMapping propertyMapping = this.LoadScalarPropertyMapping(nav.Clone(), (EdmType) elementType, tableType.Properties);
            if (propertyMapping != null)
            {
              if (!elementType.KeyMembers.Contains((EdmMember) propertyMapping.Property))
              {
                IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
                MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_EndProperty), propertyMapping.Property.Name, MappingErrorCode.InvalidEdmMember, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
                return (EndPropertyMapping) null;
              }
              endPropertyMapping.AddPropertyMapping(propertyMapping);
              break;
            }
            break;
        }
      }
      while (nav.MoveToNext(XPathNodeType.Element));
      return endPropertyMapping;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    private ConditionPropertyMapping LoadConditionPropertyMapping(
      XPathNavigator nav,
      EdmType containerType,
      ReadOnlyMetadataCollection<EdmProperty> tableProperties)
    {
      string resolvedAttributeValue1 = this.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
      string resolvedAttributeValue2 = this.GetAliasResolvedAttributeValue(nav.Clone(), "ColumnName");
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      if (resolvedAttributeValue1 != null && resolvedAttributeValue2 != null)
      {
        MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_ConditionMapping_Both_Members, MappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return (ConditionPropertyMapping) null;
      }
      if (resolvedAttributeValue1 == null && resolvedAttributeValue2 == null)
      {
        MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_ConditionMapping_Either_Members, MappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return (ConditionPropertyMapping) null;
      }
      EdmProperty edmProperty1 = (EdmProperty) null;
      if (resolvedAttributeValue1 != null && containerType != null)
      {
        EdmMember edmMember;
        ((StructuralType) containerType).Members.TryGetValue(resolvedAttributeValue1, false, out edmMember);
        edmProperty1 = edmMember as EdmProperty;
      }
      EdmProperty edmProperty2 = (EdmProperty) null;
      if (resolvedAttributeValue2 != null)
        tableProperties.TryGetValue(resolvedAttributeValue2, false, out edmProperty2);
      EdmProperty propertyOrColumn = edmProperty2 ?? edmProperty1;
      if (propertyOrColumn == null)
      {
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_ConditionMapping_InvalidMember), resolvedAttributeValue2 ?? resolvedAttributeValue1, MappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return (ConditionPropertyMapping) null;
      }
      bool? nullable = new bool?();
      object obj = (object) null;
      string attributeValue = MappingItemLoader.GetAttributeValue(nav.Clone(), "IsNull");
      EdmType edmType1 = propertyOrColumn.TypeUsage.EdmType;
      if (Helper.IsPrimitiveType(edmType1))
      {
        TypeUsage typeUsage;
        if (propertyOrColumn.DeclaringType.DataSpace == DataSpace.SSpace)
        {
          typeUsage = this.StoreItemCollection.ProviderManifest.GetEdmType(propertyOrColumn.TypeUsage);
          if (typeUsage == null)
          {
            MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_ProviderReturnsNullType((object) propertyOrColumn.Name), MappingErrorCode.MappingStoreProviderReturnsNullEdmType, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
            return (ConditionPropertyMapping) null;
          }
        }
        else
          typeUsage = propertyOrColumn.TypeUsage;
        PrimitiveType edmType2 = (PrimitiveType) typeUsage.EdmType;
        Type clrEquivalentType = edmType2.ClrEquivalentType;
        PrimitiveTypeKind primitiveTypeKind = edmType2.PrimitiveTypeKind;
        if (attributeValue == null && !MappingItemLoader.IsTypeSupportedForCondition(primitiveTypeKind))
        {
          MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_InvalidContent_ConditionMapping_InvalidPrimitiveTypeKind), propertyOrColumn.Name, edmType1.FullName, MappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
          return (ConditionPropertyMapping) null;
        }
        if (!MappingItemLoader.TryGetTypedAttributeValue(nav.Clone(), "Value", clrEquivalentType, this.m_sourceLocation, (IList<EdmSchemaError>) this.m_parsingErrors, out obj))
          return (ConditionPropertyMapping) null;
      }
      else if (Helper.IsEnumType(edmType1))
      {
        obj = (object) MappingItemLoader.GetEnumAttributeValue(nav.Clone(), "Value", (EnumType) edmType1, this.m_sourceLocation, (IList<EdmSchemaError>) this.m_parsingErrors);
      }
      else
      {
        MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_ConditionMapping_NonScalar, MappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return (ConditionPropertyMapping) null;
      }
      if (attributeValue != null && obj != null)
      {
        MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_ConditionMapping_Both_Values, MappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return (ConditionPropertyMapping) null;
      }
      if (attributeValue == null && obj == null)
      {
        MappingItemLoader.AddToSchemaErrors(Strings.Mapping_InvalidContent_ConditionMapping_Either_Values, MappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return (ConditionPropertyMapping) null;
      }
      if (attributeValue != null)
        nullable = new bool?(Convert.ToBoolean(attributeValue, (IFormatProvider) CultureInfo.InvariantCulture));
      if (edmProperty2 != null && (edmProperty2.IsStoreGeneratedComputed || edmProperty2.IsStoreGeneratedIdentity))
      {
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_InvalidContent_ConditionMapping_Computed), edmProperty2.Name, MappingErrorCode.ConditionError, this.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parsingErrors);
        return (ConditionPropertyMapping) null;
      }
      if (obj == null)
        return (ConditionPropertyMapping) new IsNullConditionMapping(propertyOrColumn, nullable.Value);
      return (ConditionPropertyMapping) new ValueConditionMapping(propertyOrColumn, obj);
    }

    internal static bool IsTypeSupportedForCondition(PrimitiveTypeKind primitiveTypeKind)
    {
      switch (primitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
        case PrimitiveTypeKind.DateTime:
        case PrimitiveTypeKind.Decimal:
        case PrimitiveTypeKind.Double:
        case PrimitiveTypeKind.Guid:
        case PrimitiveTypeKind.Single:
        case PrimitiveTypeKind.Time:
        case PrimitiveTypeKind.DateTimeOffset:
          return false;
        case PrimitiveTypeKind.Boolean:
        case PrimitiveTypeKind.Byte:
        case PrimitiveTypeKind.SByte:
        case PrimitiveTypeKind.Int16:
        case PrimitiveTypeKind.Int32:
        case PrimitiveTypeKind.Int64:
        case PrimitiveTypeKind.String:
          return true;
        default:
          return false;
      }
    }

    private static XmlSchemaSet GetOrCreateSchemaSet()
    {
      if (MappingItemLoader.s_mappingXmlSchema == null)
      {
        XmlSchemaSet set = new XmlSchemaSet();
        MappingItemLoader.AddResourceXsdToSchemaSet(set, "System.Data.Resources.CSMSL_1.xsd");
        MappingItemLoader.AddResourceXsdToSchemaSet(set, "System.Data.Resources.CSMSL_2.xsd");
        MappingItemLoader.AddResourceXsdToSchemaSet(set, "System.Data.Resources.CSMSL_3.xsd");
        Interlocked.CompareExchange<XmlSchemaSet>(ref MappingItemLoader.s_mappingXmlSchema, set, (XmlSchemaSet) null);
      }
      return MappingItemLoader.s_mappingXmlSchema;
    }

    private static void AddResourceXsdToSchemaSet(XmlSchemaSet set, string resourceName)
    {
      using (XmlReader xmlResource = DbProviderServices.GetXmlResource(resourceName))
      {
        XmlSchema schema = XmlSchema.Read(xmlResource, (ValidationEventHandler) null);
        set.Add(schema);
      }
    }

    internal static void AddToSchemaErrors(
      string message,
      MappingErrorCode errorCode,
      string location,
      IXmlLineInfo lineInfo,
      IList<EdmSchemaError> parsingErrors)
    {
      EdmSchemaError edmSchemaError = new EdmSchemaError(message, (int) errorCode, EdmSchemaErrorSeverity.Error, location, lineInfo.LineNumber, lineInfo.LinePosition);
      parsingErrors.Add(edmSchemaError);
    }

    internal static EdmSchemaError AddToSchemaErrorsWithMemberInfo(
      Func<object, string> messageFormat,
      string errorMember,
      MappingErrorCode errorCode,
      string location,
      IXmlLineInfo lineInfo,
      IList<EdmSchemaError> parsingErrors)
    {
      EdmSchemaError edmSchemaError = new EdmSchemaError(messageFormat((object) errorMember), (int) errorCode, EdmSchemaErrorSeverity.Error, location, lineInfo.LineNumber, lineInfo.LinePosition);
      parsingErrors.Add(edmSchemaError);
      return edmSchemaError;
    }

    internal static void AddToSchemaErrorWithMemberAndStructure(
      Func<object, object, string> messageFormat,
      string errorMember,
      string errorStructure,
      MappingErrorCode errorCode,
      string location,
      IXmlLineInfo lineInfo,
      IList<EdmSchemaError> parsingErrors)
    {
      EdmSchemaError edmSchemaError = new EdmSchemaError(messageFormat((object) errorMember, (object) errorStructure), (int) errorCode, EdmSchemaErrorSeverity.Error, location, lineInfo.LineNumber, lineInfo.LinePosition);
      parsingErrors.Add(edmSchemaError);
    }

    private static void AddToSchemaErrorWithMessage(
      string errorMessage,
      MappingErrorCode errorCode,
      string location,
      IXmlLineInfo lineInfo,
      IList<EdmSchemaError> parsingErrors)
    {
      EdmSchemaError edmSchemaError = new EdmSchemaError(errorMessage, (int) errorCode, EdmSchemaErrorSeverity.Error, location, lineInfo.LineNumber, lineInfo.LinePosition);
      parsingErrors.Add(edmSchemaError);
    }

    private string GetAliasResolvedAttributeValue(XPathNavigator nav, string attributeName)
    {
      return this.GetAliasResolvedValue(MappingItemLoader.GetAttributeValue(nav, attributeName));
    }

    private static bool GetBoolAttributeValue(
      XPathNavigator nav,
      string attributeName,
      bool defaultValue)
    {
      bool flag = defaultValue;
      object typedAttributeValue = Helper.GetTypedAttributeValue(nav, attributeName, typeof (bool));
      if (typedAttributeValue != null)
        flag = (bool) typedAttributeValue;
      return flag;
    }

    private static string GetAttributeValue(XPathNavigator nav, string attributeName)
    {
      return Helper.GetAttributeValue(nav, attributeName);
    }

    private static bool TryGetTypedAttributeValue(
      XPathNavigator nav,
      string attributeName,
      Type clrType,
      string sourceLocation,
      IList<EdmSchemaError> parsingErrors,
      out object value)
    {
      value = (object) null;
      try
      {
        value = Helper.GetTypedAttributeValue(nav, attributeName, clrType);
      }
      catch (FormatException ex)
      {
        MappingItemLoader.AddToSchemaErrors(Strings.Mapping_ConditionValueTypeMismatch, MappingErrorCode.ConditionError, sourceLocation, (IXmlLineInfo) nav, parsingErrors);
        return false;
      }
      return true;
    }

    private static EnumMember GetEnumAttributeValue(
      XPathNavigator nav,
      string attributeName,
      EnumType enumType,
      string sourceLocation,
      IList<EdmSchemaError> parsingErrors)
    {
      IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
      string attributeValue = MappingItemLoader.GetAttributeValue(nav, attributeName);
      if (string.IsNullOrEmpty(attributeValue))
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_Enum_EmptyValue), enumType.FullName, MappingErrorCode.InvalidEnumValue, sourceLocation, lineInfo, parsingErrors);
      EnumMember enumMember;
      if (!enumType.Members.TryGetValue(attributeValue, false, out enumMember))
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_Enum_InvalidValue), attributeValue, MappingErrorCode.InvalidEnumValue, sourceLocation, lineInfo, parsingErrors);
      return enumMember;
    }

    private string GetAliasResolvedValue(string aliasedString)
    {
      if (aliasedString == null || aliasedString.Length == 0)
        return aliasedString;
      int num = aliasedString.LastIndexOf('.');
      if (num == -1)
        return aliasedString;
      string str;
      this.m_alias.TryGetValue(aliasedString.Substring(0, num), out str);
      if (str != null)
        aliasedString = str + aliasedString.Substring(num);
      return aliasedString;
    }

    private XmlReader GetSchemaValidatingReader(XmlReader innerReader)
    {
      XmlReaderSettings xmlReaderSettings = this.GetXmlReaderSettings();
      return XmlReader.Create(innerReader, xmlReaderSettings);
    }

    private XmlReaderSettings GetXmlReaderSettings()
    {
      XmlReaderSettings xmlReaderSettings = System.Data.Entity.Core.SchemaObjectModel.Schema.CreateEdmStandardXmlReaderSettings();
      xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
      xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(this.XsdValidationCallBack);
      xmlReaderSettings.ValidationType = ValidationType.Schema;
      xmlReaderSettings.Schemas = MappingItemLoader.GetOrCreateSchemaSet();
      return xmlReaderSettings;
    }

    private void XsdValidationCallBack(object sender, ValidationEventArgs args)
    {
      if (args.Severity == XmlSeverityType.Warning)
        return;
      string schemaLocation = (string) null;
      if (!string.IsNullOrEmpty(args.Exception.SourceUri))
        schemaLocation = Helper.GetFileNameFromUri(new Uri(args.Exception.SourceUri));
      EdmSchemaErrorSeverity severity = EdmSchemaErrorSeverity.Error;
      if (args.Severity == XmlSeverityType.Warning)
        severity = EdmSchemaErrorSeverity.Warning;
      this.m_parsingErrors.Add(new EdmSchemaError(Strings.Mapping_InvalidMappingSchema_validation((object) args.Exception.Message), 2025, severity, schemaLocation, args.Exception.LineNumber, args.Exception.LinePosition));
    }

    private void ValidateAndUpdateScalarMemberMapping(
      EdmProperty member,
      EdmProperty columnMember,
      IXmlLineInfo lineInfo)
    {
      KeyValuePair<TypeUsage, TypeUsage> keyValuePair;
      if (!this.m_scalarMemberMappings.TryGetValue((EdmMember) member, out keyValuePair))
      {
        int count = this.m_parsingErrors.Count;
        TypeUsage key = Helper.ValidateAndConvertTypeUsage(member, columnMember);
        if (key == null)
        {
          if (count != this.m_parsingErrors.Count)
            return;
          this.m_parsingErrors.Add(new EdmSchemaError(MappingItemLoader.GetInvalidMemberMappingErrorMessage((EdmMember) member, (EdmMember) columnMember), 2019, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition));
        }
        else
          this.m_scalarMemberMappings.Add((EdmMember) member, new KeyValuePair<TypeUsage, TypeUsage>(key, columnMember.TypeUsage));
      }
      else
      {
        TypeUsage typeUsage = keyValuePair.Value;
        TypeUsage modelTypeUsage = columnMember.TypeUsage.ModelTypeUsage;
        if (!object.ReferenceEquals((object) columnMember.TypeUsage.EdmType, (object) typeUsage.EdmType))
        {
          this.m_parsingErrors.Add(new EdmSchemaError(Strings.Mapping_StoreTypeMismatch_ScalarPropertyMapping((object) member.Name, (object) typeUsage.EdmType.Name), 2039, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition));
        }
        else
        {
          if (TypeSemantics.IsSubTypeOf(MappingItemLoader.ResolveTypeUsageForEnums(member.TypeUsage), modelTypeUsage))
            return;
          this.m_parsingErrors.Add(new EdmSchemaError(MappingItemLoader.GetInvalidMemberMappingErrorMessage((EdmMember) member, (EdmMember) columnMember), 2019, EdmSchemaErrorSeverity.Error, this.m_sourceLocation, lineInfo.LineNumber, lineInfo.LinePosition));
        }
      }
    }

    internal static string GetInvalidMemberMappingErrorMessage(
      EdmMember cSpaceMember,
      EdmMember sSpaceMember)
    {
      return Strings.Mapping_Invalid_Member_Mapping((object) (cSpaceMember.TypeUsage.EdmType.ToString() + MappingItemLoader.GetFacetsForDisplay(cSpaceMember.TypeUsage)), (object) cSpaceMember.Name, (object) cSpaceMember.DeclaringType.FullName, (object) (sSpaceMember.TypeUsage.EdmType.ToString() + MappingItemLoader.GetFacetsForDisplay(sSpaceMember.TypeUsage)), (object) sSpaceMember.Name, (object) sSpaceMember.DeclaringType.FullName);
    }

    private static string GetFacetsForDisplay(TypeUsage typeUsage)
    {
      ReadOnlyMetadataCollection<Facet> facets = typeUsage.Facets;
      if (facets == null || facets.Count == 0)
        return string.Empty;
      int count = facets.Count;
      StringBuilder stringBuilder = new StringBuilder("[");
      for (int index = 0; index < count - 1; ++index)
        stringBuilder.AppendFormat("{0}={1},", (object) facets[index].Name, facets[index].Value ?? (object) string.Empty);
      stringBuilder.AppendFormat("{0}={1}]", (object) facets[count - 1].Name, facets[count - 1].Value ?? (object) string.Empty);
      return stringBuilder.ToString();
    }

    internal static TypeUsage ResolveTypeUsageForEnums(TypeUsage typeUsage)
    {
      if (!Helper.IsEnumType(typeUsage.EdmType))
        return typeUsage;
      return TypeUsage.Create((EdmType) Helper.GetUnderlyingEdmTypeForEnumType(typeUsage.EdmType), (IEnumerable<Facet>) typeUsage.Facets);
    }

    private class ModificationFunctionMappingLoader
    {
      private readonly MappingItemLoader m_parentLoader;
      private EdmFunction m_function;
      private readonly EntitySet m_entitySet;
      private readonly AssociationSet m_associationSet;
      private readonly System.Data.Entity.Core.Metadata.Edm.EntityContainer m_modelContainer;
      private readonly EdmItemCollection m_edmItemCollection;
      private readonly StoreItemCollection m_storeItemCollection;
      private bool m_allowCurrentVersion;
      private bool m_allowOriginalVersion;
      private readonly Set<FunctionParameter> m_seenParameters;
      private readonly Stack<EdmMember> m_members;
      private AssociationSet m_associationSetNavigation;

      internal ModificationFunctionMappingLoader(
        MappingItemLoader parentLoader,
        EntitySetBase extent)
      {
        this.m_parentLoader = parentLoader;
        this.m_modelContainer = extent.EntityContainer;
        this.m_edmItemCollection = parentLoader.EdmItemCollection;
        this.m_storeItemCollection = parentLoader.StoreItemCollection;
        this.m_entitySet = extent as EntitySet;
        if (this.m_entitySet == null)
          this.m_associationSet = (AssociationSet) extent;
        this.m_seenParameters = new Set<FunctionParameter>();
        this.m_members = new Stack<EdmMember>();
      }

      internal ModificationFunctionMapping LoadEntityTypeModificationFunctionMapping(
        XPathNavigator nav,
        EntitySetBase entitySet,
        bool allowCurrentVersion,
        bool allowOriginalVersion,
        System.Data.Entity.Core.Metadata.Edm.EntityType entityType)
      {
        FunctionParameter rowsAffectedParameter;
        this.m_function = this.LoadAndValidateFunctionMetadata(nav.Clone(), out rowsAffectedParameter);
        if (this.m_function == null)
          return (ModificationFunctionMapping) null;
        this.m_allowCurrentVersion = allowCurrentVersion;
        this.m_allowOriginalVersion = allowOriginalVersion;
        IEnumerable<ModificationFunctionParameterBinding> parameterBindings = this.LoadParameterBindings(nav.Clone(), (StructuralType) entityType);
        IEnumerable<ModificationFunctionResultBinding> resultBindings = this.LoadResultBindings(nav.Clone(), entityType);
        return new ModificationFunctionMapping(entitySet, (EntityTypeBase) entityType, this.m_function, parameterBindings, rowsAffectedParameter, resultBindings);
      }

      internal ModificationFunctionMapping LoadAssociationSetModificationFunctionMapping(
        XPathNavigator nav,
        EntitySetBase entitySet,
        bool isInsert)
      {
        FunctionParameter rowsAffectedParameter;
        this.m_function = this.LoadAndValidateFunctionMetadata(nav.Clone(), out rowsAffectedParameter);
        if (this.m_function == null)
          return (ModificationFunctionMapping) null;
        if (isInsert)
        {
          this.m_allowCurrentVersion = true;
          this.m_allowOriginalVersion = false;
        }
        else
        {
          this.m_allowCurrentVersion = false;
          this.m_allowOriginalVersion = true;
        }
        IEnumerable<ModificationFunctionParameterBinding> parameterBindings = this.LoadParameterBindings(nav.Clone(), (StructuralType) this.m_associationSet.ElementType);
        return new ModificationFunctionMapping(entitySet, entitySet.ElementType, this.m_function, parameterBindings, rowsAffectedParameter, (IEnumerable<ModificationFunctionResultBinding>) null);
      }

      private IEnumerable<ModificationFunctionResultBinding> LoadResultBindings(
        XPathNavigator nav,
        System.Data.Entity.Core.Metadata.Edm.EntityType entityType)
      {
        List<ModificationFunctionResultBinding> functionResultBindingList = new List<ModificationFunctionResultBinding>();
        IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
        if (nav.MoveToChild(XPathNodeType.Element))
        {
          do
          {
            if (nav.LocalName == "ResultBinding")
            {
              string resolvedAttributeValue1 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
              string resolvedAttributeValue2 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "ColumnName");
              EdmProperty property = (EdmProperty) null;
              if (resolvedAttributeValue1 == null || !entityType.Properties.TryGetValue(resolvedAttributeValue1, false, out property))
              {
                MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_ModificationFunction_PropertyNotFound), resolvedAttributeValue1, entityType.Name, MappingErrorCode.InvalidEdmMember, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
                return (IEnumerable<ModificationFunctionResultBinding>) new List<ModificationFunctionResultBinding>();
              }
              ModificationFunctionResultBinding functionResultBinding = new ModificationFunctionResultBinding(resolvedAttributeValue2, property);
              functionResultBindingList.Add(functionResultBinding);
            }
          }
          while (nav.MoveToNext(XPathNodeType.Element));
        }
        KeyToListMap<EdmProperty, string> keyToListMap = new KeyToListMap<EdmProperty, string>((IEqualityComparer<EdmProperty>) EqualityComparer<EdmProperty>.Default);
        foreach (ModificationFunctionResultBinding functionResultBinding in functionResultBindingList)
          keyToListMap.Add(functionResultBinding.Property, functionResultBinding.ColumnName);
        foreach (EdmProperty key in keyToListMap.Keys)
        {
          ReadOnlyCollection<string> readOnlyCollection = keyToListMap.ListForKey(key);
          if (1 < readOnlyCollection.Count)
          {
            MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_ModificationFunction_AmbiguousResultBinding), key.Name, StringUtil.ToCommaSeparatedString((IEnumerable) readOnlyCollection), MappingErrorCode.AmbiguousResultBindingInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
            return (IEnumerable<ModificationFunctionResultBinding>) new List<ModificationFunctionResultBinding>();
          }
        }
        return (IEnumerable<ModificationFunctionResultBinding>) functionResultBindingList;
      }

      private IEnumerable<ModificationFunctionParameterBinding> LoadParameterBindings(
        XPathNavigator nav,
        StructuralType type)
      {
        List<ModificationFunctionParameterBinding> parameterBindingList = new List<ModificationFunctionParameterBinding>(this.LoadParameterBindings(nav.Clone(), type, false));
        Set<FunctionParameter> set = new Set<FunctionParameter>((IEnumerable<FunctionParameter>) this.m_function.Parameters);
        set.Subtract((IEnumerable<FunctionParameter>) this.m_seenParameters);
        if (set.Count == 0)
          return (IEnumerable<ModificationFunctionParameterBinding>) parameterBindingList;
        MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_ModificationFunction_MissingParameter), this.m_function.FullName, StringUtil.ToCommaSeparatedString((IEnumerable) set), MappingErrorCode.InvalidParameterInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, (IXmlLineInfo) nav, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
        return (IEnumerable<ModificationFunctionParameterBinding>) new List<ModificationFunctionParameterBinding>();
      }

      private IEnumerable<ModificationFunctionParameterBinding> LoadParameterBindings(
        XPathNavigator nav,
        StructuralType type,
        bool restrictToKeyMembers)
      {
        if (nav.MoveToChild(XPathNodeType.Element))
        {
          do
          {
            switch (nav.LocalName)
            {
              case "ScalarProperty":
                ModificationFunctionParameterBinding binding = this.LoadScalarPropertyParameterBinding(nav.Clone(), type, restrictToKeyMembers);
                if (binding != null)
                {
                  yield return binding;
                  break;
                }
                goto label_3;
              case "ComplexProperty":
                ComplexType complexType;
                EdmMember property = this.LoadComplexTypeProperty(nav.Clone(), type, out complexType);
                if (property != null)
                {
                  this.m_members.Push(property);
                  foreach (ModificationFunctionParameterBinding parameterBinding in this.LoadParameterBindings(nav.Clone(), (StructuralType) complexType, restrictToKeyMembers))
                    yield return parameterBinding;
                  this.m_members.Pop();
                  break;
                }
                break;
              case "AssociationEnd":
                AssociationSetEnd toEnd = this.LoadAssociationEnd(nav.Clone());
                if (toEnd != null)
                {
                  this.m_members.Push((EdmMember) toEnd.CorrespondingAssociationEndMember);
                  this.m_associationSetNavigation = toEnd.ParentAssociationSet;
                  foreach (ModificationFunctionParameterBinding parameterBinding in this.LoadParameterBindings(nav.Clone(), (StructuralType) toEnd.EntitySet.ElementType, true))
                    yield return parameterBinding;
                  this.m_associationSetNavigation = (AssociationSet) null;
                  this.m_members.Pop();
                  break;
                }
                break;
              case "EndProperty":
                AssociationSetEnd end = this.LoadEndProperty(nav.Clone());
                if (end != null)
                {
                  this.m_members.Push((EdmMember) end.CorrespondingAssociationEndMember);
                  foreach (ModificationFunctionParameterBinding parameterBinding in this.LoadParameterBindings(nav.Clone(), (StructuralType) end.EntitySet.ElementType, true))
                    yield return parameterBinding;
                  this.m_members.Pop();
                  break;
                }
                break;
            }
          }
          while (nav.MoveToNext(XPathNodeType.Element));
          goto label_4;
label_3:
          yield break;
label_4:;
        }
      }

      private AssociationSetEnd LoadAssociationEnd(XPathNavigator nav)
      {
        IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
        string resolvedAttributeValue1 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "AssociationSet");
        string resolvedAttributeValue2 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "From");
        string resolvedAttributeValue3 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "To");
        RelationshipSet relationshipSet = (RelationshipSet) null;
        if (resolvedAttributeValue1 == null || !this.m_modelContainer.TryGetRelationshipSetByName(resolvedAttributeValue1, false, out relationshipSet) || BuiltInTypeKind.AssociationSet != relationshipSet.BuiltInTypeKind)
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_AssociationSetDoesNotExist), resolvedAttributeValue1, MappingErrorCode.InvalidAssociationSet, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (AssociationSetEnd) null;
        }
        AssociationSet associationSet = (AssociationSet) relationshipSet;
        AssociationSetEnd associationSetEnd1 = (AssociationSetEnd) null;
        if (resolvedAttributeValue2 == null || !associationSet.AssociationSetEnds.TryGetValue(resolvedAttributeValue2, false, out associationSetEnd1))
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_AssociationSetRoleDoesNotExist), resolvedAttributeValue2, MappingErrorCode.InvalidAssociationSetRoleInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (AssociationSetEnd) null;
        }
        AssociationSetEnd associationSetEnd2 = (AssociationSetEnd) null;
        if (resolvedAttributeValue3 == null || !associationSet.AssociationSetEnds.TryGetValue(resolvedAttributeValue3, false, out associationSetEnd2))
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_AssociationSetRoleDoesNotExist), resolvedAttributeValue3, MappingErrorCode.InvalidAssociationSetRoleInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (AssociationSetEnd) null;
        }
        if (!associationSetEnd1.EntitySet.Equals((object) this.m_entitySet))
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_AssociationSetFromRoleIsNotEntitySet), resolvedAttributeValue2, MappingErrorCode.InvalidAssociationSetRoleInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (AssociationSetEnd) null;
        }
        if (associationSetEnd2.CorrespondingAssociationEndMember.RelationshipMultiplicity != RelationshipMultiplicity.One && associationSetEnd2.CorrespondingAssociationEndMember.RelationshipMultiplicity != RelationshipMultiplicity.ZeroOrOne)
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_AssociationSetCardinality), resolvedAttributeValue3, MappingErrorCode.InvalidAssociationSetCardinalityInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (AssociationSetEnd) null;
        }
        if (associationSet.ElementType.IsForeignKey)
        {
          System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint referentialConstraint = associationSet.ElementType.ReferentialConstraints.Single<System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint>();
          EdmSchemaError errorsWithMemberInfo = MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_AssociationEndMappingForeignKeyAssociation), resolvedAttributeValue3, MappingErrorCode.InvalidModificationFunctionMappingAssociationEndForeignKey, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          if (associationSetEnd1.CorrespondingAssociationEndMember != referentialConstraint.ToRole || !referentialConstraint.ToProperties.All<EdmProperty>((Func<EdmProperty, bool>) (p => this.m_entitySet.ElementType.KeyMembers.Contains((EdmMember) p))))
            return (AssociationSetEnd) null;
          errorsWithMemberInfo.Severity = EdmSchemaErrorSeverity.Warning;
        }
        return associationSetEnd2;
      }

      private AssociationSetEnd LoadEndProperty(XPathNavigator nav)
      {
        string resolvedAttributeValue = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
        AssociationSetEnd associationSetEnd = (AssociationSetEnd) null;
        if (resolvedAttributeValue != null && this.m_associationSet.AssociationSetEnds.TryGetValue(resolvedAttributeValue, false, out associationSetEnd))
          return associationSetEnd;
        MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_AssociationSetRoleDoesNotExist), resolvedAttributeValue, MappingErrorCode.InvalidAssociationSetRoleInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, (IXmlLineInfo) nav, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
        return (AssociationSetEnd) null;
      }

      private EdmMember LoadComplexTypeProperty(
        XPathNavigator nav,
        StructuralType type,
        out ComplexType complexType)
      {
        IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
        string resolvedAttributeValue1 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
        string resolvedAttributeValue2 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "TypeName");
        EdmMember edmMember = (EdmMember) null;
        if (resolvedAttributeValue1 == null || !type.Members.TryGetValue(resolvedAttributeValue1, false, out edmMember))
        {
          MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_ModificationFunction_PropertyNotFound), resolvedAttributeValue1, type.Name, MappingErrorCode.InvalidEdmMember, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          complexType = (ComplexType) null;
          return (EdmMember) null;
        }
        complexType = (ComplexType) null;
        if (resolvedAttributeValue2 == null || !this.m_edmItemCollection.TryGetItem<ComplexType>(resolvedAttributeValue2, out complexType))
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_ComplexTypeNotFound), resolvedAttributeValue2, MappingErrorCode.InvalidComplexType, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (EdmMember) null;
        }
        if (edmMember.TypeUsage.EdmType.Equals((object) complexType) || Helper.IsSubtypeOf(edmMember.TypeUsage.EdmType, (EdmType) complexType))
          return edmMember;
        MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_ModificationFunction_WrongComplexType), resolvedAttributeValue2, edmMember.Name, MappingErrorCode.InvalidComplexType, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
        return (EdmMember) null;
      }

      [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
      private ModificationFunctionParameterBinding LoadScalarPropertyParameterBinding(
        XPathNavigator nav,
        StructuralType type,
        bool restrictToKeyMembers)
      {
        IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
        string resolvedAttributeValue1 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "ParameterName");
        string resolvedAttributeValue2 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "Name");
        string resolvedAttributeValue3 = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "Version");
        bool isCurrent;
        if (resolvedAttributeValue3 == null)
        {
          if (!this.m_allowOriginalVersion)
            isCurrent = true;
          else if (!this.m_allowCurrentVersion)
          {
            isCurrent = false;
          }
          else
          {
            MappingItemLoader.AddToSchemaErrors(Strings.Mapping_ModificationFunction_MissingVersion, MappingErrorCode.MissingVersionInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
            return (ModificationFunctionParameterBinding) null;
          }
        }
        else
          isCurrent = resolvedAttributeValue3 == "Current";
        if (isCurrent && !this.m_allowCurrentVersion)
        {
          MappingItemLoader.AddToSchemaErrors(Strings.Mapping_ModificationFunction_VersionMustBeOriginal, MappingErrorCode.InvalidVersionInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (ModificationFunctionParameterBinding) null;
        }
        if (!isCurrent && !this.m_allowOriginalVersion)
        {
          MappingItemLoader.AddToSchemaErrors(Strings.Mapping_ModificationFunction_VersionMustBeCurrent, MappingErrorCode.InvalidVersionInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (ModificationFunctionParameterBinding) null;
        }
        FunctionParameter functionParameter = (FunctionParameter) null;
        if (resolvedAttributeValue1 == null || !this.m_function.Parameters.TryGetValue(resolvedAttributeValue1, false, out functionParameter))
        {
          MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_ModificationFunction_ParameterNotFound), resolvedAttributeValue1, this.m_function.Name, MappingErrorCode.InvalidParameterInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (ModificationFunctionParameterBinding) null;
        }
        EdmMember edmMember = (EdmMember) null;
        if (restrictToKeyMembers)
        {
          if (resolvedAttributeValue2 == null || !((EntityTypeBase) type).KeyMembers.TryGetValue(resolvedAttributeValue2, false, out edmMember))
          {
            MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_ModificationFunction_PropertyNotKey), resolvedAttributeValue2, type.Name, MappingErrorCode.InvalidEdmMember, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
            return (ModificationFunctionParameterBinding) null;
          }
        }
        else if (resolvedAttributeValue2 == null || !type.Members.TryGetValue(resolvedAttributeValue2, false, out edmMember))
        {
          MappingItemLoader.AddToSchemaErrorWithMemberAndStructure(new Func<object, object, string>(Strings.Mapping_ModificationFunction_PropertyNotFound), resolvedAttributeValue2, type.Name, MappingErrorCode.InvalidEdmMember, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (ModificationFunctionParameterBinding) null;
        }
        if (this.m_seenParameters.Contains(functionParameter))
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_ParameterBoundTwice), resolvedAttributeValue1, MappingErrorCode.ParameterBoundTwiceInModificationFunctionMapping, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (ModificationFunctionParameterBinding) null;
        }
        int count = this.m_parentLoader.m_parsingErrors.Count;
        if (Helper.ValidateAndConvertTypeUsage(edmMember.TypeUsage, functionParameter.TypeUsage) == null && count == this.m_parentLoader.m_parsingErrors.Count)
          MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_ModificationFunction_PropertyParameterTypeMismatch((object) edmMember.TypeUsage.EdmType, (object) edmMember.Name, (object) edmMember.DeclaringType.FullName, (object) functionParameter.TypeUsage.EdmType, (object) functionParameter.Name, (object) this.m_function.FullName), MappingErrorCode.InvalidModificationFunctionMappingPropertyParameterTypeMismatch, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
        this.m_members.Push(edmMember);
        IEnumerable<EdmMember> members = (IEnumerable<EdmMember>) this.m_members;
        AssociationSet associationSet = this.m_associationSetNavigation;
        if (this.m_members.Last<EdmMember>().BuiltInTypeKind == BuiltInTypeKind.AssociationEndMember)
        {
          AssociationEndMember associationEndMember = (AssociationEndMember) this.m_members.Last<EdmMember>();
          AssociationType declaringType = (AssociationType) associationEndMember.DeclaringType;
          if (declaringType.IsForeignKey)
          {
            System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint referentialConstraint = declaringType.ReferentialConstraints.Single<System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint>();
            if (referentialConstraint.FromRole == associationEndMember)
            {
              int index = referentialConstraint.FromProperties.IndexOf((EdmProperty) this.m_members.First<EdmMember>());
              members = (IEnumerable<EdmMember>) new EdmMember[1]
              {
                (EdmMember) referentialConstraint.ToProperties[index]
              };
              associationSet = (AssociationSet) null;
            }
          }
        }
        ModificationFunctionParameterBinding parameterBinding = new ModificationFunctionParameterBinding(functionParameter, new ModificationFunctionMemberPath(members, associationSet), isCurrent);
        this.m_members.Pop();
        this.m_seenParameters.Add(functionParameter);
        return parameterBinding;
      }

      private EdmFunction LoadAndValidateFunctionMetadata(
        XPathNavigator nav,
        out FunctionParameter rowsAffectedParameter)
      {
        IXmlLineInfo lineInfo = (IXmlLineInfo) nav;
        this.m_seenParameters.Clear();
        string resolvedAttributeValue = this.m_parentLoader.GetAliasResolvedAttributeValue(nav.Clone(), "FunctionName");
        rowsAffectedParameter = (FunctionParameter) null;
        ReadOnlyCollection<EdmFunction> functions = this.m_storeItemCollection.GetFunctions(resolvedAttributeValue);
        if (functions.Count == 0)
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_UnknownFunction), resolvedAttributeValue, MappingErrorCode.InvalidModificationFunctionMappingUnknownFunction, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (EdmFunction) null;
        }
        if (1 < functions.Count)
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_AmbiguousFunction), resolvedAttributeValue, MappingErrorCode.InvalidModificationFunctionMappingAmbiguousFunction, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (EdmFunction) null;
        }
        EdmFunction function = functions[0];
        if (MetadataHelper.IsComposable(function))
        {
          MappingItemLoader.AddToSchemaErrorsWithMemberInfo(new Func<object, string>(Strings.Mapping_ModificationFunction_NotValidFunction), resolvedAttributeValue, MappingErrorCode.InvalidModificationFunctionMappingNotValidFunction, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
          return (EdmFunction) null;
        }
        string attributeValue = MappingItemLoader.GetAttributeValue(nav, "RowsAffectedParameter");
        if (!string.IsNullOrEmpty(attributeValue))
        {
          if (!function.Parameters.TryGetValue(attributeValue, false, out rowsAffectedParameter))
          {
            MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_RowsAffectedParameterDoesNotExist((object) attributeValue, (object) function.FullName), MappingErrorCode.MappingFunctionImportRowsAffectedParameterDoesNotExist, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
            return (EdmFunction) null;
          }
          if (ParameterMode.Out != rowsAffectedParameter.Mode && ParameterMode.InOut != rowsAffectedParameter.Mode)
          {
            MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_RowsAffectedParameterHasWrongMode((object) attributeValue, (object) rowsAffectedParameter.Mode, (object) ParameterMode.Out, (object) ParameterMode.InOut), MappingErrorCode.MappingFunctionImportRowsAffectedParameterHasWrongMode, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
            return (EdmFunction) null;
          }
          PrimitiveType edmType = (PrimitiveType) rowsAffectedParameter.TypeUsage.EdmType;
          if (!TypeSemantics.IsIntegerNumericType(rowsAffectedParameter.TypeUsage))
          {
            MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_FunctionImport_RowsAffectedParameterHasWrongType((object) attributeValue, (object) edmType.PrimitiveTypeKind), MappingErrorCode.MappingFunctionImportRowsAffectedParameterHasWrongType, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
            return (EdmFunction) null;
          }
          this.m_seenParameters.Add(rowsAffectedParameter);
        }
        foreach (FunctionParameter parameter in function.Parameters)
        {
          if (parameter.Mode != ParameterMode.In && attributeValue != parameter.Name)
          {
            MappingItemLoader.AddToSchemaErrorWithMessage(Strings.Mapping_ModificationFunction_NotValidFunctionParameter((object) resolvedAttributeValue, (object) parameter.Name, (object) "RowsAffectedParameter"), MappingErrorCode.InvalidModificationFunctionMappingNotValidFunctionParameter, this.m_parentLoader.m_sourceLocation, lineInfo, (IList<EdmSchemaError>) this.m_parentLoader.m_parsingErrors);
            return (EdmFunction) null;
          }
        }
        return function;
      }
    }
  }
}
