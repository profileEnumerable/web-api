// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.StorageMappingItemCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.Update.Internal;
using System.Data.Entity.Core.Mapping.ViewGeneration;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.SchemaObjectModel;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Represents a collection of items in Storage Mapping (CS Mapping) space.
  /// </summary>
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  public class StorageMappingItemCollection : MappingItemCollection
  {
    private readonly Dictionary<EdmMember, KeyValuePair<TypeUsage, TypeUsage>> m_memberMappings = new Dictionary<EdmMember, KeyValuePair<TypeUsage, TypeUsage>>();
    private readonly ConcurrentDictionary<Tuple<EntitySetBase, EntityTypeBase, StorageMappingItemCollection.InterestingMembersKind>, ReadOnlyCollection<EdmMember>> _cachedInterestingMembers = new ConcurrentDictionary<Tuple<EntitySetBase, EntityTypeBase, StorageMappingItemCollection.InterestingMembersKind>, ReadOnlyCollection<EdmMember>>();
    private EdmItemCollection _edmCollection;
    private StoreItemCollection _storeItemCollection;
    private StorageMappingItemCollection.ViewDictionary m_viewDictionary;
    private double m_mappingVersion;
    private MetadataWorkspace _workspace;
    private ViewLoader _viewLoader;
    private DbMappingViewCacheFactory _mappingViewCacheFactory;

    internal StorageMappingItemCollection()
      : base(DataSpace.CSSpace)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Mapping.StorageMappingItemCollection" /> class using the specified <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmItemCollection" />, <see cref="T:System.Data.Entity.Core.Metadata.Edm.StoreItemCollection" /> and a collection of string indicating the metadata file paths.</summary>
    /// <param name="edmCollection">The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmItemCollection" /> that this mapping is to use.</param>
    /// <param name="storeCollection">The <see cref="T:System.Data.Entity.Core.Metadata.Edm.StoreItemCollection" /> that this mapping is to use.</param>
    /// <param name="filePaths">The file paths that this mapping is to use.</param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public StorageMappingItemCollection(
      EdmItemCollection edmCollection,
      StoreItemCollection storeCollection,
      params string[] filePaths)
      : base(DataSpace.CSSpace)
    {
      Check.NotNull<EdmItemCollection>(edmCollection, nameof (edmCollection));
      Check.NotNull<StoreItemCollection>(storeCollection, nameof (storeCollection));
      Check.NotNull<string[]>(filePaths, nameof (filePaths));
      this._edmCollection = edmCollection;
      this._storeItemCollection = storeCollection;
      List<XmlReader> xmlReaderList = (List<XmlReader>) null;
      try
      {
        MetadataArtifactLoader compositeFromFilePaths = MetadataArtifactLoader.CreateCompositeFromFilePaths((IEnumerable<string>) filePaths, ".msl");
        xmlReaderList = compositeFromFilePaths.CreateReaders(DataSpace.CSSpace);
        this.Init(edmCollection, storeCollection, (IEnumerable<XmlReader>) xmlReaderList, (IList<string>) compositeFromFilePaths.GetPaths(DataSpace.CSSpace), true);
      }
      finally
      {
        if (xmlReaderList != null)
          Helper.DisposeXmlReaders((IEnumerable<XmlReader>) xmlReaderList);
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Mapping.StorageMappingItemCollection" /> class using the specified <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmItemCollection" />, <see cref="T:System.Data.Entity.Core.Metadata.Edm.StoreItemCollection" /> and XML readers.</summary>
    /// <param name="edmCollection">The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmItemCollection" /> that this mapping is to use.</param>
    /// <param name="storeCollection">The <see cref="T:System.Data.Entity.Core.Metadata.Edm.StoreItemCollection" /> that this mapping is to use.</param>
    /// <param name="xmlReaders">The XML readers that this mapping is to use.</param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public StorageMappingItemCollection(
      EdmItemCollection edmCollection,
      StoreItemCollection storeCollection,
      IEnumerable<XmlReader> xmlReaders)
      : base(DataSpace.CSSpace)
    {
      Check.NotNull<IEnumerable<XmlReader>>(xmlReaders, nameof (xmlReaders));
      MetadataArtifactLoader compositeFromXmlReaders = MetadataArtifactLoader.CreateCompositeFromXmlReaders(xmlReaders);
      this.Init(edmCollection, storeCollection, (IEnumerable<XmlReader>) compositeFromXmlReaders.GetReaders(), (IList<string>) compositeFromXmlReaders.GetPaths(), true);
    }

    private StorageMappingItemCollection(
      EdmItemCollection edmItemCollection,
      StoreItemCollection storeItemCollection,
      IEnumerable<XmlReader> xmlReaders,
      IList<string> filePaths,
      out IList<EdmSchemaError> errors)
      : base(DataSpace.CSSpace)
    {
      errors = this.Init(edmItemCollection, storeItemCollection, xmlReaders, filePaths, false);
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    internal StorageMappingItemCollection(
      EdmItemCollection edmCollection,
      StoreItemCollection storeCollection,
      IEnumerable<XmlReader> xmlReaders,
      IList<string> filePaths)
      : base(DataSpace.CSSpace)
    {
      this.Init(edmCollection, storeCollection, xmlReaders, filePaths, true);
    }

    private IList<EdmSchemaError> Init(
      EdmItemCollection edmCollection,
      StoreItemCollection storeCollection,
      IEnumerable<XmlReader> xmlReaders,
      IList<string> filePaths,
      bool throwOnError)
    {
      this._edmCollection = edmCollection;
      this._storeItemCollection = storeCollection;
      Dictionary<EntitySetBase, GeneratedView> userDefinedQueryViewsDict;
      Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView> userDefinedQueryViewsOfTypeDict;
      this.m_viewDictionary = new StorageMappingItemCollection.ViewDictionary(this, out userDefinedQueryViewsDict, out userDefinedQueryViewsOfTypeDict);
      List<EdmSchemaError> edmSchemaErrorList = new List<EdmSchemaError>();
      if (this._edmCollection.EdmVersion != 0.0 && this._storeItemCollection.StoreSchemaVersion != 0.0 && this._edmCollection.EdmVersion != this._storeItemCollection.StoreSchemaVersion)
      {
        edmSchemaErrorList.Add(new EdmSchemaError(Strings.Mapping_DifferentEdmStoreVersion, 2102, EdmSchemaErrorSeverity.Error));
      }
      else
      {
        double expectedVersion = this._edmCollection.EdmVersion != 0.0 ? this._edmCollection.EdmVersion : this._storeItemCollection.StoreSchemaVersion;
        edmSchemaErrorList.AddRange((IEnumerable<EdmSchemaError>) this.LoadItems(xmlReaders, filePaths, userDefinedQueryViewsDict, userDefinedQueryViewsOfTypeDict, expectedVersion));
      }
      if (edmSchemaErrorList.Count > 0 && throwOnError && !MetadataHelper.CheckIfAllErrorsAreWarnings((IList<EdmSchemaError>) edmSchemaErrorList))
        throw new MappingException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, EntityRes.GetString("InvalidSchemaEncountered"), (object) Helper.CombineErrorMessage((IEnumerable<EdmSchemaError>) edmSchemaErrorList)));
      return (IList<EdmSchemaError>) edmSchemaErrorList;
    }

    /// <summary>
    /// Gets or sets a <see cref="T:System.Data.Entity.Infrastructure.MappingViews.DbMappingViewCacheFactory" /> for creating <see cref="T:System.Data.Entity.Infrastructure.MappingViews.DbMappingViewCache" /> instances
    /// that are used to retrieve pre-generated mapping views.
    /// </summary>
    public DbMappingViewCacheFactory MappingViewCacheFactory
    {
      get
      {
        return this._mappingViewCacheFactory;
      }
      set
      {
        Check.NotNull<DbMappingViewCacheFactory>(value, nameof (value));
        Interlocked.CompareExchange<DbMappingViewCacheFactory>(ref this._mappingViewCacheFactory, value, (DbMappingViewCacheFactory) null);
        if (!this._mappingViewCacheFactory.Equals((object) value))
          throw new ArgumentException(Strings.MappingViewCacheFactory_MustNotChange, nameof (value));
      }
    }

    internal MetadataWorkspace Workspace
    {
      get
      {
        if (this._workspace == null)
          this._workspace = new MetadataWorkspace((Func<EdmItemCollection>) (() => this._edmCollection), (Func<StoreItemCollection>) (() => this._storeItemCollection), (Func<StorageMappingItemCollection>) (() => this));
        return this._workspace;
      }
    }

    internal EdmItemCollection EdmItemCollection
    {
      get
      {
        return this._edmCollection;
      }
    }

    /// <summary>Gets the version of this <see cref="T:System.Data.Entity.Core.Mapping.StorageMappingItemCollection" /> represents.</summary>
    /// <returns>The version of this <see cref="T:System.Data.Entity.Core.Mapping.StorageMappingItemCollection" /> represents.</returns>
    public double MappingVersion
    {
      get
      {
        return this.m_mappingVersion;
      }
    }

    internal StoreItemCollection StoreItemCollection
    {
      get
      {
        return this._storeItemCollection;
      }
    }

    internal override MappingBase GetMap(
      string identity,
      DataSpace typeSpace,
      bool ignoreCase)
    {
      if (typeSpace != DataSpace.CSpace)
        throw new InvalidOperationException(Strings.Mapping_Storage_InvalidSpace((object) typeSpace));
      return this.GetItem<MappingBase>(identity, ignoreCase);
    }

    internal override bool TryGetMap(
      string identity,
      DataSpace typeSpace,
      bool ignoreCase,
      out MappingBase map)
    {
      if (typeSpace != DataSpace.CSpace)
        throw new InvalidOperationException(Strings.Mapping_Storage_InvalidSpace((object) typeSpace));
      return this.TryGetItem<MappingBase>(identity, ignoreCase, out map);
    }

    internal override MappingBase GetMap(string identity, DataSpace typeSpace)
    {
      return this.GetMap(identity, typeSpace, false);
    }

    internal override bool TryGetMap(string identity, DataSpace typeSpace, out MappingBase map)
    {
      return this.TryGetMap(identity, typeSpace, false, out map);
    }

    internal override MappingBase GetMap(GlobalItem item)
    {
      DataSpace dataSpace = item.DataSpace;
      if (dataSpace != DataSpace.CSpace)
        throw new InvalidOperationException(Strings.Mapping_Storage_InvalidSpace((object) dataSpace));
      return this.GetMap(item.Identity, dataSpace);
    }

    internal override bool TryGetMap(GlobalItem item, out MappingBase map)
    {
      if (item == null)
      {
        map = (MappingBase) null;
        return false;
      }
      DataSpace dataSpace = item.DataSpace;
      if (dataSpace == DataSpace.CSpace)
        return this.TryGetMap(item.Identity, dataSpace, out map);
      map = (MappingBase) null;
      return false;
    }

    internal ReadOnlyCollection<EdmMember> GetInterestingMembers(
      EntitySetBase entitySet,
      EntityTypeBase entityType,
      StorageMappingItemCollection.InterestingMembersKind interestingMembersKind)
    {
      return this._cachedInterestingMembers.GetOrAdd(new Tuple<EntitySetBase, EntityTypeBase, StorageMappingItemCollection.InterestingMembersKind>(entitySet, entityType, interestingMembersKind), this.FindInterestingMembers(entitySet, entityType, interestingMembersKind));
    }

    private ReadOnlyCollection<EdmMember> FindInterestingMembers(
      EntitySetBase entitySet,
      EntityTypeBase entityType,
      StorageMappingItemCollection.InterestingMembersKind interestingMembersKind)
    {
      List<EdmMember> interestingMembers = new List<EdmMember>();
      foreach (TypeMapping entitySetAndSuperType in MappingMetadataHelper.GetMappingsForEntitySetAndSuperTypes(this, entitySet.EntityContainer, entitySet, entityType))
      {
        AssociationTypeMapping associationTypeMapping = entitySetAndSuperType as AssociationTypeMapping;
        if (associationTypeMapping != null)
          StorageMappingItemCollection.FindInterestingAssociationMappingMembers(associationTypeMapping, interestingMembers);
        else
          StorageMappingItemCollection.FindInterestingEntityMappingMembers((EntityTypeMapping) entitySetAndSuperType, interestingMembersKind, interestingMembers);
      }
      if (interestingMembersKind != StorageMappingItemCollection.InterestingMembersKind.RequiredOriginalValueMembers)
        StorageMappingItemCollection.FindForeignKeyProperties(entitySet, entityType, interestingMembers);
      foreach (EntityTypeModificationFunctionMapping functionMappings in MappingMetadataHelper.GetModificationFunctionMappingsForEntitySetAndType(this, entitySet.EntityContainer, entitySet, entityType).Where<EntityTypeModificationFunctionMapping>((Func<EntityTypeModificationFunctionMapping, bool>) (functionMappings => functionMappings.UpdateFunctionMapping != null)))
        StorageMappingItemCollection.FindInterestingFunctionMappingMembers(functionMappings, interestingMembersKind, ref interestingMembers);
      return new ReadOnlyCollection<EdmMember>((IList<EdmMember>) interestingMembers.Distinct<EdmMember>().ToList<EdmMember>());
    }

    private static void FindInterestingAssociationMappingMembers(
      AssociationTypeMapping associationTypeMapping,
      List<EdmMember> interestingMembers)
    {
      interestingMembers.AddRange((IEnumerable<EdmMember>) associationTypeMapping.MappingFragments.SelectMany<MappingFragment, PropertyMapping>((Func<MappingFragment, IEnumerable<PropertyMapping>>) (m => (IEnumerable<PropertyMapping>) m.AllProperties)).OfType<EndPropertyMapping>().Select<EndPropertyMapping, AssociationEndMember>((Func<EndPropertyMapping, AssociationEndMember>) (epm => epm.AssociationEnd)));
    }

    private static void FindInterestingEntityMappingMembers(
      EntityTypeMapping entityTypeMapping,
      StorageMappingItemCollection.InterestingMembersKind interestingMembersKind,
      List<EdmMember> interestingMembers)
    {
      foreach (PropertyMapping propertyMapping in entityTypeMapping.MappingFragments.SelectMany<MappingFragment, PropertyMapping>((Func<MappingFragment, IEnumerable<PropertyMapping>>) (mf => (IEnumerable<PropertyMapping>) mf.AllProperties)))
      {
        ScalarPropertyMapping scalarPropertyMapping = propertyMapping as ScalarPropertyMapping;
        ComplexPropertyMapping complexMapping = propertyMapping as ComplexPropertyMapping;
        ConditionPropertyMapping conditionPropertyMapping = propertyMapping as ConditionPropertyMapping;
        if (scalarPropertyMapping != null && scalarPropertyMapping.Property != null)
        {
          if (MetadataHelper.IsPartOfEntityTypeKey((EdmMember) scalarPropertyMapping.Property))
          {
            if (interestingMembersKind == StorageMappingItemCollection.InterestingMembersKind.RequiredOriginalValueMembers)
              interestingMembers.Add((EdmMember) scalarPropertyMapping.Property);
          }
          else if (MetadataHelper.GetConcurrencyMode((EdmMember) scalarPropertyMapping.Property) == ConcurrencyMode.Fixed)
            interestingMembers.Add((EdmMember) scalarPropertyMapping.Property);
        }
        else if (complexMapping != null)
        {
          if (interestingMembersKind == StorageMappingItemCollection.InterestingMembersKind.PartialUpdate || MetadataHelper.GetConcurrencyMode((EdmMember) complexMapping.Property) == ConcurrencyMode.Fixed || StorageMappingItemCollection.HasFixedConcurrencyModeInAnyChildProperty(complexMapping))
            interestingMembers.Add((EdmMember) complexMapping.Property);
        }
        else if (conditionPropertyMapping != null && conditionPropertyMapping.Property != null)
          interestingMembers.Add((EdmMember) conditionPropertyMapping.Property);
      }
    }

    private static bool HasFixedConcurrencyModeInAnyChildProperty(
      ComplexPropertyMapping complexMapping)
    {
      foreach (PropertyMapping propertyMapping in complexMapping.TypeMappings.SelectMany<ComplexTypeMapping, PropertyMapping>((Func<ComplexTypeMapping, IEnumerable<PropertyMapping>>) (m => (IEnumerable<PropertyMapping>) m.AllProperties)))
      {
        ScalarPropertyMapping scalarPropertyMapping = propertyMapping as ScalarPropertyMapping;
        ComplexPropertyMapping complexMapping1 = propertyMapping as ComplexPropertyMapping;
        if (scalarPropertyMapping != null && MetadataHelper.GetConcurrencyMode((EdmMember) scalarPropertyMapping.Property) == ConcurrencyMode.Fixed || complexMapping1 != null && (MetadataHelper.GetConcurrencyMode((EdmMember) complexMapping1.Property) == ConcurrencyMode.Fixed || StorageMappingItemCollection.HasFixedConcurrencyModeInAnyChildProperty(complexMapping1)))
          return true;
      }
      return false;
    }

    private static void FindForeignKeyProperties(
      EntitySetBase entitySetBase,
      EntityTypeBase entityType,
      List<EdmMember> interestingMembers)
    {
      EntitySet entitySet = entitySetBase as EntitySet;
      if (entitySet == null || !entitySet.HasForeignKeyRelationships)
        return;
      interestingMembers.AddRange((IEnumerable<EdmMember>) MetadataHelper.GetTypeAndParentTypesOf((EdmType) entityType, true).SelectMany<EdmType, EdmProperty>((Func<EdmType, IEnumerable<EdmProperty>>) (e => (IEnumerable<EdmProperty>) ((System.Data.Entity.Core.Metadata.Edm.EntityType) e).Properties)).Where<EdmProperty>((Func<EdmProperty, bool>) (p => entitySet.ForeignKeyDependents.SelectMany<Tuple<AssociationSet, System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint>, EdmProperty>((Func<Tuple<AssociationSet, System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint>, IEnumerable<EdmProperty>>) (fk => (IEnumerable<EdmProperty>) fk.Item2.ToProperties)).Contains<EdmProperty>(p))));
    }

    private static void FindInterestingFunctionMappingMembers(
      EntityTypeModificationFunctionMapping functionMappings,
      StorageMappingItemCollection.InterestingMembersKind interestingMembersKind,
      ref List<EdmMember> interestingMembers)
    {
      if (interestingMembersKind == StorageMappingItemCollection.InterestingMembersKind.PartialUpdate)
      {
        interestingMembers.AddRange(functionMappings.UpdateFunctionMapping.ParameterBindings.Select<ModificationFunctionParameterBinding, EdmMember>((Func<ModificationFunctionParameterBinding, EdmMember>) (p => p.MemberPath.Members.Last<EdmMember>())));
      }
      else
      {
        foreach (ModificationFunctionParameterBinding parameterBinding in functionMappings.UpdateFunctionMapping.ParameterBindings.Where<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (p => !p.IsCurrent)))
          interestingMembers.Add(parameterBinding.MemberPath.Members.Last<EdmMember>());
      }
    }

    internal GeneratedView GetGeneratedView(
      EntitySetBase extent,
      MetadataWorkspace workspace)
    {
      return this.m_viewDictionary.GetGeneratedView(extent, workspace, this);
    }

    private void AddInternal(MappingBase storageMap)
    {
      storageMap.DataSpace = DataSpace.CSSpace;
      try
      {
        this.AddInternal((GlobalItem) storageMap);
      }
      catch (ArgumentException ex)
      {
        throw new MappingException(Strings.Mapping_Duplicate_Type((object) storageMap.EdmItem.Identity), (Exception) ex);
      }
    }

    internal bool ContainsStorageEntityContainer(string storageEntityContainerName)
    {
      return this.GetItems<EntityContainerMapping>().Any<EntityContainerMapping>((Func<EntityContainerMapping, bool>) (map => map.StorageEntityContainer.Name.Equals(storageEntityContainerName, StringComparison.Ordinal)));
    }

    private List<EdmSchemaError> LoadItems(
      IEnumerable<XmlReader> xmlReaders,
      IList<string> mappingSchemaUris,
      Dictionary<EntitySetBase, GeneratedView> userDefinedQueryViewsDict,
      Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView> userDefinedQueryViewsOfTypeDict,
      double expectedVersion)
    {
      List<EdmSchemaError> errorCollection = new List<EdmSchemaError>();
      int index = -1;
      foreach (XmlReader xmlReader in xmlReaders)
      {
        ++index;
        string location = (string) null;
        if (mappingSchemaUris == null)
          SchemaManager.TryGetBaseUri(xmlReader, out location);
        else
          location = mappingSchemaUris[index];
        MappingItemLoader mappingItemLoader = new MappingItemLoader(xmlReader, this, location, this.m_memberMappings);
        errorCollection.AddRange((IEnumerable<EdmSchemaError>) mappingItemLoader.ParsingErrors);
        this.CheckIsSameVersion(expectedVersion, mappingItemLoader.MappingVersion, (IList<EdmSchemaError>) errorCollection);
        EntityContainerMapping containerMapping = mappingItemLoader.ContainerMapping;
        if (mappingItemLoader.HasQueryViews && containerMapping != null)
          StorageMappingItemCollection.CompileUserDefinedQueryViews(containerMapping, userDefinedQueryViewsDict, userDefinedQueryViewsOfTypeDict, (IList<EdmSchemaError>) errorCollection);
        if (MetadataHelper.CheckIfAllErrorsAreWarnings((IList<EdmSchemaError>) errorCollection) && !this.Contains((GlobalItem) containerMapping))
        {
          containerMapping.SetReadOnly();
          this.AddInternal((MappingBase) containerMapping);
        }
      }
      StorageMappingItemCollection.CheckForDuplicateItems(this.EdmItemCollection, this.StoreItemCollection, errorCollection);
      return errorCollection;
    }

    private static void CompileUserDefinedQueryViews(
      EntityContainerMapping entityContainerMapping,
      Dictionary<EntitySetBase, GeneratedView> userDefinedQueryViewsDict,
      Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView> userDefinedQueryViewsOfTypeDict,
      IList<EdmSchemaError> errors)
    {
      ConfigViewGenerator config = new ConfigViewGenerator();
      foreach (EntitySetBaseMapping allSetMap in entityContainerMapping.AllSetMaps)
      {
        GeneratedView generatedView;
        if (allSetMap.QueryView != null && !userDefinedQueryViewsDict.TryGetValue(allSetMap.Set, out generatedView))
        {
          if (GeneratedView.TryParseUserSpecifiedView(allSetMap, allSetMap.Set.ElementType, allSetMap.QueryView, true, entityContainerMapping.StorageMappingItemCollection, config, errors, out generatedView))
            userDefinedQueryViewsDict.Add(allSetMap.Set, generatedView);
          foreach (Pair<EntitySetBase, Pair<EntityTypeBase, bool>> typeSpecificQvKey in allSetMap.GetTypeSpecificQVKeys())
          {
            if (GeneratedView.TryParseUserSpecifiedView(allSetMap, typeSpecificQvKey.Second.First, allSetMap.GetTypeSpecificQueryView(typeSpecificQvKey), typeSpecificQvKey.Second.Second, entityContainerMapping.StorageMappingItemCollection, config, errors, out generatedView))
              userDefinedQueryViewsOfTypeDict.Add(typeSpecificQvKey, generatedView);
          }
        }
      }
    }

    private void CheckIsSameVersion(
      double expectedVersion,
      double currentLoaderVersion,
      IList<EdmSchemaError> errors)
    {
      if (this.m_mappingVersion == 0.0)
        this.m_mappingVersion = currentLoaderVersion;
      if (expectedVersion != 0.0 && currentLoaderVersion != 0.0 && currentLoaderVersion != expectedVersion)
        errors.Add(new EdmSchemaError(Strings.Mapping_DifferentMappingEdmStoreVersion, 2101, EdmSchemaErrorSeverity.Error));
      if (currentLoaderVersion == this.m_mappingVersion || currentLoaderVersion == 0.0)
        return;
      errors.Add(new EdmSchemaError(Strings.CannotLoadDifferentVersionOfSchemaInTheSameItemCollection, 2100, EdmSchemaErrorSeverity.Error));
    }

    internal ViewLoader GetUpdateViewLoader()
    {
      if (this._viewLoader == null)
        this._viewLoader = new ViewLoader(this);
      return this._viewLoader;
    }

    internal bool TryGetGeneratedViewOfType(
      EntitySetBase entity,
      EntityTypeBase type,
      bool includeSubtypes,
      out GeneratedView generatedView)
    {
      return this.m_viewDictionary.TryGetGeneratedViewOfType(entity, type, includeSubtypes, out generatedView);
    }

    private static void CheckForDuplicateItems(
      EdmItemCollection edmItemCollection,
      StoreItemCollection storeItemCollection,
      List<EdmSchemaError> errorCollection)
    {
      foreach (GlobalItem edmItem in (ReadOnlyMetadataCollection<GlobalItem>) edmItemCollection)
      {
        if (storeItemCollection.Contains(edmItem.Identity))
          errorCollection.Add(new EdmSchemaError(Strings.Mapping_ItemWithSameNameExistsBothInCSpaceAndSSpace((object) edmItem.Identity), 2070, EdmSchemaErrorSeverity.Error));
      }
    }

    /// <summary>
    /// Computes a hash value for the container mapping specified by the names of the mapped containers.
    /// </summary>
    /// <param name="conceptualModelContainerName">The name of a container in the conceptual model.</param>
    /// <param name="storeModelContainerName">The name of a container in the store model.</param>
    /// <returns>A string that specifies the computed hash value.</returns>
    public string ComputeMappingHashValue(
      string conceptualModelContainerName,
      string storeModelContainerName)
    {
      Check.NotEmpty(conceptualModelContainerName, nameof (conceptualModelContainerName));
      Check.NotEmpty(storeModelContainerName, nameof (storeModelContainerName));
      EntityContainerMapping entityContainerMapping = this.GetItems<EntityContainerMapping>().SingleOrDefault<EntityContainerMapping>((Func<EntityContainerMapping, bool>) (m =>
      {
        if (m.EdmEntityContainer.Name == conceptualModelContainerName)
          return m.StorageEntityContainer.Name == storeModelContainerName;
        return false;
      }));
      if (entityContainerMapping == null)
        throw new InvalidOperationException(Strings.HashCalcContainersNotFound((object) conceptualModelContainerName, (object) storeModelContainerName));
      return MetadataMappingHasherVisitor.GetMappingClosureHash(this.MappingVersion, entityContainerMapping, true);
    }

    /// <summary>
    /// Computes a hash value for the single container mapping in the collection.
    /// </summary>
    /// <returns>A string that specifies the computed hash value.</returns>
    public string ComputeMappingHashValue()
    {
      if (this.GetItems<EntityContainerMapping>().Count != 1)
        throw new InvalidOperationException(Strings.HashCalcMultipleContainers);
      return MetadataMappingHasherVisitor.GetMappingClosureHash(this.MappingVersion, this.GetItems<EntityContainerMapping>().Single<EntityContainerMapping>(), true);
    }

    /// <summary>
    /// Creates a dictionary of (extent, generated view) for a container mapping specified by
    /// the names of the mapped containers.
    /// </summary>
    /// <param name="conceptualModelContainerName">The name of a container in the conceptual model.</param>
    /// <param name="storeModelContainerName">The name of a container in the store model.</param>
    /// <param name="errors">A list that accumulates potential errors.</param>
    /// <returns>
    /// A dictionary of (<see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySetBase" />, <see cref="T:System.Data.Entity.Infrastructure.MappingViews.DbMappingView" />) that specifies the generated views.
    /// </returns>
    public Dictionary<EntitySetBase, DbMappingView> GenerateViews(
      string conceptualModelContainerName,
      string storeModelContainerName,
      IList<EdmSchemaError> errors)
    {
      Check.NotEmpty(conceptualModelContainerName, nameof (conceptualModelContainerName));
      Check.NotEmpty(storeModelContainerName, nameof (storeModelContainerName));
      Check.NotNull<IList<EdmSchemaError>>(errors, nameof (errors));
      EntityContainerMapping containerMapping = this.GetItems<EntityContainerMapping>().SingleOrDefault<EntityContainerMapping>((Func<EntityContainerMapping, bool>) (m =>
      {
        if (m.EdmEntityContainer.Name == conceptualModelContainerName)
          return m.StorageEntityContainer.Name == storeModelContainerName;
        return false;
      }));
      if (containerMapping == null)
        throw new InvalidOperationException(Strings.ViewGenContainersNotFound((object) conceptualModelContainerName, (object) storeModelContainerName));
      return StorageMappingItemCollection.GenerateViews(containerMapping, errors);
    }

    /// <summary>
    /// Creates a dictionary of (extent, generated view) for the single container mapping
    /// in the collection.
    /// </summary>
    /// <param name="errors">A list that accumulates potential errors.</param>
    /// <returns>
    /// A dictionary of (<see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySetBase" />, <see cref="T:System.Data.Entity.Infrastructure.MappingViews.DbMappingView" />) that specifies the generated views.
    /// </returns>
    public Dictionary<EntitySetBase, DbMappingView> GenerateViews(
      IList<EdmSchemaError> errors)
    {
      Check.NotNull<IList<EdmSchemaError>>(errors, nameof (errors));
      if (this.GetItems<EntityContainerMapping>().Count != 1)
        throw new InvalidOperationException(Strings.ViewGenMultipleContainers);
      return StorageMappingItemCollection.GenerateViews(this.GetItems<EntityContainerMapping>().Single<EntityContainerMapping>(), errors);
    }

    internal static Dictionary<EntitySetBase, DbMappingView> GenerateViews(
      EntityContainerMapping containerMapping,
      IList<EdmSchemaError> errors)
    {
      Dictionary<EntitySetBase, DbMappingView> dictionary = new Dictionary<EntitySetBase, DbMappingView>();
      if (!containerMapping.HasViews)
        return dictionary;
      if (!containerMapping.HasMappingFragments())
      {
        errors.Add(new EdmSchemaError(Strings.Mapping_AllQueryViewAtCompileTime((object) containerMapping.Identity), 2088, EdmSchemaErrorSeverity.Warning));
        return dictionary;
      }
      ViewGenResults viewsFromMapping = ViewgenGatekeeper.GenerateViewsFromMapping(containerMapping, new ConfigViewGenerator()
      {
        GenerateEsql = true
      });
      if (viewsFromMapping.HasErrors)
        viewsFromMapping.Errors.Each<EdmSchemaError>((System.Action<EdmSchemaError>) (e => errors.Add(e)));
      foreach (KeyValuePair<EntitySetBase, List<GeneratedView>> keyValuePair in viewsFromMapping.Views.KeyValuePairs)
        dictionary.Add(keyValuePair.Key, new DbMappingView(keyValuePair.Value[0].eSQL));
      return dictionary;
    }

    /// <summary>
    /// Factory method that creates a <see cref="T:System.Data.Entity.Core.Mapping.StorageMappingItemCollection" />.
    /// </summary>
    /// <param name="edmItemCollection">
    /// The edm metadata collection to map. Must not be <c>null</c>.
    /// </param>
    /// <param name="storeItemCollection">
    /// The store metadata collection to map. Must not be <c>null</c>.
    /// </param>
    /// <param name="xmlReaders">
    /// MSL artifacts to load. Must not be <c>null</c>.
    /// </param>
    /// <param name="filePaths">
    /// Paths to MSL artifacts. Used in error messages. Can be <c>null</c> in which case
    /// the base Uri of the XmlReader will be used as a path.
    /// </param>
    /// <param name="errors">
    /// The collection of errors encountered while loading.
    /// </param>
    /// <returns>
    /// <see cref="P:System.Data.Entity.Core.Mapping.StorageMappingItemCollection.EdmItemCollection" /> instance if no errors encountered. Otherwise <c>null</c>.
    /// </returns>
    public static StorageMappingItemCollection Create(
      EdmItemCollection edmItemCollection,
      StoreItemCollection storeItemCollection,
      IEnumerable<XmlReader> xmlReaders,
      IList<string> filePaths,
      out IList<EdmSchemaError> errors)
    {
      Check.NotNull<EdmItemCollection>(edmItemCollection, nameof (edmItemCollection));
      Check.NotNull<StoreItemCollection>(storeItemCollection, nameof (storeItemCollection));
      Check.NotNull<IEnumerable<XmlReader>>(xmlReaders, nameof (xmlReaders));
      EntityUtil.CheckArgumentContainsNull<XmlReader>(ref xmlReaders, nameof (xmlReaders));
      StorageMappingItemCollection mappingItemCollection = new StorageMappingItemCollection(edmItemCollection, storeItemCollection, xmlReaders, filePaths, out errors);
      if (errors == null || errors.Count <= 0)
        return mappingItemCollection;
      return (StorageMappingItemCollection) null;
    }

    internal delegate bool TryGetUserDefinedQueryView(
      EntitySetBase extent,
      out GeneratedView generatedView);

    internal delegate bool TryGetUserDefinedQueryViewOfType(
      Pair<EntitySetBase, Pair<EntityTypeBase, bool>> extent,
      out GeneratedView generatedView);

    internal class ViewDictionary
    {
      private static readonly ConfigViewGenerator _config = new ConfigViewGenerator();
      private bool _generatedViewsMode = true;
      private readonly StorageMappingItemCollection.TryGetUserDefinedQueryView _tryGetUserDefinedQueryView;
      private readonly StorageMappingItemCollection.TryGetUserDefinedQueryViewOfType _tryGetUserDefinedQueryViewOfType;
      private readonly StorageMappingItemCollection _storageMappingItemCollection;
      private readonly Memoizer<System.Data.Entity.Core.Metadata.Edm.EntityContainer, Dictionary<EntitySetBase, GeneratedView>> _generatedViewsMemoizer;
      private readonly Memoizer<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView> _generatedViewOfTypeMemoizer;

      internal ViewDictionary(
        StorageMappingItemCollection storageMappingItemCollection,
        out Dictionary<EntitySetBase, GeneratedView> userDefinedQueryViewsDict,
        out Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView> userDefinedQueryViewsOfTypeDict)
      {
        this._storageMappingItemCollection = storageMappingItemCollection;
        this._generatedViewsMemoizer = new Memoizer<System.Data.Entity.Core.Metadata.Edm.EntityContainer, Dictionary<EntitySetBase, GeneratedView>>(new Func<System.Data.Entity.Core.Metadata.Edm.EntityContainer, Dictionary<EntitySetBase, GeneratedView>>(this.SerializedGetGeneratedViews), (IEqualityComparer<System.Data.Entity.Core.Metadata.Edm.EntityContainer>) null);
        this._generatedViewOfTypeMemoizer = new Memoizer<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView>(new Func<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView>(this.SerializedGeneratedViewOfType), (IEqualityComparer<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>>) Pair<EntitySetBase, Pair<EntityTypeBase, bool>>.PairComparer.Instance);
        userDefinedQueryViewsDict = new Dictionary<EntitySetBase, GeneratedView>((IEqualityComparer<EntitySetBase>) EqualityComparer<EntitySetBase>.Default);
        userDefinedQueryViewsOfTypeDict = new Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView>((IEqualityComparer<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>>) Pair<EntitySetBase, Pair<EntityTypeBase, bool>>.PairComparer.Instance);
        this._tryGetUserDefinedQueryView = new StorageMappingItemCollection.TryGetUserDefinedQueryView(userDefinedQueryViewsDict.TryGetValue);
        this._tryGetUserDefinedQueryViewOfType = new StorageMappingItemCollection.TryGetUserDefinedQueryViewOfType(userDefinedQueryViewsOfTypeDict.TryGetValue);
      }

      private Dictionary<EntitySetBase, GeneratedView> SerializedGetGeneratedViews(
        System.Data.Entity.Core.Metadata.Edm.EntityContainer container)
      {
        EntityContainerMapping entityContainerMap = MappingMetadataHelper.GetEntityContainerMap(this._storageMappingItemCollection, container);
        Dictionary<EntitySetBase, GeneratedView> dictionary;
        if (this._generatedViewsMemoizer.TryGetValue(container.DataSpace == DataSpace.CSpace ? entityContainerMap.StorageEntityContainer : entityContainerMap.EdmEntityContainer, out dictionary))
          return dictionary;
        dictionary = new Dictionary<EntitySetBase, GeneratedView>();
        if (!entityContainerMap.HasViews)
          return dictionary;
        if (this._generatedViewsMode && this._storageMappingItemCollection.MappingViewCacheFactory != null)
          this.SerializedCollectViewsFromCache(entityContainerMap, dictionary);
        if (dictionary.Count == 0)
        {
          this._generatedViewsMode = false;
          StorageMappingItemCollection.ViewDictionary.SerializedGenerateViews(entityContainerMap, dictionary);
        }
        return dictionary;
      }

      private static void SerializedGenerateViews(
        EntityContainerMapping entityContainerMap,
        Dictionary<EntitySetBase, GeneratedView> resultDictionary)
      {
        ViewGenResults viewsFromMapping = ViewgenGatekeeper.GenerateViewsFromMapping(entityContainerMap, StorageMappingItemCollection.ViewDictionary._config);
        KeyToListMap<EntitySetBase, GeneratedView> views = viewsFromMapping.Views;
        if (viewsFromMapping.HasErrors)
          throw new MappingException(Helper.CombineErrorMessage(viewsFromMapping.Errors));
        foreach (KeyValuePair<EntitySetBase, List<GeneratedView>> keyValuePair in views.KeyValuePairs)
        {
          GeneratedView generatedView;
          if (!resultDictionary.TryGetValue(keyValuePair.Key, out generatedView))
          {
            generatedView = keyValuePair.Value[0];
            resultDictionary.Add(keyValuePair.Key, generatedView);
          }
        }
      }

      private bool TryGenerateQueryViewOfType(
        System.Data.Entity.Core.Metadata.Edm.EntityContainer entityContainer,
        EntitySetBase entity,
        EntityTypeBase type,
        bool includeSubtypes,
        out GeneratedView generatedView)
      {
        if (type.Abstract)
        {
          generatedView = (GeneratedView) null;
          return false;
        }
        bool success;
        ViewGenResults specificQueryView = ViewgenGatekeeper.GenerateTypeSpecificQueryView(MappingMetadataHelper.GetEntityContainerMap(this._storageMappingItemCollection, entityContainer), StorageMappingItemCollection.ViewDictionary._config, entity, type, includeSubtypes, out success);
        if (!success)
        {
          generatedView = (GeneratedView) null;
          return false;
        }
        KeyToListMap<EntitySetBase, GeneratedView> views = specificQueryView.Views;
        if (specificQueryView.HasErrors)
          throw new MappingException(Helper.CombineErrorMessage(specificQueryView.Errors));
        generatedView = views.AllValues.First<GeneratedView>();
        return true;
      }

      internal bool TryGetGeneratedViewOfType(
        EntitySetBase entity,
        EntityTypeBase type,
        bool includeSubtypes,
        out GeneratedView generatedView)
      {
        Pair<EntitySetBase, Pair<EntityTypeBase, bool>> pair = new Pair<EntitySetBase, Pair<EntityTypeBase, bool>>(entity, new Pair<EntityTypeBase, bool>(type, includeSubtypes));
        generatedView = this._generatedViewOfTypeMemoizer.Evaluate(pair);
        return generatedView != null;
      }

      private GeneratedView SerializedGeneratedViewOfType(
        Pair<EntitySetBase, Pair<EntityTypeBase, bool>> arg)
      {
        GeneratedView generatedView;
        if (this._tryGetUserDefinedQueryViewOfType(arg, out generatedView))
          return generatedView;
        EntitySetBase first1 = arg.First;
        EntityTypeBase first2 = arg.Second.First;
        bool second = arg.Second.Second;
        if (!this.TryGenerateQueryViewOfType(first1.EntityContainer, first1, first2, second, out generatedView))
          generatedView = (GeneratedView) null;
        return generatedView;
      }

      internal GeneratedView GetGeneratedView(
        EntitySetBase extent,
        MetadataWorkspace workspace,
        StorageMappingItemCollection storageMappingItemCollection)
      {
        GeneratedView generatedView;
        if (this._tryGetUserDefinedQueryView(extent, out generatedView))
          return generatedView;
        if (extent.BuiltInTypeKind == BuiltInTypeKind.AssociationSet)
        {
          AssociationSet aSet = (AssociationSet) extent;
          if (aSet.ElementType.IsForeignKey)
          {
            if (StorageMappingItemCollection.ViewDictionary._config.IsViewTracing)
            {
              Helpers.StringTraceLine(string.Empty);
              Helpers.StringTraceLine(string.Empty);
              Helpers.FormatTraceLine("================= Generating FK Query View for: {0} =================", (object) aSet.Name);
              Helpers.StringTraceLine(string.Empty);
              Helpers.StringTraceLine(string.Empty);
            }
            System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint rc = aSet.ElementType.ReferentialConstraints.Single<System.Data.Entity.Core.Metadata.Edm.ReferentialConstraint>();
            EntitySet dependentSet = aSet.AssociationSetEnds[rc.ToRole.Name].EntitySet;
            EntitySet principalSet = aSet.AssociationSetEnds[rc.FromRole.Name].EntitySet;
            DbExpression source = (DbExpression) dependentSet.Scan();
            System.Data.Entity.Core.Metadata.Edm.EntityType dependentType = MetadataHelper.GetEntityTypeForEnd((AssociationEndMember) rc.ToRole);
            System.Data.Entity.Core.Metadata.Edm.EntityType principalType = MetadataHelper.GetEntityTypeForEnd((AssociationEndMember) rc.FromRole);
            if (dependentSet.ElementType.IsBaseTypeOf((EdmType) dependentType))
              source = (DbExpression) source.OfType(TypeUsage.Create((EdmType) dependentType));
            if (rc.FromRole.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne)
              source = (DbExpression) source.Where((Func<DbExpression, DbExpression>) (e =>
              {
                DbExpression left = (DbExpression) null;
                foreach (EdmProperty toProperty in rc.ToProperties)
                {
                  DbExpression right = (DbExpression) e.Property(toProperty).IsNull().Not();
                  left = left == null ? right : (DbExpression) left.And(right);
                }
                return left;
              }));
            DbExpression query = (DbExpression) source.Select<DbNewInstanceExpression>((Func<DbExpression, DbNewInstanceExpression>) (e =>
            {
              List<DbExpression> dbExpressionList = new List<DbExpression>();
              foreach (EdmMember associationEndMember in aSet.ElementType.AssociationEndMembers)
              {
                if (associationEndMember.Name == rc.ToRole.Name)
                {
                  List<KeyValuePair<string, DbExpression>> keyValuePairList = new List<KeyValuePair<string, DbExpression>>();
                  foreach (EdmMember keyMember in dependentSet.ElementType.KeyMembers)
                    keyValuePairList.Add((KeyValuePair<string, DbExpression>) e.Property((EdmProperty) keyMember));
                  dbExpressionList.Add((DbExpression) dependentSet.RefFromKey((DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) keyValuePairList), dependentType));
                }
                else
                {
                  List<KeyValuePair<string, DbExpression>> keyValuePairList = new List<KeyValuePair<string, DbExpression>>();
                  foreach (EdmProperty keyMember in principalSet.ElementType.KeyMembers)
                  {
                    int index = rc.FromProperties.IndexOf(keyMember);
                    keyValuePairList.Add((KeyValuePair<string, DbExpression>) e.Property(rc.ToProperties[index]));
                  }
                  dbExpressionList.Add((DbExpression) principalSet.RefFromKey((DbExpression) DbExpressionBuilder.NewRow((IEnumerable<KeyValuePair<string, DbExpression>>) keyValuePairList), principalType));
                }
              }
              return TypeUsage.Create((EdmType) aSet.ElementType).New((IEnumerable<DbExpression>) dbExpressionList);
            }));
            return GeneratedView.CreateGeneratedViewForFKAssociationSet((EntitySetBase) aSet, (EdmType) aSet.ElementType, new DbQueryCommandTree(workspace, DataSpace.SSpace, query), storageMappingItemCollection, StorageMappingItemCollection.ViewDictionary._config);
          }
        }
        if (!this._generatedViewsMemoizer.Evaluate(extent.EntityContainer).TryGetValue(extent, out generatedView))
          throw new InvalidOperationException(Strings.Mapping_Views_For_Extent_Not_Generated(extent.EntityContainer.DataSpace == DataSpace.SSpace ? (object) "Table" : (object) "EntitySet", (object) extent.Name));
        return generatedView;
      }

      private void SerializedCollectViewsFromCache(
        EntityContainerMapping containerMapping,
        Dictionary<EntitySetBase, GeneratedView> extentMappingViews)
      {
        DbMappingViewCache mappingViewCache = this._storageMappingItemCollection.MappingViewCacheFactory.Create(containerMapping);
        if (mappingViewCache == null)
          return;
        if (MetadataMappingHasherVisitor.GetMappingClosureHash(containerMapping.StorageMappingItemCollection.MappingVersion, containerMapping, true) != mappingViewCache.MappingHashValue)
          throw new MappingException(Strings.ViewGen_HashOnMappingClosure_Not_Matching((object) mappingViewCache.GetType().Name));
        foreach (EntitySetBase entitySetBase in containerMapping.StorageEntityContainer.BaseEntitySets.Union<EntitySetBase>((IEnumerable<EntitySetBase>) containerMapping.EdmEntityContainer.BaseEntitySets))
        {
          GeneratedView generatedView;
          if (!extentMappingViews.TryGetValue(entitySetBase, out generatedView))
          {
            DbMappingView view = mappingViewCache.GetView(entitySetBase);
            if (view != null)
            {
              generatedView = GeneratedView.CreateGeneratedView(entitySetBase, (EdmType) null, (DbQueryCommandTree) null, view.EntitySql, this._storageMappingItemCollection, new ConfigViewGenerator());
              extentMappingViews.Add(entitySetBase, generatedView);
            }
          }
        }
      }
    }

    internal enum InterestingMembersKind
    {
      RequiredOriginalValueMembers,
      FullUpdate,
      PartialUpdate,
    }
  }
}
