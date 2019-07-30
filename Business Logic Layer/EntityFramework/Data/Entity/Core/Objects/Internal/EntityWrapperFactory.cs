// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.Internal.EntityWrapperFactory
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Reflection;

namespace System.Data.Entity.Core.Objects.Internal
{
  internal class EntityWrapperFactory
  {
    private static readonly Memoizer<Type, Func<object, IEntityWrapper>> _delegateCache = new Memoizer<Type, Func<object, IEntityWrapper>>(new Func<Type, Func<object, IEntityWrapper>>(EntityWrapperFactory.CreateWrapperDelegate), (IEqualityComparer<Type>) null);
    internal static readonly MethodInfo CreateWrapperDelegateTypedLightweightMethod = typeof (EntityWrapperFactory).GetOnlyDeclaredMethod("CreateWrapperDelegateTypedLightweight");
    internal static readonly MethodInfo CreateWrapperDelegateTypedWithRelationshipsMethod = typeof (EntityWrapperFactory).GetOnlyDeclaredMethod("CreateWrapperDelegateTypedWithRelationships");
    internal static readonly MethodInfo CreateWrapperDelegateTypedWithoutRelationshipsMethod = typeof (EntityWrapperFactory).GetOnlyDeclaredMethod("CreateWrapperDelegateTypedWithoutRelationships");

    internal static IEntityWrapper CreateNewWrapper(object entity, EntityKey key)
    {
      if (entity == null)
        return NullEntityWrapper.NullWrapper;
      IEntityWrapper entityWrapper = EntityWrapperFactory._delegateCache.Evaluate(entity.GetType())(entity);
      entityWrapper.RelationshipManager.SetWrappedOwner(entityWrapper, entity);
      if ((object) key != null && (object) entityWrapper.EntityKey == null)
        entityWrapper.EntityKey = key;
      EntityProxyTypeInfo proxyTypeInfo;
      if (EntityProxyFactory.TryGetProxyType(entity.GetType(), out proxyTypeInfo))
        proxyTypeInfo.SetEntityWrapper(entityWrapper);
      return entityWrapper;
    }

    private static Func<object, IEntityWrapper> CreateWrapperDelegate(
      Type entityType)
    {
      bool flag1 = typeof (IEntityWithRelationships).IsAssignableFrom(entityType);
      bool flag2 = typeof (IEntityWithChangeTracker).IsAssignableFrom(entityType);
      bool flag3 = typeof (IEntityWithKey).IsAssignableFrom(entityType);
      bool flag4 = EntityProxyFactory.IsProxyType(entityType);
      return (Func<object, IEntityWrapper>) (!flag1 || !flag2 || (!flag3 || flag4) ? (!flag1 ? EntityWrapperFactory.CreateWrapperDelegateTypedWithoutRelationshipsMethod : EntityWrapperFactory.CreateWrapperDelegateTypedWithRelationshipsMethod) : EntityWrapperFactory.CreateWrapperDelegateTypedLightweightMethod).MakeGenericMethod(entityType).Invoke((object) null, new object[0]);
    }

    private static Func<object, IEntityWrapper> CreateWrapperDelegateTypedLightweight<TEntity>() where TEntity : class, IEntityWithRelationships, IEntityWithKey, IEntityWithChangeTracker
    {
      bool overridesEquals = typeof (TEntity).OverridesEqualsOrGetHashCode();
      return (Func<object, IEntityWrapper>) (entity => (IEntityWrapper) new LightweightEntityWrapper<TEntity>((TEntity) entity, overridesEquals));
    }

