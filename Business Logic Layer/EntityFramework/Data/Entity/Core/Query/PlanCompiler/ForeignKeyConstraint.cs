// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.ForeignKeyConstraint
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class ForeignKeyConstraint
  {
    private readonly ExtentPair m_extentPair;
    private readonly List<string> m_parentKeys;
    private readonly List<string> m_childKeys;
    private readonly ReferentialConstraint m_constraint;
    private Dictionary<string, string> m_keyMap;

    internal List<string> ParentKeys
    {
      get
      {
        return this.m_parentKeys;
      }
    }

    internal List<string> ChildKeys
    {
      get
      {
        return this.m_childKeys;
      }
    }

    internal ExtentPair Pair
    {
      get
      {
        return this.m_extentPair;
      }
    }

    internal RelationshipMultiplicity ChildMultiplicity
    {
      get
      {
        return this.m_constraint.ToRole.RelationshipMultiplicity;
      }
    }

    internal bool GetParentProperty(string childPropertyName, out string parentPropertyName)
    {
      this.BuildKeyMap();
      return this.m_keyMap.TryGetValue(childPropertyName, out parentPropertyName);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    internal ForeignKeyConstraint(RelationshipSet relationshipSet, ReferentialConstraint constraint)
    {
      AssociationSet associationSet = relationshipSet as AssociationSet;
      AssociationEndMember fromRole = constraint.FromRole as AssociationEndMember;
      AssociationEndMember toRole = constraint.ToRole as AssociationEndMember;
      if (associationSet == null || fromRole == null || toRole == null)
        throw new NotSupportedException();
      this.m_constraint = constraint;
      this.m_extentPair = new ExtentPair((EntitySetBase) MetadataHelper.GetEntitySetAtEnd(associationSet, fromRole), (EntitySetBase) MetadataHelper.GetEntitySetAtEnd(associationSet, toRole));
      this.m_childKeys = new List<string>();
      foreach (EdmMember toProperty in constraint.ToProperties)
        this.m_childKeys.Add(toProperty.Name);
      this.m_parentKeys = new List<string>();
      foreach (EdmMember fromProperty in constraint.FromProperties)
        this.m_parentKeys.Add(fromProperty.Name);
      System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(fromRole.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne || RelationshipMultiplicity.One == fromRole.RelationshipMultiplicity, "from-end of relationship constraint cannot have multiplicity greater than 1");
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(System.Boolean,System.String)")]
    private void BuildKeyMap()
    {
      if (this.m_keyMap != null)
        return;
      this.m_keyMap = new Dictionary<string, string>();
      IEnumerator<EdmProperty> enumerator1 = (IEnumerator<EdmProperty>) this.m_constraint.FromProperties.GetEnumerator();
      IEnumerator<EdmProperty> enumerator2 = (IEnumerator<EdmProperty>) this.m_constraint.ToProperties.GetEnumerator();
      while (true)
      {
        bool flag1 = !enumerator1.MoveNext();
        bool flag2 = !enumerator2.MoveNext();
        System.Data.Entity.Core.Query.PlanCompiler.PlanCompiler.Assert(flag1 == flag2, "key count mismatch");
        if (!flag1)
          this.m_keyMap[enumerator2.Current.Name] = enumerator1.Current.Name;
        else
          break;
      }
    }
  }
}
