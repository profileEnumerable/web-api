// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.RelationshipSet
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Metadata.Edm
{
  /// <summary>Class for representing a relationship set</summary>
  public abstract class RelationshipSet : EntitySetBase
  {
    internal RelationshipSet(
      string name,
      string schema,
      string table,
      string definingQuery,
      RelationshipType relationshipType)
      : base(name, schema, table, definingQuery, (EntityTypeBase) relationshipType)
    {
    }

    /// <summary>
    /// Gets the relationship type of this <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipSet" />.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipType" /> object that represents the relationship type of this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipSet" />
    /// .
    /// </returns>
    public RelationshipType ElementType
    {
      get
      {
        return (RelationshipType) base.ElementType;
      }
    }

    /// <summary>
    /// Gets the built-in type kind for this <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipSet" />.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Metadata.Edm.BuiltInTypeKind" /> object that represents the built-in type kind for this
    /// <see cref="T:System.Data.Entity.Core.Metadata.Edm.RelationshipSet" />
    /// .
    /// </returns>
    public override BuiltInTypeKind BuiltInTypeKind
    {
      get
      {
        return BuiltInTypeKind.RelationshipSet;
      }
    }
  }
}
