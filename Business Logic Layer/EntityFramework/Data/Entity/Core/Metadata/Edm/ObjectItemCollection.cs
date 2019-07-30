// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ObjectItemCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm.Provider;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>
  /// Class for representing a collection of items for the object layer.
  /// Most of the implementation for actual maintenance of the collection is
  /// done by ItemCollection
  /// </summary>
  public class ObjectItemCollection : ItemCollection
  {
    private readonly CacheForPrimitiveTypes _primitiveTypeMaps = new CacheForPrimitiveTypes();
    private KnownAssembliesSet _knownAssemblies = new KnownAssembliesSet();
    private readonly Dictionary<string, EdmType> _ocMapping = new Dictionary<string, EdmType>();
    private readonly object _loadAssemblyLock = new object();
    private object _loaderCookie;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.Metadata.Edm.ObjectItemCollection" /> class.
    /// </summary>
    public ObjectItemCollection()
      : this((KnownAssembliesSet) null)
    {
    }

    internal ObjectItemCollection(KnownAssembliesSet knownAssembliesSet = null)
      : base(DataSpace.OSpace)
    {
      this._knownAssemblies = knownAssembliesSet ?? new KnownAssembliesSet();
      foreach (PrimitiveType storeType in ClrProviderManifest.Instance.GetStoreTypes())
      {
        this.AddInternal((GlobalItem) storeType);
        this._primitiveTypeMaps.Add(storeType);
      }
    }

    internal bool OSpaceTypesLoaded { get; set; }

    internal object LoadAssemblyLock
    {
      get
      {
        return this._loadAssemblyLock;
      }
    }

    internal void ImplicitLoadAllReferencedAssemblies(
      Assembly assembly,
      EdmItemCollection edmItemCollection)
    {
      if (MetadataAssemblyHelper.ShouldFilterAssembly(assembly))
        return;
      this.LoadAssemblyFromCache(assembly, true, edmItemCollection, (Action<string>) null);
    }

    /// <summary>Loads metadata from the given assembly.</summary>
    /// <param name="assembly">The assembly from which the metadata will be loaded.</param>
    public void LoadFromAssembly(Assembly assembly)
    {
      this.ExplicitLoadFromAssembly(assembly, (EdmItemCollection) null, (Action<string>) null);
    }

    /// <summary>Loads metadata from the given assembly.</summary>
    /// <param name="assembly">The assembly from which the metadata will be loaded.</param>
    /// <param name="edmItemCollection">The EDM metadata source for the O space metadata.</param>
    /// <param name="logLoadMessage">The delegate to which log messages are sent.</param>
    public void LoadFromAssembly(
      Assembly assembly,
      EdmItemCollection edmItemCollection,
      Action<string> logLoadMessage)
    {
      Check.NotNull<Assembly>(assembly, nameof (assembly));
      Check.NotNull<EdmItemCollection>(edmItemCollection, nameof (edmItemCollection));
      Check.NotNull<Action<string>>(logLoadMessage, nameof (logLoadMessage));
      this.ExplicitLoadFromAssembly(assembly, edmItemCollection, logLoadMessage);
    }

    /// <summary>Loads metadata from the specified assembly.</summary>
    /// <param name="assembly">The assembly from which the metadata will be loaded.</param>
    /// <param name="edmItemCollection">The EDM metadata source for the O space metadata.</param>
    public void LoadFromAssembly(Assembly assembly, EdmItemCollection edmItemCollection)
    {
      Check.NotNull<Assembly>(assembly, nameof (assembly));
      Check.NotNull<EdmItemCollection>(edmItemCollection, nameof (edmItemCollection));
      this.ExplicitLoadFromAssembly(assembly, edmItemCollection, (Action<string>) null);
    }

    internal void ExplicitLoadFromAssembly(
      Assembly assembly,
      EdmItemCollection edmItemCollection,
      Action<string> logLoadMessage)
    {
      this.LoadAssemblyFromCache(assembly, false, edmItemCollection, logLoadMessage);
    }

    internal bool ImplicitLoadAssemblyForType(Type type, EdmItemCollection edmItemCollection)
    {
      bool flag = false;
      if (!MetadataAssemblyHelper.ShouldFilterAssembly(type.Assembly()))
        flag = this.LoadAssemblyFromCache(type.Assembly(), false, edmItemCollection, (Action<string>) null);
      if (type.IsGenericType())
      {
        foreach (Type genericArgument in type.GetGenericArguments())
          flag |= this.ImplicitLoadAssemblyForType(genericArgument, edmItemCollection);
      }
      return flag;
    }

    internal AssociationType GetRelationshipType(string relationshipName)
    {
      AssociationType associationType;
      if (this.TryGetItem<AssociationType>(relationshipName, out associationType))
        return associationType;
      return (AssociationType) null;
    }

    private bool LoadAssemblyFromCache(
      Assembly assembly,
      bool loadReferencedAssemblies,
      EdmItemCollection edmItemCollection,
      Action<string> logLoadMessage)
    {
      if (this.OSpaceTypesLoaded)
        return true;
      if (edmItemCollection != null)
      {
        ReadOnlyCollection<EntityContainer> items = edmItemCollection.GetItems<EntityContainer>();
        if (items.Any<EntityContainer>() && items.All<EntityContainer>((Func<EntityContainer, bool>) (c => c.Annotations.Any<MetadataProperty>((Func<MetadataProperty, bool>) (a =>
        {
          if (a.Name == "http://schemas.microsoft.com/ado/2013/11/edm/customannotation:UseClrTypes")
            return ((string) a.Value).ToUpperInvariant() == "TRUE";
          return false;
        })))))
        {
          lock (this.LoadAssemblyLock)
          {
            if (!this.OSpaceTypesLoaded)
              new CodeFirstOSpaceLoader((CodeFirstOSpaceTypeFactory) null).LoadTypes(edmItemCollection, this);
            return true;
          }
        }
      }
      KnownAssemblyEntry entry;
      if (this._knownAssemblies.TryGetKnownAssembly(assembly, this._loaderCookie, edmItemCollection, out entry))
      {
        if (!loadReferencedAssemblies)
          return entry.CacheEntry.TypesInAssembly.Count != 0;
        if (entry.ReferencedAssembliesAreLoaded)
          return true;
      }
      lock (this.LoadAssemblyLock)
      {
        if (this._knownAssemblies.TryGetKnownAssembly(assembly, this._loaderCookie, edmItemCollection, out entry) && (!loadReferencedAssemblies || entry.ReferencedAssembliesAreLoaded))
          return true;
        KnownAssembliesSet knownAssemblies = new KnownAssembliesSet(this._knownAssemblies);
        Dictionary<string, EdmType> typesInLoading;
        List<EdmItemError> errors;
        AssemblyCache.LoadAssembly(assembly, loadReferencedAssemblies, knownAssemblies, edmItemCollection, logLoadMessage, ref this._loaderCookie, out typesInLoading, out errors);
        if (errors.Count != 0)
          throw EntityUtil.InvalidSchemaEncountered(Helper.CombineErrorMessage((IEnumerable<EdmItemError>) errors));
        if (typesInLoading.Count != 0)
          this.AddLoadedTypes(typesInLoading);
        this._knownAssemblies = knownAssemblies;
        return typesInLoading.Count != 0;
      }
    }

    internal virtual void AddLoadedTypes(Dictionary<string, EdmType> typesInLoading)
    {
      List<GlobalItem> items = new List<GlobalItem>();
      foreach (EdmType edmType in typesInLoading.Values)
      {
        items.Add((GlobalItem) edmType);
        string key = "";
        try
        {
          if (Helper.IsEntityType(edmType))
          {
            key = ((ClrEntityType) edmType).CSpaceTypeName;
            this._ocMapping.Add(key, edmType);
          }
          else if (Helper.IsComplexType(edmType))
          {
            key = ((ClrComplexType) edmType).CSpaceTypeName;
            this._ocMapping.Add(key, edmType);
          }
          else if (Helper.IsEnumType(edmType))
          {
            key = ((ClrEnumType) edmType).CSpaceTypeName;
            this._ocMapping.Add(key, edmType);
          }
        }
        catch (ArgumentException ex)
        {
          throw new MappingException(Strings.Mapping_CannotMapCLRTypeMultipleTimes((object) key), (Exception) ex);
        }
      }
      this.AddRange(items);
    }

    /// <summary>Returns a collection of primitive type objects.</summary>
    /// <returns>A collection of primitive type objects.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public IEnumerable<PrimitiveType> GetPrimitiveTypes()
    {
      return (IEnumerable<PrimitiveType>) this._primitiveTypeMaps.GetTypes();
    }

    /// <summary>
    /// Returns the CLR type that corresponds to the <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> supplied by the objectSpaceType parameter.
    /// </summary>
    /// <returns>The CLR type of the OSpace argument.</returns>
    /// <param name="objectSpaceType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> that represents the object space type.
    /// </param>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public Type GetClrType(StructuralType objectSpaceType)
    {
      return ObjectItemCollection.GetClrType((EdmType) objectSpaceType);
    }

    /// <summary>
    /// Returns a CLR type corresponding to the <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> supplied by the objectSpaceType parameter.
    /// </summary>
    /// <returns>true if there is a type that matches the search criteria; otherwise, false.</returns>
    /// <param name="objectSpaceType">
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.StructuralType" /> that represents the object space type.
    /// </param>
    /// <param name="clrType">The CLR type.</param>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public bool TryGetClrType(StructuralType objectSpaceType, out Type clrType)
    {
      return ObjectItemCollection.TryGetClrType((EdmType) objectSpaceType, out clrType);
    }

    /// <summary> The method returns the underlying CLR type for the specified OSpace type argument. If the DataSpace of the parameter is not OSpace, an ArgumentException is thrown. </summary>
    /// <returns>The CLR type of the OSpace argument.</returns>
    /// <param name="objectSpaceType">The OSpace type to look up.</param>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public Type GetClrType(EnumType objectSpaceType)
    {
      return ObjectItemCollection.GetClrType((EdmType) objectSpaceType);
    }

    /// <summary>Returns the underlying CLR type for the specified OSpace enum type argument. If the DataSpace of the parameter is not OSpace, the method returns false and sets the out parameter to null. </summary>
    /// <returns>true on success, false on failure</returns>
    /// <param name="objectSpaceType">The OSpace enum type to look up</param>
    /// <param name="clrType">The CLR enum type of the OSpace argument</param>
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    public bool TryGetClrType(EnumType objectSpaceType, out Type clrType)
    {
      return ObjectItemCollection.TryGetClrType((EdmType) objectSpaceType, out clrType);
    }

    private static Type GetClrType(EdmType objectSpaceType)
    {
      Type clrType;
      if (!ObjectItemCollection.TryGetClrType(objectSpaceType, out clrType))
        throw new ArgumentException(Strings.FailedToFindClrTypeMapping((object) objectSpaceType.Identity));
      return clrType;
    }

    private static bool TryGetClrType(EdmType objectSpaceType, out Type clrType)
    {
      if (objectSpaceType.DataSpace != DataSpace.OSpace)
        throw new ArgumentException(Strings.ArgumentMustBeOSpaceType, nameof (objectSpaceType));
      clrType = (Type) null;
      if (Helper.IsEntityType(objectSpaceType) || Helper.IsComplexType(objectSpaceType) || Helper.IsEnumType(objectSpaceType))
        clrType = objectSpaceType.ClrType;
      return clrType != (Type) null;
    }

    internal override PrimitiveType GetMappedPrimitiveType(PrimitiveTypeKind modelType)
    {
      if (Helper.IsGeometricTypeKind(modelType))
        modelType = PrimitiveTypeKind.Geometry;
      else if (Helper.IsGeographicTypeKind(modelType))
        modelType = PrimitiveTypeKind.Geography;
      PrimitiveType type = (PrimitiveType) null;
      this._primitiveTypeMaps.TryGetType(modelType, (IEnumerable<Facet>) null, out type);
      return type;
    }

    internal bool TryGetOSpaceType(EdmType cspaceType, out EdmType edmType)
    {
      if (Helper.IsEntityType(cspaceType) || Helper.IsComplexType(cspaceType) || Helper.IsEnumType(cspaceType))
        return this._ocMapping.TryGetValue(cspaceType.Identity, out edmType);
      return this.TryGetItem<EdmType>(cspaceType.Identity, out edmType);
    }

    internal static string TryGetMappingCSpaceTypeIdentity(EdmType edmType)
    {
      if (Helper.IsEntityType(edmType))
        return ((ClrEntityType) edmType).CSpaceTypeName;
      if (Helper.IsComplexType(edmType))
        return ((ClrComplexType) edmType).CSpaceTypeName;
      if (Helper.IsEnumType(edmType))
        return ((ClrEnumType) edmType).CSpaceTypeName;
      return edmType.Identity;
    }

    /// <summary>Returns all the items of the specified type from this item collection.</summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains all items of the specified type.
    /// </returns>
    /// <typeparam name="T">The type returned by the method.</typeparam>
    public override ReadOnlyCollection<T> GetItems<T>()
    {
      return this.InternalGetItems(typeof (T)) as ReadOnlyCollection<T>;
    }
  }
}