    private static Func<object, IEntityWrapper> CreateWrapperDelegateTypedWithRelationships<TEntity>() where TEntity : class, IEntityWithRelationships
    {
      bool overridesEquals = typeof (TEntity).OverridesEqualsOrGetHashCode();
      Func<object, IPropertyAccessorStrategy> propertyAccessorStrategy;
      Func<object, IEntityKeyStrategy> keyStrategy;
      Func<object, IChangeTrackingStrategy> changeTrackingStrategy;
      EntityWrapperFactory.CreateStrategies<TEntity>(out propertyAccessorStrategy, out changeTrackingStrategy, out keyStrategy);
      return (Func<object, IEntityWrapper>) (entity => (IEntityWrapper) new EntityWrapperWithRelationships<TEntity>((TEntity) entity, propertyAccessorStrategy, changeTrackingStrategy, keyStrategy, overridesEquals));
    }

    private static Func<object, IEntityWrapper> CreateWrapperDelegateTypedWithoutRelationships<TEntity>() where TEntity : class
    {
      bool overridesEquals = typeof (TEntity).OverridesEqualsOrGetHashCode();
      Func<object, IPropertyAccessorStrategy> propertyAccessorStrategy;
      Func<object, IEntityKeyStrategy> keyStrategy;
      Func<object, IChangeTrackingStrategy> changeTrackingStrategy;
      EntityWrapperFactory.CreateStrategies<TEntity>(out propertyAccessorStrategy, out changeTrackingStrategy, out keyStrategy);
      return (Func<object, IEntityWrapper>) (entity => (IEntityWrapper) new EntityWrapperWithoutRelationships<TEntity>((TEntity) entity, propertyAccessorStrategy, changeTrackingStrategy, keyStrategy, overridesEquals));
    }

    private static void CreateStrategies<TEntity>(
      out Func<object, IPropertyAccessorStrategy> createPropertyAccessorStrategy,
      out Func<object, IChangeTrackingStrategy> createChangeTrackingStrategy,
      out Func<object, IEntityKeyStrategy> createKeyStrategy)
    {
      Type type = typeof (TEntity);
      bool flag1 = typeof (IEntityWithRelationships).IsAssignableFrom(type);
      bool flag2 = typeof (IEntityWithChangeTracker).IsAssignableFrom(type);
      bool flag3 = typeof (IEntityWithKey).IsAssignableFrom(type);
      bool flag4 = EntityProxyFactory.IsProxyType(type);
      createPropertyAccessorStrategy = !flag1 || flag4 ? EntityWrapperFactory.GetPocoPropertyAccessorStrategyFunc() : EntityWrapperFactory.GetNullPropertyAccessorStrategyFunc();
      createChangeTrackingStrategy = !flag2 ? EntityWrapperFactory.GetSnapshotChangeTrackingStrategyFunc() : EntityWrapperFactory.GetEntityWithChangeTrackerStrategyFunc();
      if (flag3)
        createKeyStrategy = EntityWrapperFactory.GetEntityWithKeyStrategyStrategyFunc();
      else
        createKeyStrategy = EntityWrapperFactory.GetPocoEntityKeyStrategyFunc();
    }

    internal IEntityWrapper WrapEntityUsingContext(
      object entity,
      ObjectContext context)
    {
      EntityEntry existingEntry;
      return this.WrapEntityUsingStateManagerGettingEntry(entity, context == null ? (ObjectStateManager) null : context.ObjectStateManager, out existingEntry);
    }

    internal IEntityWrapper WrapEntityUsingContextGettingEntry(
      object entity,
      ObjectContext context,
      out EntityEntry existingEntry)
    {
      return this.WrapEntityUsingStateManagerGettingEntry(entity, context == null ? (ObjectStateManager) null : context.ObjectStateManager, out existingEntry);
    }

    internal IEntityWrapper WrapEntityUsingStateManager(
      object entity,
      ObjectStateManager stateManager)
    {
      EntityEntry existingEntry;
      return this.WrapEntityUsingStateManagerGettingEntry(entity, stateManager, out existingEntry);
    }

