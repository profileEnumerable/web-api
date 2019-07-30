// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.AssociationSetModificationFunctionMapping
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Globalization;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Describes modification function mappings for an association set.
  /// </summary>
  public sealed class AssociationSetModificationFunctionMapping : MappingItem
  {
    private readonly AssociationSet _associationSet;
    private readonly ModificationFunctionMapping _deleteFunctionMapping;
    private readonly ModificationFunctionMapping _insertFunctionMapping;

    /// <summary>
    /// Initalizes a new AssociationSetModificationFunctionMapping instance.
    /// </summary>
    /// <param name="associationSet">An association set.</param>
    /// <param name="deleteFunctionMapping">A delete function mapping.</param>
    /// <param name="insertFunctionMapping">An insert function mapping.</param>
    public AssociationSetModificationFunctionMapping(
      AssociationSet associationSet,
      ModificationFunctionMapping deleteFunctionMapping,
      ModificationFunctionMapping insertFunctionMapping)
    {
      Check.NotNull<AssociationSet>(associationSet, nameof (associationSet));
      this._associationSet = associationSet;
      this._deleteFunctionMapping = deleteFunctionMapping;
      this._insertFunctionMapping = insertFunctionMapping;
    }

    /// <summary>Gets the association set.</summary>
    public AssociationSet AssociationSet
    {
      get
      {
        return this._associationSet;
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

    /// <inheritdoc />
    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AS{{{0}}}:{3}DFunc={{{1}}},{3}IFunc={{{2}}}", (object) this.AssociationSet, (object) this.DeleteFunctionMapping, (object) this.InsertFunctionMapping, (object) (Environment.NewLine + "  "));
    }

    internal override void SetReadOnly()
    {
      MappingItem.SetReadOnly((MappingItem) this._deleteFunctionMapping);
      MappingItem.SetReadOnly((MappingItem) this._insertFunctionMapping);
      base.SetReadOnly();
    }
  }
}
