// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportResultMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>Represents a result mapping for a function import.</summary>
  public sealed class FunctionImportResultMapping : MappingItem
  {
    private readonly List<FunctionImportStructuralTypeMapping> _typeMappings = new List<FunctionImportStructuralTypeMapping>();

    /// <summary>Gets the type mappings.</summary>
    public ReadOnlyCollection<FunctionImportStructuralTypeMapping> TypeMappings
    {
      get
      {
        return new ReadOnlyCollection<FunctionImportStructuralTypeMapping>((IList<FunctionImportStructuralTypeMapping>) this._typeMappings);
      }
    }

    /// <summary>Adds a type mapping.</summary>
    /// <param name="typeMapping">The type mapping to add.</param>
    public void AddTypeMapping(FunctionImportStructuralTypeMapping typeMapping)
    {
      Check.NotNull<FunctionImportStructuralTypeMapping>(typeMapping, nameof (typeMapping));
      this.ThrowIfReadOnly();
      this._typeMappings.Add(typeMapping);
    }

    /// <summary>Removes a type mapping.</summary>
    /// <param name="typeMapping">The type mapping to remove.</param>
    public void RemoveTypeMapping(FunctionImportStructuralTypeMapping typeMapping)
    {
      Check.NotNull<FunctionImportStructuralTypeMapping>(typeMapping, nameof (typeMapping));
      this.ThrowIfReadOnly();
      this._typeMappings.Remove(typeMapping);
    }

    internal override void SetReadOnly()
    {
      this._typeMappings.TrimExcess();
      MappingItem.SetReadOnly((IEnumerable<MappingItem>) this._typeMappings);
      base.SetReadOnly();
    }

    internal List<FunctionImportStructuralTypeMapping> SourceList
    {
      get
      {
        return this._typeMappings;
      }
    }
  }
}
