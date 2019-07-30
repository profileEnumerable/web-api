// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Mapping.ViewGeneration.DiscriminatorMap
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.Internal;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Query.InternalTrees;
using System.Linq;

namespace System.Data.Entity.Core.Mapping.ViewGeneration
{
  internal class DiscriminatorMap
  {
    internal readonly DbPropertyExpression Discriminator;
    internal readonly ReadOnlyCollection<KeyValuePair<object, EntityType>> TypeMap;
    internal readonly ReadOnlyCollection<KeyValuePair<EdmProperty, DbExpression>> PropertyMap;
    internal readonly ReadOnlyCollection<KeyValuePair<RelProperty, DbExpression>> RelPropertyMap;
    internal readonly EntitySet EntitySet;

    private DiscriminatorMap(
      DbPropertyExpression discriminator,
      List<KeyValuePair<object, EntityType>> typeMap,
      Dictionary<EdmProperty, DbExpression> propertyMap,
      Dictionary<RelProperty, DbExpression> relPropertyMap,
      EntitySet entitySet)
    {
      this.Discriminator = discriminator;
      this.TypeMap = new ReadOnlyCollection<KeyValuePair<object, EntityType>>((IList<KeyValuePair<object, EntityType>>) typeMap);
      this.PropertyMap = new ReadOnlyCollection<KeyValuePair<EdmProperty, DbExpression>>((IList<KeyValuePair<EdmProperty, DbExpression>>) propertyMap.ToList<KeyValuePair<EdmProperty, DbExpression>>());
      this.RelPropertyMap = new ReadOnlyCollection<KeyValuePair<RelProperty, DbExpression>>((IList<KeyValuePair<RelProperty, DbExpression>>) relPropertyMap.ToList<KeyValuePair<RelProperty, DbExpression>>());
      this.EntitySet = entitySet;
    }

    internal static bool TryCreateDiscriminatorMap(
      EntitySet entitySet,
      DbExpression queryView,
      out DiscriminatorMap discriminatorMap)
    {
      discriminatorMap = (DiscriminatorMap) null;
      if (queryView.ExpressionKind != DbExpressionKind.Project)
        return false;
      DbProjectExpression projectExpression = (DbProjectExpression) queryView;
      if (projectExpression.Projection.ExpressionKind != DbExpressionKind.Case)
        return false;
      DbCaseExpression projection = (DbCaseExpression) projectExpression.Projection;
      if (projectExpression.Projection.ResultType.EdmType.BuiltInTypeKind != BuiltInTypeKind.EntityType || projectExpression.Input.Expression.ExpressionKind != DbExpressionKind.Filter)
        return false;
      DbFilterExpression expression1 = (DbFilterExpression) projectExpression.Input.Expression;
      HashSet<object> discriminatorDomain = new HashSet<object>();
      if (!ViewSimplifier.TryMatchDiscriminatorPredicate(expression1, (Action<DbComparisonExpression, object>) ((equalsExp, discriminatorValue) => discriminatorDomain.Add(discriminatorValue))))
        return false;
      List<KeyValuePair<object, EntityType>> keyValuePairList = new List<KeyValuePair<object, EntityType>>();
      Dictionary<EdmProperty, DbExpression> propertyMap = new Dictionary<EdmProperty, DbExpression>();
      Dictionary<RelProperty, DbExpression> relPropertyMap = new Dictionary<RelProperty, DbExpression>();
      Dictionary<EntityType, List<RelProperty>> typeToRelPropertyMap = new Dictionary<EntityType, List<RelProperty>>();
      DbPropertyExpression discriminator = (DbPropertyExpression) null;
      EdmProperty edmProperty = (EdmProperty) null;
      for (int index = 0; index < projection.When.Count; ++index)
      {
        DbExpression expression2 = projection.When[index];
        DbExpression then = projection.Then[index];
        string variableName = projectExpression.Input.VariableName;
        DbPropertyExpression property;
        object key;
        if (!ViewSimplifier.TryMatchPropertyEqualsValue(expression2, variableName, out property, out key))
          return false;
        if (edmProperty == null)
          edmProperty = (EdmProperty) property.Property;
        else if (edmProperty != property.Property)
          return false;
        discriminator = property;
        EntityType entityType;
        if (!DiscriminatorMap.TryMatchEntityTypeConstructor(then, propertyMap, relPropertyMap, typeToRelPropertyMap, out entityType))
          return false;
        keyValuePairList.Add(new KeyValuePair<object, EntityType>(key, entityType));
        discriminatorDomain.Remove(key);
      }
      EntityType entityType1;
      if (1 != discriminatorDomain.Count || projection.Else == null || !DiscriminatorMap.TryMatchEntityTypeConstructor(projection.Else, propertyMap, relPropertyMap, typeToRelPropertyMap, out entityType1))
        return false;
      keyValuePairList.Add(new KeyValuePair<object, EntityType>(discriminatorDomain.Single<object>(), entityType1));
      if (!DiscriminatorMap.CheckForMissingRelProperties(relPropertyMap, typeToRelPropertyMap) || keyValuePairList.Select<KeyValuePair<object, EntityType>, object>((Func<KeyValuePair<object, EntityType>, object>) (map => map.Key)).Distinct<object>((IEqualityComparer<object>) TrailingSpaceComparer.Instance).Count<object>() != keyValuePairList.Count)
        return false;
      discriminatorMap = new DiscriminatorMap(discriminator, keyValuePairList, propertyMap, relPropertyMap, entitySet);
      return true;
    }

