// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.FunctionImportEntityTypeMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>Represents a function import entity type mapping.</summary>
  public sealed class FunctionImportEntityTypeMapping : FunctionImportStructuralTypeMapping
  {
    private readonly ReadOnlyCollection<EntityType> _entityTypes;
    private readonly ReadOnlyCollection<EntityType> _isOfTypeEntityTypes;
    private readonly ReadOnlyCollection<FunctionImportEntityTypeMappingCondition> _conditions;

    /// <summary>
    /// Initializes a new FunctionImportEntityTypeMapping instance.
    /// </summary>
    /// <param name="isOfTypeEntityTypes">The entity types at the base of
    /// the type hierarchies to be mapped.</param>
    /// <param name="entityTypes">The entity types to be mapped.</param>
    /// <param name="properties">The property mappings for the result types of a function import.</param>
    /// <param name="conditions">The mapping conditions.</param>
    public FunctionImportEntityTypeMapping(
      IEnumerable<EntityType> isOfTypeEntityTypes,
      IEnumerable<EntityType> entityTypes,
      Collection<FunctionImportReturnTypePropertyMapping> properties,
      IEnumerable<FunctionImportEntityTypeMappingCondition> conditions)
      : this(Check.NotNull<IEnumerable<EntityType>>(isOfTypeEntityTypes, nameof (isOfTypeEntityTypes)), Check.NotNull<IEnumerable<EntityType>>(entityTypes, nameof (entityTypes)), Check.NotNull<IEnumerable<FunctionImportEntityTypeMappingCondition>>(conditions, nameof (conditions)), Check.NotNull<Collection<FunctionImportReturnTypePropertyMapping>>(properties, nameof (properties)), LineInfo.Empty)
    {
    }

    internal FunctionImportEntityTypeMapping(
      IEnumerable<EntityType> isOfTypeEntityTypes,
      IEnumerable<EntityType> entityTypes,
      IEnumerable<FunctionImportEntityTypeMappingCondition> conditions,
      Collection<FunctionImportReturnTypePropertyMapping> columnsRenameList,
      LineInfo lineInfo)
      : base(columnsRenameList, lineInfo)
    {
      this._isOfTypeEntityTypes = new ReadOnlyCollection<EntityType>((IList<EntityType>) isOfTypeEntityTypes.ToList<EntityType>());
      this._entityTypes = new ReadOnlyCollection<EntityType>((IList<EntityType>) entityTypes.ToList<EntityType>());
      this._conditions = new ReadOnlyCollection<FunctionImportEntityTypeMappingCondition>((IList<FunctionImportEntityTypeMappingCondition>) conditions.ToList<FunctionImportEntityTypeMappingCondition>());
    }

    /// <summary>Gets the entity types being mapped.</summary>
    public ReadOnlyCollection<EntityType> EntityTypes
    {
      get
      {
        return this._entityTypes;
      }
    }

    /// <summary>
    /// Gets the entity types at the base of the hierarchies being mapped.
    /// </summary>
    public ReadOnlyCollection<EntityType> IsOfTypeEntityTypes
    {
      get
      {
        return this._isOfTypeEntityTypes;
      }
    }

    /// <summary>Gets the mapping conditions.</summary>
    public ReadOnlyCollection<FunctionImportEntityTypeMappingCondition> Conditions
    {
      get
      {
        return this._conditions;
      }
    }

    internal override void SetReadOnly()
    {
      MappingItem.SetReadOnly((IEnumerable<MappingItem>) this._conditions);
      base.SetReadOnly();
    }

    internal IEnumerable<EntityType> GetMappedEntityTypes(
      ItemCollection itemCollection)
    {
      return this.EntityTypes.Concat<EntityType>(this.IsOfTypeEntityTypes.SelectMany<EntityType, EntityType>((Func<EntityType, IEnumerable<EntityType>>) (entityType => MetadataHelper.GetTypeAndSubtypesOf((EdmType) entityType, itemCollection, false).Cast<EntityType>())));
    }

    internal IEnumerable<string> GetDiscriminatorColumns()
    {
      return this.Conditions.Select<FunctionImportEntityTypeMappingCondition, string>((Func<FunctionImportEntityTypeMappingCondition, string>) (condition => condition.ColumnName));
    }
  }
}
