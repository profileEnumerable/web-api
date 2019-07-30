// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.EntityTypeModificationFunctionMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Describes modification function mappings for an entity type within an entity set.
  /// </summary>
  public sealed class EntityTypeModificationFunctionMapping : MappingItem
  {
    private readonly EntityType _entityType;
    private readonly ModificationFunctionMapping _deleteFunctionMapping;
    private readonly ModificationFunctionMapping _insertFunctionMapping;
    private readonly ModificationFunctionMapping _updateFunctionMapping;

    /// <summary>
    /// Initializes a new EntityTypeModificationFunctionMapping instance.
    /// </summary>
    /// <param name="entityType">An entity type.</param>
    /// <param name="deleteFunctionMapping">A delete function mapping.</param>
    /// <param name="insertFunctionMapping">An insert function mapping.</param>
    /// <param name="updateFunctionMapping">An updated function mapping.</param>
    public EntityTypeModificationFunctionMapping(
      EntityType entityType,
      ModificationFunctionMapping deleteFunctionMapping,
      ModificationFunctionMapping insertFunctionMapping,
      ModificationFunctionMapping updateFunctionMapping)
    {
      Check.NotNull<EntityType>(entityType, nameof (entityType));
      this._entityType = entityType;
      this._deleteFunctionMapping = deleteFunctionMapping;
      this._insertFunctionMapping = insertFunctionMapping;
      this._updateFunctionMapping = updateFunctionMapping;
    }

    /// <summary>Gets the entity type.</summary>
    public EntityType EntityType
    {
      get
      {
        return this._entityType;
      }
    }

    /// <summary>Gets the delete function mapping.</summary>
    public ModificationFunctionMapping DeleteFunctionMapping
    {
      get
      {
        return this._deleteFunctionMapping;
      }
    }

    /// <summary>Gets the insert function mapping.</summary>
    public ModificationFunctionMapping InsertFunctionMapping
    {
      get
      {
        return this._insertFunctionMapping;
      }
    }

    /// <summary>Gets hte update function mapping.</summary>
    public ModificationFunctionMapping UpdateFunctionMapping
    {
      get
      {
        return this._updateFunctionMapping;
      }
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ET{{{0}}}:{4}DFunc={{{1}}},{4}IFunc={{{2}}},{4}UFunc={{{3}}}", (object) this.EntityType, (object) this.DeleteFunctionMapping, (object) this.InsertFunctionMapping, (object) this.UpdateFunctionMapping, (object) (Environment.NewLine + "  "));
    }

    internal override void SetReadOnly()
    {
      MappingItem.SetReadOnly((MappingItem) this._deleteFunctionMapping);
      MappingItem.SetReadOnly((MappingItem) this._insertFunctionMapping);
      MappingItem.SetReadOnly((MappingItem) this._updateFunctionMapping);
      base.SetReadOnly();
    }

    internal IEnumerable<ModificationFunctionParameterBinding> PrimaryParameterBindings
    {
      get
      {
        IEnumerable<ModificationFunctionParameterBinding> first = Enumerable.Empty<ModificationFunctionParameterBinding>();
        if (this.DeleteFunctionMapping != null)
          first = first.Concat<ModificationFunctionParameterBinding>((IEnumerable<ModificationFunctionParameterBinding>) this.DeleteFunctionMapping.ParameterBindings);
        if (this.InsertFunctionMapping != null)
          first = first.Concat<ModificationFunctionParameterBinding>((IEnumerable<ModificationFunctionParameterBinding>) this.InsertFunctionMapping.ParameterBindings);
        if (this.UpdateFunctionMapping != null)
          first = first.Concat<ModificationFunctionParameterBinding>(this.UpdateFunctionMapping.ParameterBindings.Where<ModificationFunctionParameterBinding>((Func<ModificationFunctionParameterBinding, bool>) (pb => pb.IsCurrent)));
        return first;
      }
    }
  }
}
