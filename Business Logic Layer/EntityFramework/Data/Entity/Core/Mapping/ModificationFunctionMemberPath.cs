// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ModificationFunctionMemberPath
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Utilities;
using System.Globalization;

namespace System.Data.Entity.Core.Mapping
{
  /// <summary>
  /// Describes the location of a member within an entity or association type structure.
  /// </summary>
  public sealed class ModificationFunctionMemberPath : MappingItem
  {
    private readonly ReadOnlyCollection<EdmMember> _members;
    private readonly AssociationSetEnd _associationSetEnd;

    /// <summary>
    /// Initializes a new ModificationFunctionMemberPath instance.
    /// </summary>
    /// <param name="members">Gets the members in the path from the leaf (the member being bound)
    /// to the root of the structure.</param>
    /// <param name="associationSet">Gets the association set to which we are navigating
    /// via this member. If the value is null, this is not a navigation member path.</param>
    public ModificationFunctionMemberPath(
      IEnumerable<EdmMember> members,
      AssociationSet associationSet)
    {
      Check.NotNull<IEnumerable<EdmMember>>(members, nameof (members));
      this._members = new ReadOnlyCollection<EdmMember>((IList<EdmMember>) new List<EdmMember>(members));
      if (associationSet == null)
        return;
      this._associationSetEnd = associationSet.AssociationSetEnds[this.Members[1].Name];
    }

    /// <summary>
    /// Gets the members in the path from the leaf (the member being bound)
    /// to the Root of the structure.
    /// </summary>
    public ReadOnlyCollection<EdmMember> Members
    {
      get
      {
        return this._members;
      }
    }

    /// <summary>
    /// Gets the association set to which we are navigating via this member. If the value
    /// is null, this is not a navigation member path.
    /// </summary>
    public AssociationSetEnd AssociationSetEnd
    {
      get
      {
        return this._associationSetEnd;
      }
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", this.AssociationSetEnd == null ? (object) string.Empty : (object) ("[" + (object) this.AssociationSetEnd.ParentAssociationSet + "]"), (object) StringUtil.BuildDelimitedList<EdmMember>((IEnumerable<EdmMember>) this.Members, (StringUtil.ToStringConverter<EdmMember>) null, "."));
    }
  }
}
