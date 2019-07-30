// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ObjectItemLoadingSessionData
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class ObjectItemLoadingSessionData
  {
    private readonly Dictionary<Assembly, MutableAssemblyCacheEntry> _listOfAssembliesLoaded = new Dictionary<Assembly, MutableAssemblyCacheEntry>();
    private readonly HashSet<ObjectItemAssemblyLoader> _loadersThatNeedLevel1PostSessionProcessing = new HashSet<ObjectItemAssemblyLoader>();
    private readonly HashSet<ObjectItemAssemblyLoader> _loadersThatNeedLevel2PostSessionProcessing = new HashSet<ObjectItemAssemblyLoader>();
    private Func<Assembly, ObjectItemLoadingSessionData, ObjectItemAssemblyLoader> _loaderFactory;
    private readonly Dictionary<string, EdmType> _typesInLoading;
    private readonly LoadMessageLogger _loadMessageLogger;
    private readonly List<EdmItemError> _errors;
    private readonly KnownAssembliesSet _knownAssemblies;
    private readonly LockedAssemblyCache _lockedAssemblyCache;
    private readonly EdmItemCollection _edmItemCollection;
    private Dictionary<string, KeyValuePair<EdmType, int>> _conventionCSpaceTypeNames;
    private readonly Dictionary<EdmType, EdmType> _cspaceToOspace;
    private readonly object _originalLoaderCookie;

    internal virtual Dictionary<string, EdmType> TypesInLoading
    {
      get
      {
        return this._typesInLoading;
      }
    }

    internal Dictionary<Assembly, MutableAssemblyCacheEntry> AssembliesLoaded
    {
      get
      {
        return this._listOfAssembliesLoaded;
      }
    }

    internal virtual List<EdmItemError> EdmItemErrors
    {
      get
      {
        return this._errors;
      }
    }

    internal KnownAssembliesSet KnownAssemblies
    {
      get
      {
        return this._knownAssemblies;
      }
    }

    internal LockedAssemblyCache LockedAssemblyCache
    {
      get
      {
        return this._lockedAssemblyCache;
      }
    }

    internal EdmItemCollection EdmItemCollection
    {
      get
      {
        return this._edmItemCollection;
      }
    }

    internal virtual Dictionary<EdmType, EdmType> CspaceToOspace
    {
      get
      {
        return this._cspaceToOspace;
      }
    }

    internal bool ConventionBasedRelationshipsAreLoaded { get; set; }

    internal virtual LoadMessageLogger LoadMessageLogger
    {
      get
      {
        return this._loadMessageLogger;
      }
    }

    internal Dictionary<string, KeyValuePair<EdmType, int>> ConventionCSpaceTypeNames
    {
      get
      {
        if (this._edmItemCollection != null && this._conventionCSpaceTypeNames == null)
        {
          this._conventionCSpaceTypeNames = new Dictionary<string, KeyValuePair<EdmType, int>>();
          foreach (EdmType edmType in this._edmItemCollection.GetItems<EdmType>())
          {
            if (edmType is StructuralType && edmType.BuiltInTypeKind != BuiltInTypeKind.AssociationType || Helper.IsEnumType(edmType))
            {
              KeyValuePair<EdmType, int> keyValuePair;
              if (this._conventionCSpaceTypeNames.TryGetValue(edmType.Name, out keyValuePair))
              {
                this._conventionCSpaceTypeNames[edmType.Name] = new KeyValuePair<EdmType, int>(keyValuePair.Key, keyValuePair.Value + 1);
              }
              else
              {
                keyValuePair = new KeyValuePair<EdmType, int>(edmType, 1);
                this._conventionCSpaceTypeNames.Add(edmType.Name, keyValuePair);
              }
            }
          }
        }
        return this._conventionCSpaceTypeNames;
      }
    }

    internal Func<Assembly, ObjectItemLoadingSessionData, ObjectItemAssemblyLoader> ObjectItemAssemblyLoaderFactory
    {
      get
      {
        return this._loaderFactory;
      }
      set
      {
        if (!(this._loaderFactory != value))
          return;
        this._loaderFactory = value;
      }
    }

    internal object LoaderCookie
    {
      get
      {
        if (this._originalLoaderCookie != null)
          return this._originalLoaderCookie;
        return (object) this._loaderFactory;
      }
    }

    internal ObjectItemLoadingSessionData()
    {
    }

    internal ObjectItemLoadingSessionData(
      KnownAssembliesSet knownAssemblies,
      LockedAssemblyCache lockedAssemblyCache,
      EdmItemCollection edmItemCollection,
      Action<string> logLoadMessage,
      object loaderCookie)
    {
      this._typesInLoading = new Dictionary<string, EdmType>((IEqualityComparer<string>) StringComparer.Ordinal);
      this._errors = new List<EdmItemError>();
      this._knownAssemblies = knownAssemblies;
      this._lockedAssemblyCache = lockedAssemblyCache;
      this._edmItemCollection = edmItemCollection;
      this._loadMessageLogger = new LoadMessageLogger(logLoadMessage);
      this._cspaceToOspace = new Dictionary<EdmType, EdmType>();
      this._loaderFactory = (Func<Assembly, ObjectItemLoadingSessionData, ObjectItemAssemblyLoader>) loaderCookie;
      this._originalLoaderCookie = loaderCookie;
      if (!(this._loaderFactory == new Func<Assembly, ObjectItemLoadingSessionData, ObjectItemAssemblyLoader>(ObjectItemConventionAssemblyLoader.Create)) || this._edmItemCollection == null)
        return;
      foreach (KnownAssemblyEntry entry in this._knownAssemblies.GetEntries((object) this._loaderFactory, edmItemCollection))
      {
        foreach (EdmType edmType in entry.CacheEntry.TypesInAssembly.OfType<EdmType>())
        {
          if (Helper.IsEntityType(edmType))
          {
            ClrEntityType clrEntityType = (ClrEntityType) edmType;
            this._cspaceToOspace.Add((EdmType) this._edmItemCollection.GetItem<StructuralType>(clrEntityType.CSpaceTypeName), (EdmType) clrEntityType);
          }
          else if (Helper.IsComplexType(edmType))
          {
            ClrComplexType clrComplexType = (ClrComplexType) edmType;
            this._cspaceToOspace.Add((EdmType) this._edmItemCollection.GetItem<StructuralType>(clrComplexType.CSpaceTypeName), (EdmType) clrComplexType);
          }
          else if (Helper.IsEnumType(edmType))
          {
            ClrEnumType clrEnumType = (ClrEnumType) edmType;
            this._cspaceToOspace.Add((EdmType) this._edmItemCollection.GetItem<EnumType>(clrEnumType.CSpaceTypeName), (EdmType) clrEnumType);
          }
          else
            this._cspaceToOspace.Add((EdmType) this._edmItemCollection.GetItem<StructuralType>(edmType.FullName), edmType);
        }
      }
    }

    internal void RegisterForLevel1PostSessionProcessing(ObjectItemAssemblyLoader loader)
    {
      this._loadersThatNeedLevel1PostSessionProcessing.Add(loader);
    }

    internal void RegisterForLevel2PostSessionProcessing(ObjectItemAssemblyLoader loader)
    {
      this._loadersThatNeedLevel2PostSessionProcessing.Add(loader);
    }

    internal void CompleteSession()
    {
      foreach (ObjectItemAssemblyLoader itemAssemblyLoader in this._loadersThatNeedLevel1PostSessionProcessing)
        itemAssemblyLoader.OnLevel1SessionProcessing();
      foreach (ObjectItemAssemblyLoader itemAssemblyLoader in this._loadersThatNeedLevel2PostSessionProcessing)
        itemAssemblyLoader.OnLevel2SessionProcessing();
    }
  }
}