    internal virtual IEntityWrapper WrapEntityUsingStateManagerGettingEntry(
      object entity,
      ObjectStateManager stateManager,
      out EntityEntry existingEntry)
    {
      IEntityWrapper wrapper = (IEntityWrapper) null;
      existingEntry = (EntityEntry) null;
      if (entity == null)
        return NullEntityWrapper.NullWrapper;
      if (stateManager != null)
      {
        existingEntry = stateManager.FindEntityEntry(entity);
        if (existingEntry != null)
          return existingEntry.WrappedEntity;
        if (stateManager.TransactionManager.TrackProcessedEntities && stateManager.TransactionManager.WrappedEntities.TryGetValue(entity, out wrapper))
          return wrapper;
      }
      IEntityWithRelationships withRelationships = entity as IEntityWithRelationships;
      if (withRelationships != null)
      {
        RelationshipManager relationshipManager = withRelationships.RelationshipManager;
        if (relationshipManager == null)
          throw new InvalidOperationException(Strings.RelationshipManager_UnexpectedNull);
        IEntityWrapper wrappedOwner = relationshipManager.WrappedOwner;
        if (!object.ReferenceEquals(wrappedOwner.Entity, entity))
          throw new InvalidOperationException(Strings.RelationshipManager_InvalidRelationshipManagerOwner);
        return wrappedOwner;
      }
      EntityProxyFactory.TryGetProxyWrapper(entity, out wrapper);
      if (wrapper == null)
      {
        IEntityWithKey entityWithKey = entity as IEntityWithKey;
        wrapper = EntityWrapperFactory.CreateNewWrapper(entity, entityWithKey == null ? (EntityKey) null : entityWithKey.EntityKey);
      }
      if (stateManager != null && stateManager.TransactionManager.TrackProcessedEntities)
        stateManager.TransactionManager.WrappedEntities.Add(entity, wrapper);
      return wrapper;
    }

    internal virtual void UpdateNoTrackingWrapper(
      IEntityWrapper wrapper,
      ObjectContext context,
      EntitySet entitySet)
    {
      if (wrapper.EntityKey == (EntityKey) null)
        wrapper.EntityKey = context.ObjectStateManager.CreateEntityKey(entitySet, wrapper.Entity);
      if (wrapper.Context != null)
        return;
      wrapper.AttachContext(context, entitySet, MergeOption.NoTracking);
    }

    internal static Func<object, IPropertyAccessorStrategy> GetPocoPropertyAccessorStrategyFunc()
    {
      return (Func<object, IPropertyAccessorStrategy>) (entity => (IPropertyAccessorStrategy) new PocoPropertyAccessorStrategy(entity));
    }

    internal static Func<object, IPropertyAccessorStrategy> GetNullPropertyAccessorStrategyFunc()
    {
      return (Func<object, IPropertyAccessorStrategy>) (entity => (IPropertyAccessorStrategy) null);
    }

    internal static Func<object, IChangeTrackingStrategy> GetEntityWithChangeTrackerStrategyFunc()
    {
      return (Func<object, IChangeTrackingStrategy>) (entity => (IChangeTrackingStrategy) new EntityWithChangeTrackerStrategy((IEntityWithChangeTracker) entity));
    }

    internal static Func<object, IChangeTrackingStrategy> GetSnapshotChangeTrackingStrategyFunc()
    {
      return (Func<object, IChangeTrackingStrategy>) (entity => (IChangeTrackingStrategy) SnapshotChangeTrackingStrategy.Instance);
    }

    internal static Func<object, IEntityKeyStrategy> GetEntityWithKeyStrategyStrategyFunc()
    {
      return (Func<object, IEntityKeyStrategy>) (entity => (IEntityKeyStrategy) new EntityWithKeyStrategy((IEntityWithKey) entity));
    }

    internal static Func<object, IEntityKeyStrategy> GetPocoEntityKeyStrategyFunc()
    {
      return (Func<object, IEntityKeyStrategy>) (entity => (IEntityKeyStrategy) new PocoEntityKeyStrategy());
    }
  }
}
