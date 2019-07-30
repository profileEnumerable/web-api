// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.ObjectItemConventionAssemblyLoader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class ObjectItemConventionAssemblyLoader : ObjectItemAssemblyLoader
  {
    private readonly List<Action> _referenceResolutions = new List<Action>();
    private readonly ObjectItemConventionAssemblyLoader.ConventionOSpaceTypeFactory _factory;

    public virtual MutableAssemblyCacheEntry CacheEntry
    {
      get
      {
        return (MutableAssemblyCacheEntry) base.CacheEntry;
      }
    }

    internal ObjectItemConventionAssemblyLoader(
      Assembly assembly,
      ObjectItemLoadingSessionData sessionData)
      : base(assembly, (AssemblyCacheEntry) new MutableAssemblyCacheEntry(), sessionData)
    {
      this.SessionData.RegisterForLevel1PostSessionProcessing((ObjectItemAssemblyLoader) this);
      this._factory = new ObjectItemConventionAssemblyLoader.ConventionOSpaceTypeFactory(this);
    }

    protected override void LoadTypesFromAssembly()
    {
      foreach (Type accessibleType in this.SourceAssembly.GetAccessibleTypes())
      {
        EdmType cspaceType;
        if (this.TryGetCSpaceTypeMatch(accessibleType, out cspaceType))
        {
          if (accessibleType.IsValueType() && !accessibleType.IsEnum())
          {
            this.SessionData.LoadMessageLogger.LogLoadMessage(Strings.Validator_OSpace_Convention_Struct((object) cspaceType.FullName, (object) accessibleType.FullName), cspaceType);
          }
          else
          {
            EdmType type = this._factory.TryCreateType(accessibleType, cspaceType);
            if (type != null)
            {
              this.CacheEntry.TypesInAssembly.Add(type);
              if (!this.SessionData.CspaceToOspace.ContainsKey(cspaceType))
              {
                this.SessionData.CspaceToOspace.Add(cspaceType, type);
              }
              else
              {
                EdmType edmType = this.SessionData.CspaceToOspace[cspaceType];
                this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.Validator_OSpace_Convention_AmbiguousClrType((object) cspaceType.Name, (object) edmType.ClrType.FullName, (object) accessibleType.FullName)));
              }
            }
          }
        }
      }
      if (this.SessionData.TypesInLoading.Count != 0)
        return;
      this.SessionData.ObjectItemAssemblyLoaderFactory = (Func<Assembly, ObjectItemLoadingSessionData, ObjectItemAssemblyLoader>) null;
    }

    protected override void AddToAssembliesLoaded()
    {
      this.SessionData.AssembliesLoaded.Add(this.SourceAssembly, this.CacheEntry);
    }

    private bool TryGetCSpaceTypeMatch(Type type, out EdmType cspaceType)
    {
      KeyValuePair<EdmType, int> keyValuePair;
      if (this.SessionData.ConventionCSpaceTypeNames.TryGetValue(type.Name, out keyValuePair))
      {
        if (keyValuePair.Value == 1)
        {
          cspaceType = keyValuePair.Key;
          return true;
        }
        this.SessionData.EdmItemErrors.Add(new EdmItemError(Strings.Validator_OSpace_Convention_MultipleTypesWithSameName((object) type.Name)));
      }
      cspaceType = (EdmType) null;
      return false;
    }

    internal override void OnLevel1SessionProcessing()
    {
      this.CreateRelationships();
      foreach (Action referenceResolution in this._referenceResolutions)
        referenceResolution();
      base.OnLevel1SessionProcessing();
    }

    internal virtual void TrackClosure(Type type)
    {
      if (this.SourceAssembly != type.Assembly() && !this.CacheEntry.ClosureAssemblies.Contains(type.Assembly()) && (!type.IsGenericType() || !EntityUtil.IsAnICollection(type) && !(type.GetGenericTypeDefinition() == typeof (EntityReference<>)) && !(type.GetGenericTypeDefinition() == typeof (Nullable<>))))
        this.CacheEntry.ClosureAssemblies.Add(type.Assembly());
      if (!type.IsGenericType())
        return;
      foreach (Type genericArgument in type.GetGenericArguments())
        this.TrackClosure(genericArgument);
    }

    private void CreateRelationships()
    {
      if (this.SessionData.ConventionBasedRelationshipsAreLoaded)
        return;
      this.SessionData.ConventionBasedRelationshipsAreLoaded = true;
      this._factory.CreateRelationships(this.SessionData.EdmItemCollection);
    }

    internal static bool SessionContainsConventionParameters(
      ObjectItemLoadingSessionData sessionData)
    {
      return sessionData.EdmItemCollection != null;
    }

    internal static ObjectItemAssemblyLoader Create(
      Assembly assembly,
      ObjectItemLoadingSessionData sessionData)
    {
      if (!ObjectItemAttributeAssemblyLoader.IsSchemaAttributePresent(assembly))
        return (ObjectItemAssemblyLoader) new ObjectItemConventionAssemblyLoader(assembly, sessionData);
      sessionData.EdmItemErrors.Add(new EdmItemError(Strings.Validator_OSpace_Convention_AttributeAssemblyReferenced((object) assembly.FullName)));
      return (ObjectItemAssemblyLoader) new ObjectItemNoOpAssemblyLoader(assembly, sessionData);
    }

    internal class ConventionOSpaceTypeFactory : OSpaceTypeFactory
    {
      private readonly ObjectItemConventionAssemblyLoader _loader;

      public ConventionOSpaceTypeFactory(ObjectItemConventionAssemblyLoader loader)
      {
        this._loader = loader;
      }

      public override List<Action> ReferenceResolutions
      {
        get
        {
          return this._loader._referenceResolutions;
        }
      }

      public override void LogLoadMessage(string message, EdmType relatedType)
      {
        this._loader.SessionData.LoadMessageLogger.LogLoadMessage(message, relatedType);
      }

      public override void LogError(string errorMessage, EdmType relatedType)
      {
        this._loader.SessionData.EdmItemErrors.Add(new EdmItemError(this._loader.SessionData.LoadMessageLogger.CreateErrorMessageWithTypeSpecificLoadLogs(errorMessage, relatedType)));
      }

      public override void TrackClosure(Type type)
      {
        this._loader.TrackClosure(type);
      }

      public override Dictionary<EdmType, EdmType> CspaceToOspace
      {
        get
        {
          return this._loader.SessionData.CspaceToOspace;
        }
      }

      public override Dictionary<string, EdmType> LoadedTypes
      {
        get
        {
          return this._loader.SessionData.TypesInLoading;
        }
      }

      public override void AddToTypesInAssembly(EdmType type)
      {
        this._loader.CacheEntry.TypesInAssembly.Add(type);
      }
    }
  }
}
