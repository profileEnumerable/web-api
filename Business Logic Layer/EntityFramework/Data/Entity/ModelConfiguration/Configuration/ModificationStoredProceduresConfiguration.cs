// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ModificationStoredProceduresConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Mapping;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  internal class ModificationStoredProceduresConfiguration
  {
    private ModificationStoredProcedureConfiguration _insertModificationStoredProcedureConfiguration;
    private ModificationStoredProcedureConfiguration _updateModificationStoredProcedureConfiguration;
    private ModificationStoredProcedureConfiguration _deleteModificationStoredProcedureConfiguration;

    public ModificationStoredProceduresConfiguration()
    {
    }

    private ModificationStoredProceduresConfiguration(
      ModificationStoredProceduresConfiguration source)
    {
      if (source._insertModificationStoredProcedureConfiguration != null)
        this._insertModificationStoredProcedureConfiguration = source._insertModificationStoredProcedureConfiguration.Clone();
      if (source._updateModificationStoredProcedureConfiguration != null)
        this._updateModificationStoredProcedureConfiguration = source._updateModificationStoredProcedureConfiguration.Clone();
      if (source._deleteModificationStoredProcedureConfiguration == null)
        return;
      this._deleteModificationStoredProcedureConfiguration = source._deleteModificationStoredProcedureConfiguration.Clone();
    }

    public virtual ModificationStoredProceduresConfiguration Clone()
    {
      return new ModificationStoredProceduresConfiguration(this);
    }

    public virtual void Insert(
      ModificationStoredProcedureConfiguration modificationStoredProcedureConfiguration)
    {
      this._insertModificationStoredProcedureConfiguration = modificationStoredProcedureConfiguration;
    }

    public virtual void Update(
      ModificationStoredProcedureConfiguration modificationStoredProcedureConfiguration)
    {
      this._updateModificationStoredProcedureConfiguration = modificationStoredProcedureConfiguration;
    }

    public virtual void Delete(
      ModificationStoredProcedureConfiguration modificationStoredProcedureConfiguration)
    {
      this._deleteModificationStoredProcedureConfiguration = modificationStoredProcedureConfiguration;
    }

    public ModificationStoredProcedureConfiguration InsertModificationStoredProcedureConfiguration
    {
      get
      {
        return this._insertModificationStoredProcedureConfiguration;
      }
    }

    public ModificationStoredProcedureConfiguration UpdateModificationStoredProcedureConfiguration
    {
      get
      {
        return this._updateModificationStoredProcedureConfiguration;
      }
    }

    public ModificationStoredProcedureConfiguration DeleteModificationStoredProcedureConfiguration
    {
      get
      {
        return this._deleteModificationStoredProcedureConfiguration;
      }
    }

    public virtual void Configure(
      EntityTypeModificationFunctionMapping modificationStoredProcedureMapping,
      DbProviderManifest providerManifest)
    {
      if (this._insertModificationStoredProcedureConfiguration != null)
        this._insertModificationStoredProcedureConfiguration.Configure(modificationStoredProcedureMapping.InsertFunctionMapping, providerManifest);
      if (this._updateModificationStoredProcedureConfiguration != null)
        this._updateModificationStoredProcedureConfiguration.Configure(modificationStoredProcedureMapping.UpdateFunctionMapping, providerManifest);
      if (this._deleteModificationStoredProcedureConfiguration == null)
        return;
      this._deleteModificationStoredProcedureConfiguration.Configure(modificationStoredProcedureMapping.DeleteFunctionMapping, providerManifest);
    }

    public void Configure(
      AssociationSetModificationFunctionMapping modificationStoredProcedureMapping,
      DbProviderManifest providerManifest)
    {
      if (this._insertModificationStoredProcedureConfiguration != null)
        this._insertModificationStoredProcedureConfiguration.Configure(modificationStoredProcedureMapping.InsertFunctionMapping, providerManifest);
      if (this._deleteModificationStoredProcedureConfiguration == null)
        return;
      this._deleteModificationStoredProcedureConfiguration.Configure(modificationStoredProcedureMapping.DeleteFunctionMapping, providerManifest);
    }

    public bool IsCompatibleWith(ModificationStoredProceduresConfiguration other)
    {
      return (this._insertModificationStoredProcedureConfiguration == null || other._insertModificationStoredProcedureConfiguration == null || this._insertModificationStoredProcedureConfiguration.IsCompatibleWith(other._insertModificationStoredProcedureConfiguration)) && (this._deleteModificationStoredProcedureConfiguration == null || other._deleteModificationStoredProcedureConfiguration == null || this._deleteModificationStoredProcedureConfiguration.IsCompatibleWith(other._deleteModificationStoredProcedureConfiguration));
    }

    public void Merge(
      ModificationStoredProceduresConfiguration modificationStoredProceduresConfiguration,
      bool allowOverride)
    {
      if (this._insertModificationStoredProcedureConfiguration == null)
        this._insertModificationStoredProcedureConfiguration = modificationStoredProceduresConfiguration.InsertModificationStoredProcedureConfiguration;
      else if (modificationStoredProceduresConfiguration.InsertModificationStoredProcedureConfiguration != null)
        this._insertModificationStoredProcedureConfiguration.Merge(modificationStoredProceduresConfiguration.InsertModificationStoredProcedureConfiguration, allowOverride);
      if (this._updateModificationStoredProcedureConfiguration == null)
        this._updateModificationStoredProcedureConfiguration = modificationStoredProceduresConfiguration.UpdateModificationStoredProcedureConfiguration;
      else if (modificationStoredProceduresConfiguration.UpdateModificationStoredProcedureConfiguration != null)
        this._updateModificationStoredProcedureConfiguration.Merge(modificationStoredProceduresConfiguration.UpdateModificationStoredProcedureConfiguration, allowOverride);
      if (this._deleteModificationStoredProcedureConfiguration == null)
      {
        this._deleteModificationStoredProcedureConfiguration = modificationStoredProceduresConfiguration.DeleteModificationStoredProcedureConfiguration;
      }
      else
      {
        if (modificationStoredProceduresConfiguration.DeleteModificationStoredProcedureConfiguration == null)
          return;
        this._deleteModificationStoredProcedureConfiguration.Merge(modificationStoredProceduresConfiguration.DeleteModificationStoredProcedureConfiguration, allowOverride);
      }
    }
  }
}
