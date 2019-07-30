// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.ConstraintManager
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal class ConstraintManager
  {
    private readonly Dictionary<EntityContainer, EntityContainer> m_entityContainerMap;
    private readonly Dictionary<ExtentPair, List<ForeignKeyConstraint>> m_parentChildRelationships;

    internal bool IsParentChildRelationship(
      EntitySetBase table1,
      EntitySetBase table2,
      out List<ForeignKeyConstraint> constraints)
    {
      this.LoadRelationships(table1.EntityContainer);
      this.LoadRelationships(table2.EntityContainer);
      return this.m_parentChildRelationships.TryGetValue(new ExtentPair(table1, table2), out constraints);
    }

    internal void LoadRelationships(EntityContainer entityContainer)
    {
      if (this.m_entityContainerMap.ContainsKey(entityContainer))
        return;
      foreach (EntitySetBase baseEntitySet in entityContainer.BaseEntitySets)
      {
        RelationshipSet relationshipSet = baseEntitySet as RelationshipSet;
        if (relationshipSet != null)
        {
          RelationshipType elementType = relationshipSet.ElementType;
          AssociationType associationType = elementType as AssociationType;
          if (associationType != null && ConstraintManager.IsBinary(elementType))
          {
            foreach (ReferentialConstraint referentialConstraint in associationType.ReferentialConstraints)
            {
              ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint(relationshipSet, referentialConstraint);
              List<ForeignKeyConstraint> foreignKeyConstraintList;
              if (!this.m_parentChildRelationships.TryGetValue(foreignKeyConstraint.Pair, out foreignKeyConstraintList))
              {
                foreignKeyConstraintList = new List<ForeignKeyConstraint>();
                this.m_parentChildRelationships[foreignKeyConstraint.Pair] = foreignKeyConstraintList;
              }
              foreignKeyConstraintList.Add(foreignKeyConstraint);
            }
          }
        }
      }
      this.m_entityContainerMap[entityContainer] = entityContainer;
    }

    internal ConstraintManager()
    {
      this.m_entityContainerMap = new Dictionary<EntityContainer, EntityContainer>();
      this.m_parentChildRelationships = new Dictionary<ExtentPair, List<ForeignKeyConstraint>>();
    }

    private static bool IsBinary(RelationshipType relationshipType)
    {
      int num = 0;
      foreach (EdmMember member in relationshipType.Members)
      {
        if (member is RelationshipEndMember)
        {
          ++num;
          if (num > 2)
            return false;
        }
      }
      return num == 2;
    }
  }
}
