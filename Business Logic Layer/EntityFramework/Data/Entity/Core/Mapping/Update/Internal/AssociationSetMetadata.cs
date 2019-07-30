// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.Update.Internal.AssociationSetMetadata
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.Update.Internal
{
  internal sealed class AssociationSetMetadata
  {
    internal readonly Set<AssociationEndMember> RequiredEnds;
    internal readonly Set<AssociationEndMember> OptionalEnds;
    internal readonly Set<AssociationEndMember> IncludedValueEnds;

    internal bool HasEnds
    {
      get
      {
        if (0 >= this.RequiredEnds.Count && 0 >= this.OptionalEnds.Count)
          return 0 < this.IncludedValueEnds.Count;
        return true;
      }
    }

    internal AssociationSetMetadata(
      Set<EntitySet> affectedTables,
      AssociationSet associationSet,
      MetadataWorkspace workspace)
    {
      bool flag = 1 < affectedTables.Count;
      ReadOnlyMetadataCollection<AssociationSetEnd> associationSetEnds = associationSet.AssociationSetEnds;
      foreach (EntitySet affectedTable in affectedTables)
      {
        foreach (EntitySet entitySet in MetadataHelper.GetInfluencingEntitySetsForTable(affectedTable, workspace))
        {
          foreach (AssociationSetEnd associationSetEnd in associationSetEnds)
          {
            if (associationSetEnd.EntitySet.EdmEquals((MetadataItem) entitySet))
            {
              if (flag)
                AssociationSetMetadata.AddEnd(ref this.RequiredEnds, associationSetEnd.CorrespondingAssociationEndMember);
              else if (this.RequiredEnds == null || !this.RequiredEnds.Contains(associationSetEnd.CorrespondingAssociationEndMember))
                AssociationSetMetadata.AddEnd(ref this.OptionalEnds, associationSetEnd.CorrespondingAssociationEndMember);
            }
          }
        }
      }
      AssociationSetMetadata.FixSet(ref this.RequiredEnds);
      AssociationSetMetadata.FixSet(ref this.OptionalEnds);
      foreach (ReferentialConstraint referentialConstraint in associationSet.ElementType.ReferentialConstraints)
      {
        AssociationEndMember fromRole = (AssociationEndMember) referentialConstraint.FromRole;
        if (!this.RequiredEnds.Contains(fromRole) && !this.OptionalEnds.Contains(fromRole))
          AssociationSetMetadata.AddEnd(ref this.IncludedValueEnds, fromRole);
      }
      AssociationSetMetadata.FixSet(ref this.IncludedValueEnds);
    }

    internal AssociationSetMetadata(IEnumerable<AssociationEndMember> requiredEnds)
    {
      if (requiredEnds.Any<AssociationEndMember>())
        this.RequiredEnds = new Set<AssociationEndMember>(requiredEnds);
      AssociationSetMetadata.FixSet(ref this.RequiredEnds);
      AssociationSetMetadata.FixSet(ref this.OptionalEnds);
      AssociationSetMetadata.FixSet(ref this.IncludedValueEnds);
    }

    private static void AddEnd(ref Set<AssociationEndMember> set, AssociationEndMember element)
    {
      if (set == null)
        set = new Set<AssociationEndMember>();
      set.Add(element);
    }

    private static void FixSet(ref Set<AssociationEndMember> set)
    {
      if (set == null)
        set = Set<AssociationEndMember>.Empty;
      else
        set.MakeReadOnly();
    }
  }
}
