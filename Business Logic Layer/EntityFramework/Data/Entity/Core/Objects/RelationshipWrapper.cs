// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.RelationshipWrapper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Objects
{
  internal sealed class RelationshipWrapper : IEquatable<RelationshipWrapper>
  {
    internal readonly AssociationSet AssociationSet;
    internal readonly EntityKey Key0;
    internal readonly EntityKey Key1;

    internal RelationshipWrapper(AssociationSet extent, EntityKey key)
    {
      this.AssociationSet = extent;
      this.Key0 = key;
      this.Key1 = key;
    }

    internal RelationshipWrapper(RelationshipWrapper wrapper, int ordinal, EntityKey key)
    {
      this.AssociationSet = wrapper.AssociationSet;
      this.Key0 = ordinal == 0 ? key : wrapper.Key0;
      this.Key1 = ordinal == 0 ? wrapper.Key1 : key;
    }

    internal RelationshipWrapper(
      AssociationSet extent,
      KeyValuePair<string, EntityKey> roleAndKey1,
      KeyValuePair<string, EntityKey> roleAndKey2)
      : this(extent, roleAndKey1.Key, roleAndKey1.Value, roleAndKey2.Key, roleAndKey2.Value)
    {
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "role1")]
    internal RelationshipWrapper(
      AssociationSet extent,
      string role0,
      EntityKey key0,
      string role1,
      EntityKey key1)
    {
      this.AssociationSet = extent;
      if (extent.ElementType.AssociationEndMembers[0].Name == role0)
      {
        this.Key0 = key0;
        this.Key1 = key1;
      }
      else
      {
        this.Key0 = key1;
        this.Key1 = key0;
      }
    }

    internal ReadOnlyMetadataCollection<AssociationEndMember> AssociationEndMembers
    {
      get
      {
        return this.AssociationSet.ElementType.AssociationEndMembers;
      }
    }

    internal AssociationEndMember GetAssociationEndMember(EntityKey key)
    {
      return this.AssociationEndMembers[this.Key0 != key ? 1 : 0];
    }

    internal EntityKey GetOtherEntityKey(EntityKey key)
    {
      if (this.Key0 == key)
        return this.Key1;
      if (!(this.Key1 == key))
        return (EntityKey) null;
      return this.Key0;
    }

    internal EntityKey GetEntityKey(int ordinal)
    {
      switch (ordinal)
      {
        case 0:
          return this.Key0;
        case 1:
          return this.Key1;
        default:
          throw new ArgumentOutOfRangeException(nameof (ordinal));
      }
    }

    public override int GetHashCode()
    {
      return this.AssociationSet.Name.GetHashCode() ^ this.Key0.GetHashCode() + this.Key1.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as RelationshipWrapper);
    }

    public bool Equals(RelationshipWrapper wrapper)
    {
      if (object.ReferenceEquals((object) this, (object) wrapper))
        return true;
      if (wrapper != null && object.ReferenceEquals((object) this.AssociationSet, (object) wrapper.AssociationSet) && this.Key0.Equals(wrapper.Key0))
        return this.Key1.Equals(wrapper.Key1);
      return false;
    }
  }
}