    private static bool CheckForMissingRelProperties(
      Dictionary<RelProperty, DbExpression> relPropertyMap,
      Dictionary<EntityType, List<RelProperty>> typeToRelPropertyMap)
    {
      foreach (RelProperty key in relPropertyMap.Keys)
      {
        foreach (KeyValuePair<EntityType, List<RelProperty>> typeToRelProperty in typeToRelPropertyMap)
        {
          if (typeToRelProperty.Key.IsSubtypeOf(key.FromEnd.TypeUsage.EdmType) && !typeToRelProperty.Value.Contains(key))
            return false;
        }
      }
      return true;
    }

    private static bool TryMatchEntityTypeConstructor(
      DbExpression then,
      Dictionary<EdmProperty, DbExpression> propertyMap,
      Dictionary<RelProperty, DbExpression> relPropertyMap,
      Dictionary<EntityType, List<RelProperty>> typeToRelPropertyMap,
      out EntityType entityType)
    {
      if (then.ExpressionKind != DbExpressionKind.NewInstance)
      {
        entityType = (EntityType) null;
        return false;
      }
      DbNewInstanceExpression instanceExpression = (DbNewInstanceExpression) then;
      entityType = (EntityType) instanceExpression.ResultType.EdmType;
      for (int index = 0; index < entityType.Properties.Count; ++index)
      {
        EdmProperty property = entityType.Properties[index];
        DbExpression x = instanceExpression.Arguments[index];
        DbExpression y;
        if (propertyMap.TryGetValue(property, out y))
        {
          if (!DiscriminatorMap.ExpressionsCompatible(x, y))
            return false;
        }
        else
          propertyMap.Add(property, x);
      }
      if (instanceExpression.HasRelatedEntityReferences)
      {
        List<RelProperty> relPropertyList;
        if (!typeToRelPropertyMap.TryGetValue(entityType, out relPropertyList))
        {
          relPropertyList = new List<RelProperty>();
          typeToRelPropertyMap[entityType] = relPropertyList;
        }
        foreach (DbRelatedEntityRef relatedEntityReference in instanceExpression.RelatedEntityReferences)
        {
          RelProperty key = new RelProperty((RelationshipType) relatedEntityReference.TargetEnd.DeclaringType, relatedEntityReference.SourceEnd, relatedEntityReference.TargetEnd);
          DbExpression targetEntityReference = relatedEntityReference.TargetEntityReference;
          DbExpression y;
          if (relPropertyMap.TryGetValue(key, out y))
          {
            if (!DiscriminatorMap.ExpressionsCompatible(targetEntityReference, y))
              return false;
          }
          else
            relPropertyMap.Add(key, targetEntityReference);
          relPropertyList.Add(key);
        }
      }
      return true;
    }

    private static bool ExpressionsCompatible(DbExpression x, DbExpression y)
    {
      if (x.ExpressionKind != y.ExpressionKind)
        return false;
      switch (x.ExpressionKind)
      {
        case DbExpressionKind.NewInstance:
          DbNewInstanceExpression instanceExpression1 = (DbNewInstanceExpression) x;
          DbNewInstanceExpression instanceExpression2 = (DbNewInstanceExpression) y;
          if (!instanceExpression1.ResultType.EdmType.EdmEquals((MetadataItem) instanceExpression2.ResultType.EdmType))
            return false;
          for (int index = 0; index < instanceExpression1.Arguments.Count; ++index)
          {
            if (!DiscriminatorMap.ExpressionsCompatible(instanceExpression1.Arguments[index], instanceExpression2.Arguments[index]))
              return false;
          }
          return true;
        case DbExpressionKind.Property:
          DbPropertyExpression propertyExpression1 = (DbPropertyExpression) x;
          DbPropertyExpression propertyExpression2 = (DbPropertyExpression) y;
          if (propertyExpression1.Property == propertyExpression2.Property)
            return DiscriminatorMap.ExpressionsCompatible(propertyExpression1.Instance, propertyExpression2.Instance);
          return false;
        case DbExpressionKind.Ref:
          DbRefExpression dbRefExpression1 = (DbRefExpression) x;
          DbRefExpression dbRefExpression2 = (DbRefExpression) y;
          if (dbRefExpression1.EntitySet.EdmEquals((MetadataItem) dbRefExpression2.EntitySet))
            return DiscriminatorMap.ExpressionsCompatible(dbRefExpression1.Argument, dbRefExpression2.Argument);
          return false;
        case DbExpressionKind.VariableReference:
          return ((DbVariableReferenceExpression) x).VariableName == ((DbVariableReferenceExpression) y).VariableName;
        default:
          return false;
      }
    }
  }
}
