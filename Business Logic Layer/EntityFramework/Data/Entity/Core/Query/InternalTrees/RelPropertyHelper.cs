// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.RelPropertyHelper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal sealed class RelPropertyHelper
  {
    private readonly Dictionary<EntityTypeBase, List<RelProperty>> _relPropertyMap;
    private readonly HashSet<RelProperty> _interestingRelProperties;

    private void AddRelProperty(
      AssociationType associationType,
      AssociationEndMember fromEnd,
      AssociationEndMember toEnd)
    {
      if (toEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many)
        return;
      RelProperty relProperty = new RelProperty((RelationshipType) associationType, (RelationshipEndMember) fromEnd, (RelationshipEndMember) toEnd);
      if (this._interestingRelProperties == null || !this._interestingRelProperties.Contains(relProperty))
        return;
      EntityTypeBase elementType = ((RefType) fromEnd.TypeUsage.EdmType).ElementType;
      List<RelProperty> relPropertyList;
      if (!this._relPropertyMap.TryGetValue(elementType, out relPropertyList))
      {
        relPropertyList = new List<RelProperty>();
        this._relPropertyMap[elementType] = relPropertyList;
      }
      relPropertyList.Add(relProperty);
    }

    private void ProcessRelationship(RelationshipType relationshipType)
    {
      AssociationType associationType = relationshipType as AssociationType;
      if (associationType == null || associationType.AssociationEndMembers.Count != 2)
        return;
      AssociationEndMember associationEndMember1 = associationType.AssociationEndMembers[0];
      AssociationEndMember associationEndMember2 = associationType.AssociationEndMembers[1];
      this.AddRelProperty(associationType, associationEndMember1, associationEndMember2);
      this.AddRelProperty(associationType, associationEndMember2, associationEndMember1);
    }

    internal RelPropertyHelper(MetadataWorkspace ws, HashSet<RelProperty> interestingRelProperties)
    {
      this._relPropertyMap = new Dictionary<EntityTypeBase, List<RelProperty>>();
      this._interestingRelProperties = interestingRelProperties;
      foreach (RelationshipType relationshipType in ws.GetItems<RelationshipType>(DataSpace.CSpace))
        this.ProcessRelationship(relationshipType);
    }

    internal IEnumerable<RelProperty> GetDeclaredOnlyRelProperties(
      EntityTypeBase entityType)
    {
      List<RelProperty> relProperties;
      if (this._relPropertyMap.TryGetValue(entityType, out relProperties))
      {
        foreach (RelProperty relProperty in relProperties)
          yield return relProperty;
      }
    }

    internal IEnumerable<RelProperty> GetRelProperties(
      EntityTypeBase entityType)
    {
      if (entityType.BaseType != null)
      {
        foreach (RelProperty relProperty in this.GetRelProperties(entityType.BaseType as EntityTypeBase))
          yield return relProperty;
      }
      foreach (RelProperty declaredOnlyRelProperty in this.GetDeclaredOnlyRelProperties(entityType))
        yield return declaredOnlyRelProperty;
    }
  }
}
