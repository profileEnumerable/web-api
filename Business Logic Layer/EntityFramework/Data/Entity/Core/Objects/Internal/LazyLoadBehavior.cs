// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.LazyLoadBehavior
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal sealed class LazyLoadBehavior
  {
    internal static Func<TProxy, TItem, bool> GetInterceptorDelegate<TProxy, TItem>(
      EdmMember member,
      Func<object, object> getEntityWrapperDelegate)
      where TProxy : class
      where TItem : class
    {
      Func<TProxy, TItem, bool> func = (Func<TProxy, TItem, bool>) ((proxy, item) => true);
      if (member.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty)
      {
        NavigationProperty navProperty = (NavigationProperty) member;
        func = navProperty.ToEndMember.RelationshipMultiplicity != RelationshipMultiplicity.Many ? (Func<TProxy, TItem, bool>) ((proxy, item) => LazyLoadBehavior.LoadProperty<TItem>(item, navProperty.RelationshipType.Identity, navProperty.ToEndMember.Identity, true, getEntityWrapperDelegate((object) proxy))) : (Func<TProxy, TItem, bool>) ((proxy, item) => LazyLoadBehavior.LoadProperty<TItem>(item, navProperty.RelationshipType.Identity, navProperty.ToEndMember.Identity, false, getEntityWrapperDelegate((object) proxy)));
      }
      return func;
    }

    internal static bool IsLazyLoadCandidate(EntityType ospaceEntityType, EdmMember member)
    {
      bool flag = false;
      if (member.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty)
      {
        RelationshipMultiplicity relationshipMultiplicity = ((NavigationProperty) member).ToEndMember.RelationshipMultiplicity;
        Type propertyType = ospaceEntityType.ClrType.GetTopProperty(member.Name).PropertyType;
        switch (relationshipMultiplicity)
        {
          case RelationshipMultiplicity.ZeroOrOne:
          case RelationshipMultiplicity.One:
            flag = true;
            break;
          case RelationshipMultiplicity.Many:
            flag = propertyType.TryGetElementType(typeof (ICollection<>)) != (Type) null;
            break;
        }
      }
      return flag;
    }

    private static bool LoadProperty<TItem>(
      TItem propertyValue,
      string relationshipName,
      string targetRoleName,
      bool mustBeNull,
      object wrapperObject)
      where TItem : class
    {
      IEntityWrapper entityWrapper = (IEntityWrapper) wrapperObject;
      if (entityWrapper != null && entityWrapper.Context != null)
      {
        RelationshipManager relationshipManager = entityWrapper.RelationshipManager;
        if (relationshipManager != null && (!mustBeNull || (object) propertyValue == null))
          relationshipManager.GetRelatedEndInternal(relationshipName, targetRoleName).DeferredLoad();
      }
      return (object) propertyValue != null;
    }
  }
}
