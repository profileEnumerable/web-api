// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.EntitySql;
using System.Data.Entity.Core.Common.QueryCache;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Mapping.Update.Internal;
using System.Data.Entity.Core.Mapping.ViewGeneration;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Core.Objects.ELinq;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Runtime Metadata Workspace</summary>
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
  public class MetadataWorkspace
  {
    private static readonly double _maximumEdmVersionSupported = MetadataWorkspace.SupportedEdmVersions.Last<double>();
    private readonly object _schemaVersionLock = new object();
    private readonly Guid _metadataWorkspaceId = Guid.NewGuid();
    private Lazy<EdmItemCollection> _itemsCSpace;
    private Lazy<StoreItemCollection> _itemsSSpace;
    private Lazy<ObjectItemCollection> _itemsOSpace;
    private Lazy<StorageMappingItemCollection> _itemsCSSpace;
    private Lazy<DefaultObjectMappingItemCollection> _itemsOCSpace;
    private bool _foundAssemblyWithAttribute;
    private double _schemaVersion;
    internal readonly MetadataOptimization MetadataOptimization;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> class.
    /// </summary>
    public MetadataWorkspace()
    {
      this._itemsOSpace = new Lazy<ObjectItemCollection>((Func<ObjectItemCollection>) (() => new ObjectItemCollection()), true);
      this.MetadataOptimization = new MetadataOptimization(this);
    }

    /// <summary>
    /// Constructs a <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> with loaders for all item collections (<see cref="T:System.Data.Entity.Core.Metadata.Edm.ItemCollection" />)
    /// needed by EF except the o/c mapping which will be created automatically based on the given o-space and c-space
    /// loaders. The item collection delegates are executed lazily when a given collection is used for the first
    /// time. It is acceptable to pass a delegate that returns null if the collection will never be used, but this
    /// is rarely done, and any attempt by EF to use the collection in such cases will result in an exception.
    /// </summary>
    /// <param name="cSpaceLoader">Delegate to return the c-space (CSDL) item collection.</param>
    /// <param name="sSpaceLoader">Delegate to return the s-space (SSDL) item collection.</param>
    /// <param name="csMappingLoader">Delegate to return the c/s mapping (MSL) item collection.</param>
    /// <param name="oSpaceLoader">Delegate to return the o-space item collection.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "o")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "s")]
    public MetadataWorkspace(
      Func<EdmItemCollection> cSpaceLoader,
      Func<StoreItemCollection> sSpaceLoader,
      Func<StorageMappingItemCollection> csMappingLoader,
      Func<ObjectItemCollection> oSpaceLoader)
    {
      MetadataWorkspace metadataWorkspace = this;
      Check.NotNull<Func<EdmItemCollection>>(cSpaceLoader, nameof (cSpaceLoader));
      Check.NotNull<Func<StoreItemCollection>>(sSpaceLoader, nameof (sSpaceLoader));
      Check.NotNull<Func<StorageMappingItemCollection>>(csMappingLoader, nameof (csMappingLoader));
      Check.NotNull<Func<ObjectItemCollection>>(oSpaceLoader, nameof (oSpaceLoader));
      this._itemsCSpace = new Lazy<EdmItemCollection>((Func<EdmItemCollection>) (() => metadataWorkspace.LoadAndCheckItemCollection<EdmItemCollection>(cSpaceLoader)), true);
      this._itemsSSpace = new Lazy<StoreItemCollection>((Func<StoreItemCollection>) (() => metadataWorkspace.LoadAndCheckItemCollection<StoreItemCollection>(sSpaceLoader)), true);
      this._itemsOSpace = new Lazy<ObjectItemCollection>(oSpaceLoader, true);
      this._itemsCSSpace = new Lazy<StorageMappingItemCollection>((Func<StorageMappingItemCollection>) (() => metadataWorkspace.LoadAndCheckItemCollection<StorageMappingItemCollection>(csMappingLoader)), true);
      this._itemsOCSpace = new Lazy<DefaultObjectMappingItemCollection>((Func<DefaultObjectMappingItemCollection>) (() => new DefaultObjectMappingItemCollection(this._itemsCSpace.Value, this._itemsOSpace.Value)), true);
      this.MetadataOptimization = new MetadataOptimization(this);
    }

    /// <summary>
    /// Constructs a <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> with loaders for all item collections (<see cref="T:System.Data.Entity.Core.Metadata.Edm.ItemCollection" />)
    /// that come from traditional EDMX mapping. Default o-space and o/c mapping collections will be used.
    /// The item collection delegates are executed lazily when a given collection is used for the first
    /// time. It is acceptable to pass a delegate that returns null if the collection will never be used, but this
    /// is rarely done, and any attempt by EF to use the collection in such cases will result in an exception.
    /// </summary>
    /// <param name="cSpaceLoader">Delegate to return the c-space (CSDL) item collection.</param>
    /// <param name="sSpaceLoader">Delegate to return the s-space (SSDL) item collection.</param>
    /// <param name="csMappingLoader">Delegate to return the c/s mapping (MSL) item collection.</param>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "s")]
    public MetadataWorkspace(
      Func<EdmItemCollection> cSpaceLoader,
      Func<StoreItemCollection> sSpaceLoader,
      Func<StorageMappingItemCollection> csMappingLoader)
    {
      MetadataWorkspace metadataWorkspace = this;
      Check.NotNull<Func<EdmItemCollection>>(cSpaceLoader, nameof (cSpaceLoader));
      Check.NotNull<Func<StoreItemCollection>>(sSpaceLoader, nameof (sSpaceLoader));
      Check.NotNull<Func<StorageMappingItemCollection>>(csMappingLoader, nameof (csMappingLoader));
      this._itemsCSpace = new Lazy<EdmItemCollection>((Func<EdmItemCollection>) (() => metadataWorkspace.LoadAndCheckItemCollection<EdmItemCollection>(cSpaceLoader)), true);
      this._itemsSSpace = new Lazy<StoreItemCollection>((Func<StoreItemCollection>) (() => metadataWorkspace.LoadAndCheckItemCollection<StoreItemCollection>(sSpaceLoader)), true);
      this._itemsOSpace = new Lazy<ObjectItemCollection>((Func<ObjectItemCollection>) (() => new ObjectItemCollection()), true);
      this._itemsCSSpace = new Lazy<StorageMappingItemCollection>((Func<StorageMappingItemCollection>) (() => metadataWorkspace.LoadAndCheckItemCollection<StorageMappingItemCollection>(csMappingLoader)), true);
      this._itemsOCSpace = new Lazy<DefaultObjectMappingItemCollection>((Func<DefaultObjectMappingItemCollection>) (() => new DefaultObjectMappingItemCollection(this._itemsCSpace.Value, this._itemsOSpace.Value)), true);
      this.MetadataOptimization = new MetadataOptimization(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> class using the specified paths and assemblies.
    /// </summary>
    /// <param name="paths">The paths to workspace metadata.</param>
    /// <param name="assembliesToConsider">The names of assemblies used to construct workspace.</param>
    [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    public MetadataWorkspace(IEnumerable<string> paths, IEnumerable<Assembly> assembliesToConsider)
    {
      Check.NotNull<IEnumerable<string>>(paths, nameof (paths));
      Check.NotNull<IEnumerable<Assembly>>(assembliesToConsider, nameof (assembliesToConsider));
      EntityUtil.CheckArgumentContainsNull<string>(ref paths, nameof (paths));
      EntityUtil.CheckArgumentContainsNull<Assembly>(ref assembliesToConsider, nameof (assembliesToConsider));
      Func<AssemblyName, Assembly> resolveReference = (Func<AssemblyName, Assembly>) (referenceName =>
      {
        foreach (Assembly assembly in assembliesToConsider)
        {
          if (AssemblyName.ReferenceMatchesDefinition(referenceName, new AssemblyName(assembly.FullName)))
            return assembly;
        }
        throw new ArgumentException(Strings.AssemblyMissingFromAssembliesToConsider((object) referenceName.FullName), nameof (assembliesToConsider));
      });
      this.CreateMetadataWorkspaceWithResolver(paths, (Func<IEnumerable<Assembly>>) (() => assembliesToConsider), resolveReference);
      this.MetadataOptimization = new MetadataOptimization(this);
    }

    private void CreateMetadataWorkspaceWithResolver(
      IEnumerable<string> paths,
      Func<IEnumerable<Assembly>> wildcardAssemblies,
      Func<AssemblyName, Assembly> resolveReference)
    {
      MetadataArtifactLoader compositeFromFilePaths = MetadataArtifactLoader.CreateCompositeFromFilePaths((IEnumerable<string>) paths.ToArray<string>(), "", (MetadataArtifactAssemblyResolver) new CustomAssemblyResolver(wildcardAssemblies, resolveReference));
      this._itemsOSpace = new Lazy<ObjectItemCollection>((Func<ObjectItemCollection>) (() => new ObjectItemCollection()), true);
      using (DisposableCollectionWrapper<XmlReader> source = new DisposableCollectionWrapper<XmlReader>((IEnumerable<XmlReader>) compositeFromFilePaths.CreateReaders(DataSpace.CSpace)))
      {
        if (source.Any<XmlReader>())
        {
          EdmItemCollection itemCollection = new EdmItemCollection((IEnumerable<XmlReader>) source, (IEnumerable<string>) compositeFromFilePaths.GetPaths(DataSpace.CSpace), false);
          this._itemsCSpace = new Lazy<EdmItemCollection>((Func<EdmItemCollection>) (() => itemCollection), true);
          this._itemsOCSpace = new Lazy<DefaultObjectMappingItemCollection>((Func<DefaultObjectMappingItemCollection>) (() => new DefaultObjectMappingItemCollection(itemCollection, this._itemsOSpace.Value)), true);
        }
      }
      using (DisposableCollectionWrapper<XmlReader> source = new DisposableCollectionWrapper<XmlReader>((IEnumerable<XmlReader>) compositeFromFilePaths.CreateReaders(DataSpace.SSpace)))
      {
        if (source.Any<XmlReader>())
        {
          StoreItemCollection itemCollection = new StoreItemCollection((IEnumerable<XmlReader>) source, (IEnumerable<string>) compositeFromFilePaths.GetPaths(DataSpace.SSpace));
          this._itemsSSpace = new Lazy<StoreItemCollection>((Func<StoreItemCollection>) (() => itemCollection), true);
        }
      }
      using (DisposableCollectionWrapper<XmlReader> source = new DisposableCollectionWrapper<XmlReader>((IEnumerable<XmlReader>) compositeFromFilePaths.CreateReaders(DataSpace.CSSpace)))
      {
        if (!source.Any<XmlReader>() || this._itemsCSpace == null || this._itemsSSpace == null)
          return;
        StorageMappingItemCollection mapping = new StorageMappingItemCollection(this._itemsCSpace.Value, this._itemsSSpace.Value, (IEnumerable<XmlReader>) source, (IList<string>) compositeFromFilePaths.GetPaths(DataSpace.CSSpace));
        this._itemsCSSpace = new Lazy<StorageMappingItemCollection>((Func<StorageMappingItemCollection>) (() => mapping), true);
      }
    }

    private static IEnumerable<double> SupportedEdmVersions
    {
      get
      {
        yield return 0.0;
        yield return 1.0;
        yield return 2.0;
        yield return 3.0;
      }
    }

    /// <summary>
    /// The Max EDM version thats going to be supported by the runtime.
    /// </summary>
    public static double MaximumEdmVersionSupported
    {
      get
      {
        return MetadataWorkspace._maximumEdmVersionSupported;
      }
    }

    internal virtual Guid MetadataWorkspaceId
    {
      get
      {
        return this._metadataWorkspaceId;
      }
    }

    /// <summary>
    /// Creates an <see cref="T:System.Data.Entity.Core.Common.EntitySql.EntitySqlParser" /> configured to use the
    /// <see cref="F:System.Data.Entity.Core.Metadata.Edm.DataSpace.CSpace" />
    /// data space.
    /// </summary>
    /// <returns>The created parser object.</returns>
    public virtual EntitySqlParser CreateEntitySqlParser()
    {
      return new EntitySqlParser((Perspective) new ModelPerspective(this));
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbQueryCommandTree" /> bound to this metadata workspace based on the specified query expression.
    /// </summary>
    /// <returns>
    /// A new <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbQueryCommandTree" /> with the specified expression as it's
    /// <see cref="P:System.Data.Entity.Core.Common.CommandTrees.DbQueryCommandTree.Query" />
    /// property.
    /// </returns>
    /// <param name="query">
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that defines the query.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// If
    /// <paramref name="query" />
    /// is null
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// If
    /// <paramref name="query" />
    /// contains metadata that cannot be resolved in this metadata workspace
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// If
    /// <paramref name="query" />
    /// is not structurally valid because it contains unresolvable variable references
    /// </exception>
    public virtual DbQueryCommandTree CreateQueryCommandTree(DbExpression query)
    {
      return new DbQueryCommandTree(this, DataSpace.CSpace, query);
    }

    /// <summary>
    /// Gets <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> items.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" /> items.
    /// </returns>
    /// <param name="dataSpace">
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.DataSpace" /> from which to retrieve items.
    /// </param>
    public virtual ItemCollection GetItemCollection(DataSpace dataSpace)
    {
      return this.GetItemCollection(dataSpace, true);
    }

    /// <summary>Registers the item collection with each associated data model.</summary>
    /// <param name="collection">The output parameter collection that needs to be filled up.</param>
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [Obsolete("Construct MetadataWorkspace using constructor that accepts metadata loading delegates.")]
    public virtual void RegisterItemCollection(ItemCollection collection)
    {
      Check.NotNull<ItemCollection>(collection, nameof (collection));
      try
      {
        switch (collection.DataSpace)
        {
          case DataSpace.OSpace:
            this._itemsOSpace = new Lazy<ObjectItemCollection>((Func<ObjectItemCollection>) (() => (ObjectItemCollection) collection), true);
            if (this._itemsOCSpace != null || this._itemsCSpace == null)
              break;
            this._itemsOCSpace = new Lazy<DefaultObjectMappingItemCollection>((Func<DefaultObjectMappingItemCollection>) (() => new DefaultObjectMappingItemCollection(this._itemsCSpace.Value, this._itemsOSpace.Value)));
            break;
          case DataSpace.CSpace:
            EdmItemCollection edmCollection = (EdmItemCollection) collection;
            if (!MetadataWorkspace.SupportedEdmVersions.Contains<double>(edmCollection.EdmVersion))
              throw new InvalidOperationException(Strings.EdmVersionNotSupportedByRuntime((object) edmCollection.EdmVersion, (object) Helper.GetCommaDelimitedString(MetadataWorkspace.SupportedEdmVersions.Where<double>((Func<double, bool>) (e => e != 0.0)).Select<double, string>((Func<double, string>) (e => e.ToString((IFormatProvider) CultureInfo.InvariantCulture))))));
            this.CheckAndSetItemCollectionVersionInWorkSpace(collection);
            this._itemsCSpace = new Lazy<EdmItemCollection>((Func<EdmItemCollection>) (() => edmCollection), true);
            if (this._itemsOCSpace != null)
              break;
            this._itemsOCSpace = new Lazy<DefaultObjectMappingItemCollection>((Func<DefaultObjectMappingItemCollection>) (() => new DefaultObjectMappingItemCollection(edmCollection, this._itemsOSpace.Value)));
            break;
          case DataSpace.SSpace:
            this.CheckAndSetItemCollectionVersionInWorkSpace(collection);
            this._itemsSSpace = new Lazy<StoreItemCollection>((Func<StoreItemCollection>) (() => (StoreItemCollection) collection), true);
            break;
          case DataSpace.CSSpace:
            this.CheckAndSetItemCollectionVersionInWorkSpace(collection);
            this._itemsCSSpace = new Lazy<StorageMappingItemCollection>((Func<StorageMappingItemCollection>) (() => (StorageMappingItemCollection) collection), true);
            break;
          default:
            this._itemsOCSpace = new Lazy<DefaultObjectMappingItemCollection>((Func<DefaultObjectMappingItemCollection>) (() => (DefaultObjectMappingItemCollection) collection), true);
            break;
        }
      }
      catch (InvalidCastException ex)
      {
        throw new System.Data.Entity.Core.MetadataException(Strings.InvalidCollectionForMapping((object) collection.DataSpace.ToString()));
      }
    }

    private T LoadAndCheckItemCollection<T>(Func<T> itemCollectionLoader) where T : ItemCollection
    {
      T obj = itemCollectionLoader();
      if ((object) obj != null)
        this.CheckAndSetItemCollectionVersionInWorkSpace((ItemCollection) obj);
      return obj;
    }

    private void CheckAndSetItemCollectionVersionInWorkSpace(ItemCollection itemCollectionToRegister)
    {
      double num = 0.0;
      string str = (string) null;
      switch (itemCollectionToRegister.DataSpace)
      {
        case DataSpace.CSpace:
          num = ((EdmItemCollection) itemCollectionToRegister).EdmVersion;
          str = "EdmItemCollection";
          break;
        case DataSpace.SSpace:
          num = ((StoreItemCollection) itemCollectionToRegister).StoreSchemaVersion;
          str = "StoreItemCollection";
          break;
        case DataSpace.CSSpace:
          num = ((StorageMappingItemCollection) itemCollectionToRegister).MappingVersion;
          str = "StorageMappingItemCollection";
          break;
      }
      lock (this._schemaVersionLock)
      {
        if (num != this._schemaVersion && num != 0.0 && this._schemaVersion != 0.0)
          throw new System.Data.Entity.Core.MetadataException(Strings.DifferentSchemaVersionInCollection((object) str, (object) num, (object) this._schemaVersion));
        this._schemaVersion = num;
      }
    }

    /// <summary>Loads metadata from the given assembly.</summary>
    /// <param name="assembly">The assembly from which the metadata will be loaded.</param>
    public virtual void LoadFromAssembly(Assembly assembly)
    {
      this.LoadFromAssembly(assembly, (Action<string>) null);
    }

    /// <summary>Loads metadata from the given assembly.</summary>
    /// <param name="assembly">The assembly from which the metadata will be loaded.</param>
    /// <param name="logLoadMessage">The delegate for logging the load messages.</param>
    public virtual void LoadFromAssembly(Assembly assembly, Action<string> logLoadMessage)
    {
      Check.NotNull<Assembly>(assembly, nameof (assembly));
      ObjectItemCollection itemCollection = (ObjectItemCollection) this.GetItemCollection(DataSpace.OSpace);
      this.ExplicitLoadFromAssembly(assembly, itemCollection, logLoadMessage);
    }

    private void ExplicitLoadFromAssembly(
      Assembly assembly,
      ObjectItemCollection collection,
      Action<string> logLoadMessage)
    {
      ItemCollection collection1;
      if (!this.TryGetItemCollection(DataSpace.CSpace, out collection1))
        collection1 = (ItemCollection) null;
      collection.ExplicitLoadFromAssembly(assembly, (EdmItemCollection) collection1, logLoadMessage);
    }

    private void ImplicitLoadFromAssembly(Assembly assembly, ObjectItemCollection collection)
    {
      if (MetadataAssemblyHelper.ShouldFilterAssembly(assembly))
        return;
      this.ExplicitLoadFromAssembly(assembly, collection, (Action<string>) null);
    }

    internal virtual void ImplicitLoadAssemblyForType(Type type, Assembly callingAssembly)
    {
      ItemCollection collection1;
      if (!this.TryGetItemCollection(DataSpace.OSpace, out collection1))
        return;
      ObjectItemCollection collection2 = (ObjectItemCollection) collection1;
      ItemCollection collection3;
      this.TryGetItemCollection(DataSpace.CSpace, out collection3);
      EdmItemCollection edmItemCollection = (EdmItemCollection) collection3;
      if (collection2.ImplicitLoadAssemblyForType(type, edmItemCollection) || !((Assembly) null != callingAssembly))
        return;
      if (ObjectItemAttributeAssemblyLoader.IsSchemaAttributePresent(callingAssembly) || this._foundAssemblyWithAttribute || MetadataAssemblyHelper.GetNonSystemReferencedAssemblies(callingAssembly).Any<Assembly>(new Func<Assembly, bool>(ObjectItemAttributeAssemblyLoader.IsSchemaAttributePresent)))
      {
        this._foundAssemblyWithAttribute = true;
        collection2.ImplicitLoadAllReferencedAssemblies(callingAssembly, edmItemCollection);
      }
      else
        this.ImplicitLoadFromAssembly(callingAssembly, collection2);
    }

    internal virtual void ImplicitLoadFromEntityType(EntityType type, Assembly callingAssembly)
    {
      MappingBase map;
      if (this.TryGetMap((GlobalItem) type, DataSpace.OCSpace, out map))
        return;
      this.ImplicitLoadAssemblyForType(typeof (IEntityWithKey), callingAssembly);
      ObjectItemCollection itemCollection = this.GetItemCollection(DataSpace.OSpace) as ObjectItemCollection;
      EdmType edmType;
      if (itemCollection == null || !itemCollection.TryGetOSpaceType((EdmType) type, out edmType))
        throw new InvalidOperationException(Strings.Mapping_Object_InvalidType((object) type.Identity));
    }

    /// <summary>Returns an item by using the specified identity and the data model.</summary>
    /// <returns>The item that matches the given identity in the specified data model.</returns>
    /// <param name="identity">The identity of the item.</param>
    /// <param name="dataSpace">The conceptual model in which the item is searched.</param>
    /// <typeparam name="T">The type returned by the method.</typeparam>
    /// <exception cref="T:System.ArgumentException">Thrown if the space is not a valid space. Valid space is either C, O, CS or OCSpace</exception>
    public virtual T GetItem<T>(string identity, DataSpace dataSpace) where T : GlobalItem
    {
      return this.GetItemCollection(dataSpace, true).GetItem<T>(identity, false);
    }

    /// <summary>Returns an item by using the specified identity and the data model.</summary>
    /// <returns>true if there is an item that matches the search criteria; otherwise, false.</returns>
    /// <param name="identity">The conceptual model on which the item is searched.</param>
    /// <param name="space">The conceptual model on which the item is searched.</param>
    /// <param name="item">
    /// When this method returns, contains a <see cref="T:System.Data.Metadata.Edm.GlobalIem" /> object. This parameter is passed uninitialized.
    /// </param>
    /// <typeparam name="T">The type returned by the method.</typeparam>
    public virtual bool TryGetItem<T>(string identity, DataSpace space, out T item) where T : GlobalItem
    {
      item = default (T);
      ItemCollection itemCollection = this.GetItemCollection(space, false);
      if (itemCollection != null)
        return itemCollection.TryGetItem<T>(identity, false, out item);
      return false;
    }

    /// <summary>Returns an item by using the specified identity and the data model.</summary>
    /// <returns>The item that matches the given identity in the specified data model.</returns>
    /// <param name="identity">The identity of the item.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <param name="dataSpace">The conceptual model on which the item is searched.</param>
    /// <typeparam name="T">The type returned by the method.</typeparam>
    /// <exception cref="T:System.ArgumentException">Thrown if the space is not a valid space. Valid space is either C, O, CS or OCSpace</exception>
    public virtual T GetItem<T>(string identity, bool ignoreCase, DataSpace dataSpace) where T : GlobalItem
    {
      return this.GetItemCollection(dataSpace, true).GetItem<T>(identity, ignoreCase);
    }

    /// <summary>Returns an item by using the specified identity and the data model.</summary>
    /// <returns>true if there is an item that matches the search criteria; otherwise, false.</returns>
    /// <param name="identity">The conceptual model on which the item is searched.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <param name="dataSpace">The conceptual model on which the item is searched.</param>
    /// <param name="item">
    /// When this method returns, contains a <see cref="T:System.Data.Metadata.Edm.GlobalIem" /> object. This parameter is passed uninitialized.
    /// </param>
    /// <typeparam name="T">The type returned by the method.</typeparam>
    public virtual bool TryGetItem<T>(
      string identity,
      bool ignoreCase,
      DataSpace dataSpace,
      out T item)
      where T : GlobalItem
    {
      item = default (T);
      ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
      if (itemCollection != null)
        return itemCollection.TryGetItem<T>(identity, ignoreCase, out item);
      return false;
    }

    /// <summary>Gets all the items in the specified data model.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains all the items in the specified data model.
    /// </returns>
    /// <param name="dataSpace">The conceptual model for which the list of items is needed.</param>
    /// <typeparam name="T">The type returned by the method.</typeparam>
    /// <exception cref="T:System.ArgumentException">Thrown if the space is not a valid space. Valid space is either C, O, CS or OCSpace</exception>
    public virtual ReadOnlyCollection<T> GetItems<T>(DataSpace dataSpace) where T : GlobalItem
    {
      return this.GetItemCollection(dataSpace, true).GetItems<T>();
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object by using the specified type name, namespace name, and data model.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object that represents the type that matches the given type name and the namespace name in the specified data model. If there is no matched type, this method returns null.
    /// </returns>
    /// <param name="name">The name of the type.</param>
    /// <param name="namespaceName">The namespace of the type.</param>
    /// <param name="dataSpace">The conceptual model on which the type is searched.</param>
    /// <exception cref="T:System.ArgumentException">Thrown if the space is not a valid space. Valid space is either C, O, CS or OCSpace</exception>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "GetType")]
    public virtual EdmType GetType(string name, string namespaceName, DataSpace dataSpace)
    {
      return this.GetItemCollection(dataSpace, true).GetType(name, namespaceName, false);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object by using the specified type name, namespace name, and data model.
    /// </summary>
    /// <returns>true if there is a type that matches the search criteria; otherwise, false.</returns>
    /// <param name="name">The name of the type.</param>
    /// <param name="namespaceName">The namespace of the type.</param>
    /// <param name="dataSpace">The conceptual model on which the type is searched.</param>
    /// <param name="type">
    /// When this method returns, contains an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object. This parameter is passed uninitialized.
    /// </param>
    public virtual bool TryGetType(
      string name,
      string namespaceName,
      DataSpace dataSpace,
      out EdmType type)
    {
      type = (EdmType) null;
      ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
      if (itemCollection != null)
        return itemCollection.TryGetType(name, namespaceName, false, out type);
      return false;
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object by using the specified type name, namespace name, and data model.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object.
    /// </returns>
    /// <param name="name">The name of the type.</param>
    /// <param name="namespaceName">The namespace of the type.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <param name="dataSpace">The conceptual model on which the type is searched.</param>
    /// <exception cref="T:System.ArgumentException">Thrown if the space is not a valid space. Valid space is either C, O, CS or OCSpace</exception>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "GetType")]
    public virtual EdmType GetType(
      string name,
      string namespaceName,
      bool ignoreCase,
      DataSpace dataSpace)
    {
      return this.GetItemCollection(dataSpace, true).GetType(name, namespaceName, ignoreCase);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object by using the specified type name, namespace name, and data model.
    /// </summary>
    /// <returns>true if there is a type that matches the search criteria; otherwise, false.</returns>
    /// <param name="name">The name of the type.</param>
    /// <param name="namespaceName">The namespace of the type.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <param name="dataSpace">The conceptual model on which the type is searched.</param>
    /// <param name="type">
    /// When this method returns, contains an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" /> object. This parameter is passed uninitialized.
    /// </param>
    public virtual bool TryGetType(
      string name,
      string namespaceName,
      bool ignoreCase,
      DataSpace dataSpace,
      out EdmType type)
    {
      type = (EdmType) null;
      ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
      if (itemCollection != null)
        return itemCollection.TryGetType(name, namespaceName, ignoreCase, out type);
      return false;
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object by using the specified entity container name and the data model.
    /// </summary>
    /// <returns>If there is no entity container, this method returns null; otherwise, it returns the first entity container.</returns>
    /// <param name="name">The name of the entity container.</param>
    /// <param name="dataSpace">The conceptual model on which the entity container is searched.</param>
    /// <exception cref="T:System.ArgumentException">Thrown if the space is not a valid space. Valid space is either C, O, CS or OCSpace</exception>
    public virtual EntityContainer GetEntityContainer(
      string name,
      DataSpace dataSpace)
    {
      return this.GetItemCollection(dataSpace, true).GetEntityContainer(name);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object by using the specified entity container name and the data model.
    /// </summary>
    /// <returns>true if there is an entity container that matches the search criteria; otherwise, false.</returns>
    /// <param name="name">The name of the entity container.</param>
    /// <param name="dataSpace">The conceptual model on which the entity container is searched.</param>
    /// <param name="entityContainer">
    /// When this method returns, contains an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object. If there is no entity container, this output parameter contains null; otherwise, it returns the first entity container. This parameter is passed uninitialized.
    /// </param>
    public virtual bool TryGetEntityContainer(
      string name,
      DataSpace dataSpace,
      out EntityContainer entityContainer)
    {
      entityContainer = (EntityContainer) null;
      Check.NotNull<string>(name, nameof (name));
      ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
      if (itemCollection != null)
        return itemCollection.TryGetEntityContainer(name, out entityContainer);
      return false;
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object by using the specified entity container name and the data model.
    /// </summary>
    /// <returns>If there is no entity container, this method returns null; otherwise, it returns the first entity container.</returns>
    /// <param name="name">The name of the entity container.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <param name="dataSpace">The conceptual model on which the entity container is searched.</param>
    /// <exception cref="T:System.ArgumentException">Thrown if the space is not a valid space. Valid space is either C, O, CS or OCSpace</exception>
    public virtual EntityContainer GetEntityContainer(
      string name,
      bool ignoreCase,
      DataSpace dataSpace)
    {
      return this.GetItemCollection(dataSpace, true).GetEntityContainer(name, ignoreCase);
    }

    /// <summary>
    /// Returns an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object by using the specified entity container name and the data model.
    /// </summary>
    /// <returns>true if there is an entity container that matches the search criteria; otherwise, false.</returns>
    /// <param name="name">The name of the entity container.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <param name="dataSpace">The conceptual model on which the entity container is searched.</param>
    /// <param name="entityContainer">
    /// When this method returns, contains an <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityContainer" /> object. If there is no entity container, this output parameter contains null; otherwise, it returns the first entity container. This parameter is passed uninitialized.
    /// </param>
    public virtual bool TryGetEntityContainer(
      string name,
      bool ignoreCase,
      DataSpace dataSpace,
      out EntityContainer entityContainer)
    {
      entityContainer = (EntityContainer) null;
      Check.NotNull<string>(name, nameof (name));
      ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
      if (itemCollection != null)
        return itemCollection.TryGetEntityContainer(name, ignoreCase, out entityContainer);
      return false;
    }

    /// <summary>Returns all the overloads of the functions by using the specified name, namespace name, and data model.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains all the functions that match the specified name in a given namespace and a data model.
    /// </returns>
    /// <param name="name">The name of the function.</param>
    /// <param name="namespaceName">The namespace of the function.</param>
    /// <param name="dataSpace">The conceptual model in which the functions are searched.</param>
    /// <exception cref="T:System.ArgumentException">Thrown if the space is not a valid space. Valid space is either C, O, CS or OCSpace</exception>
    public virtual ReadOnlyCollection<EdmFunction> GetFunctions(
      string name,
      string namespaceName,
      DataSpace dataSpace)
    {
      return this.GetFunctions(name, namespaceName, dataSpace, false);
    }

    /// <summary>Returns all the overloads of the functions by using the specified name, namespace name, and data model.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains all the functions that match the specified name in a given namespace and a data model.
    /// </returns>
    /// <param name="name">The name of the function.</param>
    /// <param name="namespaceName">The namespace of the function.</param>
    /// <param name="dataSpace">The conceptual model in which the functions are searched.</param>
    /// <param name="ignoreCase">true to perform the case-insensitive search; otherwise, false.</param>
    /// <exception cref="T:System.ArgumentException">Thrown if the space is not a valid space. Valid space is either C, O, CS or OCSpace</exception>
    public virtual ReadOnlyCollection<EdmFunction> GetFunctions(
      string name,
      string namespaceName,
      DataSpace dataSpace,
      bool ignoreCase)
    {
      Check.NotEmpty(name, nameof (name));
      Check.NotEmpty(namespaceName, nameof (namespaceName));
      return this.GetItemCollection(dataSpace, true).GetFunctions(namespaceName + "." + name, ignoreCase);
    }

    internal virtual bool TryGetFunction(
      string name,
      string namespaceName,
      TypeUsage[] parameterTypes,
      bool ignoreCase,
      DataSpace dataSpace,
      out EdmFunction function)
    {
      function = (EdmFunction) null;
      Check.NotNull<string>(name, nameof (name));
      Check.NotNull<string>(namespaceName, nameof (namespaceName));
      ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
      if (itemCollection != null)
        return itemCollection.TryGetFunction(namespaceName + "." + name, parameterTypes, ignoreCase, out function);
      return false;
    }

    /// <summary>Returns the list of primitive types in the specified data model.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains all the primitive types in the specified data model.
    /// </returns>
    /// <param name="dataSpace">The data model for which you need the list of primitive types.</param>
    /// <exception cref="T:System.ArgumentException">Thrown if the space is not a valid space. Valid space is either C, O, CS or OCSpace</exception>
    public virtual ReadOnlyCollection<PrimitiveType> GetPrimitiveTypes(
      DataSpace dataSpace)
    {
      return this.GetItemCollection(dataSpace, true).GetItems<PrimitiveType>();
    }

    /// <summary>Gets all the items in the specified data model.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains all the items in the specified data model.
    /// </returns>
    /// <param name="dataSpace">The conceptual model for which the list of items is needed.</param>
    /// <exception cref="T:System.ArgumentException">Thrown if the space is not a valid space. Valid space is either C, O, CS or OCSpace</exception>
    public virtual ReadOnlyCollection<GlobalItem> GetItems(
      DataSpace dataSpace)
    {
      return this.GetItemCollection(dataSpace, true).GetItems<GlobalItem>();
    }

    internal virtual PrimitiveType GetMappedPrimitiveType(
      PrimitiveTypeKind primitiveTypeKind,
      DataSpace dataSpace)
    {
      return this.GetItemCollection(dataSpace, true).GetMappedPrimitiveType(primitiveTypeKind);
    }

    internal virtual bool TryGetMap(
      string typeIdentity,
      DataSpace typeSpace,
      bool ignoreCase,
      DataSpace mappingSpace,
      out MappingBase map)
    {
      map = (MappingBase) null;
      ItemCollection itemCollection = this.GetItemCollection(mappingSpace, false);
      if (itemCollection != null)
        return ((MappingItemCollection) itemCollection).TryGetMap(typeIdentity, typeSpace, ignoreCase, out map);
      return false;
    }

    internal virtual MappingBase GetMap(
      string identity,
      DataSpace typeSpace,
      DataSpace dataSpace)
    {
      return ((MappingItemCollection) this.GetItemCollection(dataSpace, true)).GetMap(identity, typeSpace);
    }

    internal virtual MappingBase GetMap(GlobalItem item, DataSpace dataSpace)
    {
      return ((MappingItemCollection) this.GetItemCollection(dataSpace, true)).GetMap(item);
    }

    internal virtual bool TryGetMap(GlobalItem item, DataSpace dataSpace, out MappingBase map)
    {
      map = (MappingBase) null;
      ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
      if (itemCollection != null)
        return ((MappingItemCollection) itemCollection).TryGetMap(item, out map);
      return false;
    }

    /// <summary>
    /// Tests the retrieval of <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" />.
    /// </summary>
    /// <returns>true if the retrieval was successful; otherwise, false.</returns>
    /// <param name="dataSpace">
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.DataSpace" /> from which to attempt retrieval of
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.MetadataWorkspace" />
    /// .
    /// </param>
    /// <param name="collection">When this method returns, contains the item collection. This parameter is passed uninitialized.</param>
    public virtual bool TryGetItemCollection(DataSpace dataSpace, out ItemCollection collection)
    {
      collection = this.GetItemCollection(dataSpace, false);
      return null != collection;
    }

    internal virtual ItemCollection GetItemCollection(
      DataSpace dataSpace,
      bool required)
    {
      ItemCollection itemCollection;
      switch (dataSpace)
      {
        case DataSpace.OSpace:
          itemCollection = (ItemCollection) this._itemsOSpace.Value;
          break;
        case DataSpace.CSpace:
          itemCollection = this._itemsCSpace == null ? (ItemCollection) null : (ItemCollection) this._itemsCSpace.Value;
          break;
        case DataSpace.SSpace:
          itemCollection = this._itemsSSpace == null ? (ItemCollection) null : (ItemCollection) this._itemsSSpace.Value;
          break;
        case DataSpace.OCSpace:
          itemCollection = this._itemsOCSpace == null ? (ItemCollection) null : (ItemCollection) this._itemsOCSpace.Value;
          break;
        case DataSpace.CSSpace:
          itemCollection = this._itemsCSSpace == null ? (ItemCollection) null : (ItemCollection) this._itemsCSSpace.Value;
          break;
        default:
          int num = required ? 1 : 0;
          itemCollection = (ItemCollection) null;
          break;
      }
      if (required && itemCollection == null)
        throw new InvalidOperationException(Strings.NoCollectionForSpace((object) dataSpace.ToString()));
      return itemCollection;
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the object space type that matches the type supplied by the parameter  edmSpaceType .
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the Object space type. If there is no matched type, this method returns null.
    /// </returns>
    /// <param name="edmSpaceType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// .
    /// </param>
    public virtual StructuralType GetObjectSpaceType(StructuralType edmSpaceType)
    {
      return this.GetObjectSpaceType<StructuralType>(edmSpaceType);
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object via the out parameter  objectSpaceType  that represents the type that matches the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// supplied by the parameter  edmSpaceType .
    /// </summary>
    /// <returns>true if there is a type that matches the search criteria; otherwise, false.</returns>
    /// <param name="edmSpaceType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// .
    /// </param>
    /// <param name="objectSpaceType">
    /// When this method returns, contains a <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the Object space type. This parameter is passed uninitialized.
    /// </param>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public virtual bool TryGetObjectSpaceType(
      StructuralType edmSpaceType,
      out StructuralType objectSpaceType)
    {
      return this.TryGetObjectSpaceType<StructuralType>(edmSpaceType, out objectSpaceType);
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the object space type that matches the type supplied by the parameter  edmSpaceType .
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the Object space type. If there is no matched type, this method returns null.
    /// </returns>
    /// <param name="edmSpaceType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.EnumType" /> object that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// .
    /// </param>
    public virtual EnumType GetObjectSpaceType(EnumType edmSpaceType)
    {
      return this.GetObjectSpaceType<EnumType>(edmSpaceType);
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.Core.Metadata.Edm.EnumType" /> object via the out parameter  objectSpaceType  that represents the type that matches the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// supplied by the parameter  edmSpaceType .
    /// </summary>
    /// <returns>true if there is a type that matches the search criteria; otherwise, false.</returns>
    /// <param name="edmSpaceType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.EnumType" /> object that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// .
    /// </param>
    /// <param name="objectSpaceType">
    /// When this method returns, contains a <see cref="T:System.Data.Entity.Core.Metadata.Edm.EnumType" /> object that represents the Object space type. This parameter is passed uninitialized.
    /// </param>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public virtual bool TryGetObjectSpaceType(EnumType edmSpaceType, out EnumType objectSpaceType)
    {
      return this.TryGetObjectSpaceType<EnumType>(edmSpaceType, out objectSpaceType);
    }

    private T GetObjectSpaceType<T>(T edmSpaceType) where T : EdmType
    {
      T objectSpaceType;
      if (!this.TryGetObjectSpaceType<T>(edmSpaceType, out objectSpaceType))
        throw new ArgumentException(Strings.FailedToFindOSpaceTypeMapping((object) edmSpaceType.Identity));
      return objectSpaceType;
    }

    private bool TryGetObjectSpaceType<T>(T edmSpaceType, out T objectSpaceType) where T : EdmType
    {
      if (edmSpaceType.DataSpace != DataSpace.CSpace)
        throw new ArgumentException(Strings.ArgumentMustBeCSpaceType, nameof (edmSpaceType));
      objectSpaceType = default (T);
      MappingBase map;
      if (this.TryGetMap((GlobalItem) edmSpaceType, DataSpace.OCSpace, out map))
      {
        ObjectTypeMapping objectTypeMapping = map as ObjectTypeMapping;
        if (objectTypeMapping != null)
          objectSpaceType = (T) objectTypeMapping.ClrType;
      }
      return (object) objectSpaceType != null;
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// that matches the type supplied by the parameter  objectSpaceType .
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// . If there is no matched type, this method returns null.
    /// </returns>
    /// <param name="objectSpaceType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> that supplies the type in the object space.
    /// </param>
    public virtual StructuralType GetEdmSpaceType(StructuralType objectSpaceType)
    {
      return this.GetEdmSpaceType<StructuralType>(objectSpaceType);
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object via the out parameter  edmSpaceType  that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// that matches the type supplied by the parameter  objectSpaceType .
    /// </summary>
    /// <returns>true if there is a type that matches the search criteria; otherwise, false.</returns>
    /// <param name="objectSpaceType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the object space type.
    /// </param>
    /// <param name="edmSpaceType">
    /// When this method returns, contains a <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// . This parameter is passed uninitialized.
    /// </param>
    public virtual bool TryGetEdmSpaceType(
      StructuralType objectSpaceType,
      out StructuralType edmSpaceType)
    {
      return this.TryGetEdmSpaceType<StructuralType>(objectSpaceType, out edmSpaceType);
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// that matches the type supplied by the parameter  objectSpaceType .
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> object that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// . If there is no matched type, this method returns null.
    /// </returns>
    /// <param name="objectSpaceType">
    /// A <see cref="T:System.Data.Metadata.Edm.EnumlType" /> that supplies the type in the object space.
    /// </param>
    public virtual EnumType GetEdmSpaceType(EnumType objectSpaceType)
    {
      return this.GetEdmSpaceType<EnumType>(objectSpaceType);
    }

    /// <summary>
    /// Returns a <see cref="T:System.Data.Entity.Core.Metadata.Edm.EnumType" /> object via the out parameter  edmSpaceType  that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// that matches the type supplied by the parameter  objectSpaceType .
    /// </summary>
    /// <returns>true on success, false on failure.</returns>
    /// <param name="objectSpaceType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.EnumType" /> object that represents the object space type.
    /// </param>
    /// <param name="edmSpaceType">
    /// When this method returns, contains a <see cref="T:System.Data.Entity.Core.Metadata.Edm.EnumType" /> object that represents the
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmType" />
    /// . This parameter is passed uninitialized.
    /// </param>
    public virtual bool TryGetEdmSpaceType(EnumType objectSpaceType, out EnumType edmSpaceType)
    {
      return this.TryGetEdmSpaceType<EnumType>(objectSpaceType, out edmSpaceType);
    }

    private T GetEdmSpaceType<T>(T objectSpaceType) where T : EdmType
    {
      T edmSpaceType;
      if (!this.TryGetEdmSpaceType<T>(objectSpaceType, out edmSpaceType))
        throw new ArgumentException(Strings.FailedToFindCSpaceTypeMapping((object) objectSpaceType.Identity));
      return edmSpaceType;
    }

    private bool TryGetEdmSpaceType<T>(T objectSpaceType, out T edmSpaceType) where T : EdmType
    {
      if (objectSpaceType.DataSpace != DataSpace.OSpace)
        throw new ArgumentException(Strings.ArgumentMustBeOSpaceType, nameof (objectSpaceType));
      edmSpaceType = default (T);
      MappingBase map;
      if (this.TryGetMap((GlobalItem) objectSpaceType, DataSpace.OCSpace, out map))
      {
        ObjectTypeMapping objectTypeMapping = map as ObjectTypeMapping;
        if (objectTypeMapping != null)
          edmSpaceType = (T) objectTypeMapping.EdmType;
      }
      return (object) edmSpaceType != null;
    }

    internal virtual DbQueryCommandTree GetCqtView(EntitySetBase extent)
    {
      return this.GetGeneratedView(extent).GetCommandTree();
    }

    internal virtual GeneratedView GetGeneratedView(EntitySetBase extent)
    {
      return ((StorageMappingItemCollection) this.GetItemCollection(DataSpace.CSSpace, true)).GetGeneratedView(extent, this);
    }

    internal virtual bool TryGetGeneratedViewOfType(
      EntitySetBase extent,
      EntityTypeBase type,
      bool includeSubtypes,
      out GeneratedView generatedView)
    {
      return ((StorageMappingItemCollection) this.GetItemCollection(DataSpace.CSSpace, true)).TryGetGeneratedViewOfType(extent, type, includeSubtypes, out generatedView);
    }

    internal virtual DbLambda GetGeneratedFunctionDefinition(EdmFunction function)
    {
      return ((EdmItemCollection) this.GetItemCollection(DataSpace.CSpace, true)).GetGeneratedFunctionDefinition(function);
    }

    internal virtual bool TryGetFunctionImportMapping(
      EdmFunction functionImport,
      out FunctionImportMapping targetFunctionMapping)
    {
      foreach (EntityContainerMapping containerMapping in this.GetItems<EntityContainerMapping>(DataSpace.CSSpace))
      {
        if (containerMapping.TryGetFunctionImportMapping(functionImport, out targetFunctionMapping))
          return true;
      }
      targetFunctionMapping = (FunctionImportMapping) null;
      return false;
    }

    internal virtual ViewLoader GetUpdateViewLoader()
    {
      if (this._itemsCSSpace == null || this._itemsCSSpace.Value == null)
        return (ViewLoader) null;
      return this._itemsCSSpace.Value.GetUpdateViewLoader();
    }

    internal virtual TypeUsage GetOSpaceTypeUsage(TypeUsage edmSpaceTypeUsage)
    {
      return TypeUsage.Create(!Helper.IsPrimitiveType(edmSpaceTypeUsage.EdmType) ? ((ObjectTypeMapping) ((MappingItemCollection) this.GetItemCollection(DataSpace.OCSpace, true)).GetMap((GlobalItem) edmSpaceTypeUsage.EdmType)).ClrType : (EdmType) this.GetItemCollection(DataSpace.OSpace, true).GetMappedPrimitiveType(((PrimitiveType) edmSpaceTypeUsage.EdmType).PrimitiveTypeKind), (IEnumerable<Facet>) edmSpaceTypeUsage.Facets);
    }

    internal virtual bool IsItemCollectionAlreadyRegistered(DataSpace dataSpace)
    {
      ItemCollection collection;
      return this.TryGetItemCollection(dataSpace, out collection);
    }

    internal virtual bool IsMetadataWorkspaceCSCompatible(MetadataWorkspace other)
    {
      return this.GetItemCollection(DataSpace.CSSpace, false).MetadataEquals(other.GetItemCollection(DataSpace.CSSpace, false));
    }

    /// <summary>Clears all the metadata cache entries.</summary>
    public static void ClearCache()
    {
      MetadataCache.Instance.Clear();
      using (LockedAssemblyCache lockedAssemblyCache = AssemblyCache.AquireLockedAssemblyCache())
        lockedAssemblyCache.Clear();
    }

    internal static TypeUsage GetCanonicalModelTypeUsage(
      PrimitiveTypeKind primitiveTypeKind)
    {
      return EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(primitiveTypeKind);
    }

    internal static PrimitiveType GetModelPrimitiveType(
      PrimitiveTypeKind primitiveTypeKind)
    {
      return EdmProviderManifest.Instance.GetPrimitiveType(primitiveTypeKind);
    }

    /// <summary>Gets original value members from an entity set and entity type.</summary>
    /// <returns>The original value members from an entity set and entity type.</returns>
    /// <param name="entitySet">The entity set from which to retrieve original values.</param>
    /// <param name="entityType">The entity type of which to retrieve original values.</param>
    [Obsolete("Use MetadataWorkspace.GetRelevantMembersForUpdate(EntitySetBase, EntityTypeBase, bool) instead")]
    public virtual IEnumerable<EdmMember> GetRequiredOriginalValueMembers(
      EntitySetBase entitySet,
      EntityTypeBase entityType)
    {
      return (IEnumerable<EdmMember>) this.GetInterestingMembers(entitySet, entityType, StorageMappingItemCollection.InterestingMembersKind.RequiredOriginalValueMembers);
    }

    /// <summary>
    /// Returns members of a given <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />/
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" />
    /// for which original values are needed when modifying an entity.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Metadata.Edm.EdmMember" />s for which original value is required.
    /// </returns>
    /// <param name="entitySet">
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> belonging to the C-Space.
    /// </param>
    /// <param name="entityType">
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntityType" /> that participates in the given
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />
    /// .
    /// </param>
    /// <param name="partialUpdateSupported">true if entities may be updated partially; otherwise, false.</param>
    public virtual ReadOnlyCollection<EdmMember> GetRelevantMembersForUpdate(
      EntitySetBase entitySet,
      EntityTypeBase entityType,
      bool partialUpdateSupported)
    {
      return this.GetInterestingMembers(entitySet, entityType, partialUpdateSupported ? StorageMappingItemCollection.InterestingMembersKind.PartialUpdate : StorageMappingItemCollection.InterestingMembersKind.FullUpdate);
    }

    private ReadOnlyCollection<EdmMember> GetInterestingMembers(
      EntitySetBase entitySet,
      EntityTypeBase entityType,
      StorageMappingItemCollection.InterestingMembersKind interestingMembersKind)
    {
      AssociationSet associationSet = entitySet as AssociationSet;
      if (entitySet.EntityContainer.DataSpace != DataSpace.CSpace)
      {
        if (associationSet != null)
          throw new ArgumentException(Strings.EntitySetNotInCSPace((object) entitySet.Name));
        throw new ArgumentException(Strings.EntitySetNotInCSPace((object) entitySet.Name));
      }
      if (entitySet.ElementType.IsAssignableFrom((EdmType) entityType))
        return ((StorageMappingItemCollection) this.GetItemCollection(DataSpace.CSSpace, true)).GetInterestingMembers(entitySet, entityType, interestingMembersKind);
      if (associationSet != null)
        throw new ArgumentException(Strings.TypeNotInAssociationSet((object) entityType.FullName, (object) entitySet.ElementType.FullName, (object) entitySet.Name));
      throw new ArgumentException(Strings.TypeNotInEntitySet((object) entityType.FullName, (object) entitySet.ElementType.FullName, (object) entitySet.Name));
    }

    internal virtual QueryCacheManager GetQueryCacheManager()
    {
      return this._itemsSSpace.Value.QueryCacheManager;
    }

    internal bool TryDetermineCSpaceModelType<T>(out EdmType modelEdmType)
    {
      return this.TryDetermineCSpaceModelType(typeof (T), out modelEdmType);
    }

    internal virtual bool TryDetermineCSpaceModelType(Type type, out EdmType modelEdmType)
    {
      Type nonNullableType = TypeSystem.GetNonNullableType(type);
      this.ImplicitLoadAssemblyForType(nonNullableType, Assembly.GetCallingAssembly());
      EdmType edmType;
      MappingBase map;
      if (this.GetItemCollection(DataSpace.OSpace).TryGetItem<EdmType>(nonNullableType.FullNameWithNesting(), out edmType) && this.TryGetMap((GlobalItem) edmType, DataSpace.OCSpace, out map))
      {
        ObjectTypeMapping objectTypeMapping = (ObjectTypeMapping) map;
        modelEdmType = objectTypeMapping.EdmType;
        return true;
      }
      modelEdmType = (EdmType) null;
      return false;
    }
  }
}
