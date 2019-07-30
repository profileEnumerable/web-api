// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.RelationshipType
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Represents the Relationship type</summary>
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
  public abstract class RelationshipType : EntityTypeBase
  {
    private ReadOnlyMetadataCollection<RelationshipEndMember> _relationshipEndMembers;

    internal RelationshipType(string name, string namespaceName, DataSpace dataSpace)
      : base(name, namespaceName, dataSpace)
    {
    }

    /// <summary>Gets the list of ends for this relationship type. </summary>
    /// <returns>
    /// A collection of type <see cref="T:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1" /> that contains the list of Ends for this relationship type.
    /// </returns>
    public ReadOnlyMetadataCollection<RelationshipEndMember> RelationshipEndMembers
    {
      get
      {
        if (this._relationshipEndMembers == null)
          Interlocked.CompareExchange<ReadOnlyMetadataCollection<RelationshipEndMember>>(ref this._relationshipEndMembers, (ReadOnlyMetadataCollection<RelationshipEndMember>) new FilteredReadOnlyMetadataCollection<RelationshipEndMember, EdmMember>(this.Members, new Predicate<EdmMember>(Helper.IsRelationshipEndMember)), (ReadOnlyMetadataCollection<RelationshipEndMember>) null);
        return this._relationshipEndMembers;
      }
    }
  }
}
