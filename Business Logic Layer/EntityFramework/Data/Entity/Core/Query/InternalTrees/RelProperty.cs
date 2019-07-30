// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.RelProperty
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class RelProperty
  {
    private readonly RelationshipType m_relationshipType;
    private readonly RelationshipEndMember m_fromEnd;
    private readonly RelationshipEndMember m_toEnd;

    internal RelProperty(
      RelationshipType relationshipType,
      RelationshipEndMember fromEnd,
      RelationshipEndMember toEnd)
    {
      this.m_relationshipType = relationshipType;
      this.m_fromEnd = fromEnd;
      this.m_toEnd = toEnd;
    }

    public RelationshipType Relationship
    {
      get
      {
        return this.m_relationshipType;
      }
    }

    public RelationshipEndMember FromEnd
    {
      get
      {
        return this.m_fromEnd;
      }
    }

    public RelationshipEndMember ToEnd
    {
      get
      {
        return this.m_toEnd;
      }
    }

    public override bool Equals(object obj)
    {
      RelProperty relProperty = obj as RelProperty;
      if (relProperty != null && this.Relationship.EdmEquals((MetadataItem) relProperty.Relationship) && this.FromEnd.EdmEquals((MetadataItem) relProperty.FromEnd))
        return this.ToEnd.EdmEquals((MetadataItem) relProperty.ToEnd);
      return false;
    }

    public override int GetHashCode()
    {
      return this.ToEnd.Identity.GetHashCode();
    }

    [DebuggerNonUserCode]
    public override string ToString()
    {
      return this.m_relationshipType.ToString() + ":" + (object) this.m_fromEnd + ":" + (object) this.m_toEnd;
    }
  }
}
