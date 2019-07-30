// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.Validation.BasicCellRelation
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Mapping.ViewGeneration.Structures;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text;

namespace System.Data.Entity.Core.Mapping.ViewGeneration.Validation
{
  internal class BasicCellRelation : CellRelation
  {
    private readonly CellQuery m_cellQuery;
    private readonly List<MemberProjectedSlot> m_slots;
    private readonly ViewCellRelation m_viewCellRelation;

    internal BasicCellRelation(
      CellQuery cellQuery,
      ViewCellRelation viewCellRelation,
      IEnumerable<MemberProjectedSlot> slots)
      : base(viewCellRelation.CellNumber)
    {
      this.m_cellQuery = cellQuery;
      this.m_slots = new List<MemberProjectedSlot>(slots);
      this.m_viewCellRelation = viewCellRelation;
    }

    internal ViewCellRelation ViewCellRelation
    {
      get
      {
        return this.m_viewCellRelation;
      }
    }

    internal void PopulateKeyConstraints(SchemaConstraints<BasicKeyConstraint> constraints)
    {
      if (this.m_cellQuery.Extent is EntitySet)
        this.PopulateKeyConstraintsForEntitySet(constraints);
      else
        this.PopulateKeyConstraintsForRelationshipSet(constraints);
    }

    private void PopulateKeyConstraintsForEntitySet(
      SchemaConstraints<BasicKeyConstraint> constraints)
    {
      this.AddKeyConstraints((IEnumerable<ExtentKey>) ExtentKey.GetKeysForEntityType(new MemberPath(this.m_cellQuery.Extent), (EntityType) this.m_cellQuery.Extent.ElementType), constraints);
    }

    private void PopulateKeyConstraintsForRelationshipSet(
      SchemaConstraints<BasicKeyConstraint> constraints)
    {
      AssociationSet extent = this.m_cellQuery.Extent as AssociationSet;
      Set<MemberPath> set = new Set<MemberPath>(MemberPath.EqualityComparer);
      bool flag = false;
      foreach (AssociationSetEnd associationSetEnd in extent.AssociationSetEnds)
      {
        AssociationEndMember associationEndMember = associationSetEnd.CorrespondingAssociationEndMember;
        List<ExtentKey> keysForEntityType = ExtentKey.GetKeysForEntityType(new MemberPath((EntitySetBase) extent, (EdmMember) associationEndMember), associationSetEnd.EntitySet.ElementType);
        if (MetadataHelper.DoesEndFormKey(extent, associationEndMember))
        {
          this.AddKeyConstraints((IEnumerable<ExtentKey>) keysForEntityType, constraints);
          flag = true;
        }
        set.AddRange(keysForEntityType[0].KeyFields);
      }
      if (flag)
        return;
      this.AddKeyConstraints((IEnumerable<ExtentKey>) new ExtentKey[1]
      {
        new ExtentKey((IEnumerable<MemberPath>) set)
      }, constraints);
    }

    private void AddKeyConstraints(
      IEnumerable<ExtentKey> keys,
      SchemaConstraints<BasicKeyConstraint> constraints)
    {
      foreach (ExtentKey key in keys)
      {
        List<MemberProjectedSlot> slots = MemberProjectedSlot.GetSlots((IEnumerable<MemberProjectedSlot>) this.m_slots, key.KeyFields);
        if (slots != null)
        {
          BasicKeyConstraint constraint = new BasicKeyConstraint(this, (IEnumerable<MemberProjectedSlot>) slots);
          constraints.Add(constraint);
        }
      }
    }

    protected override int GetHash()
    {
      return this.m_cellQuery.GetHashCode();
    }

    internal override void ToCompactString(StringBuilder builder)
    {
      builder.Append("BasicRel: ");
      StringUtil.FormatStringBuilder(builder, "{0}", (object) this.m_slots[0]);
    }
  }
}
