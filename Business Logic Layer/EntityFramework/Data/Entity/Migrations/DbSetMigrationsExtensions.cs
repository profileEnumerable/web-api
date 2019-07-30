// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.DbSetMigrationsExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Internal.Linq;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Migrations
{
  /// <summary>
  /// A set of extension methods for <see cref="T:System.Data.Entity.IDbSet`1" />
  /// </summary>
  public static class DbSetMigrationsExtensions
  {
    /// <summary>
    /// Adds or updates entities by key when SaveChanges is called. Equivalent to an "upsert" operation
    /// from database terminology.
    /// This method can useful when seeding data using Migrations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to add or update.</typeparam>
    /// <param name="set">The set to which the entities belong.</param>
    /// <param name="entities"> The entities to add or update. </param>
    /// <remarks>
    /// When the <paramref name="set" /> parameter is a custom or fake IDbSet implementation, this method will
    /// attempt to locate and invoke a public, instance method with the same signature as this extension method.
    /// </remarks>
    public static void AddOrUpdate<TEntity>(this IDbSet<TEntity> set, params TEntity[] entities) where TEntity : class
    {
      Check.NotNull<IDbSet<TEntity>>(set, nameof (set));
      Check.NotNull<TEntity[]>(entities, nameof (entities));
      DbSet<TEntity> set1 = set as DbSet<TEntity>;
      if (set1 != null)
      {
        InternalSet<TEntity> internalSet = (InternalSet<TEntity>) ((IInternalSetAdapter) set1).InternalSet;
        if (internalSet != null)
        {
          set1.AddOrUpdate<TEntity>(DbSetMigrationsExtensions.GetKeyProperties<TEntity>(typeof (TEntity), internalSet), internalSet, entities);
          return;
        }
      }
      Type type = set.GetType();
      MethodInfo declaredMethod = type.GetDeclaredMethod(nameof (AddOrUpdate), typeof (TEntity[]));
      if (declaredMethod == (MethodInfo) null)
        throw Error.UnableToDispatchAddOrUpdate((object) type);
      declaredMethod.Invoke((object) set, (object[]) new TEntity[1][]
      {
        entities
      });
    }

    /// <summary>
    /// Adds or updates entities by a custom identification expression when SaveChanges is called.
    /// Equivalent to an "upsert" operation from database terminology.
    /// This method can useful when seeding data using Migrations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to add or update.</typeparam>
    /// <param name="set">The set to which the entities belong.</param>
    /// <param name="identifierExpression"> An expression specifying the properties that should be used when determining whether an Add or Update operation should be performed. </param>
    /// <param name="entities"> The entities to add or update. </param>
    /// <remarks>
    /// When the <paramref name="set" /> parameter is a custom or fake IDbSet implementation, this method will
    /// attempt to locate and invoke a public, instance method with the same signature as this extension method.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public static void AddOrUpdate<TEntity>(
      this IDbSet<TEntity> set,
      Expression<Func<TEntity, object>> identifierExpression,
      params TEntity[] entities)
      where TEntity : class
    {
      Check.NotNull<IDbSet<TEntity>>(set, nameof (set));
      Check.NotNull<Expression<Func<TEntity, object>>>(identifierExpression, nameof (identifierExpression));
      Check.NotNull<TEntity[]>(entities, nameof (entities));
      DbSet<TEntity> set1 = set as DbSet<TEntity>;
      if (set1 != null)
      {
        InternalSet<TEntity> internalSet = (InternalSet<TEntity>) ((IInternalSetAdapter) set1).InternalSet;
        if (internalSet != null)
        {
          IEnumerable<PropertyPath> propertyAccessList = identifierExpression.GetSimplePropertyAccessList();
          set1.AddOrUpdate<TEntity>(propertyAccessList, internalSet, entities);
          return;
        }
      }
      Type type = set.GetType();
      MethodInfo declaredMethod = type.GetDeclaredMethod(nameof (AddOrUpdate), typeof (Expression<Func<TEntity, object>>), typeof (TEntity[]));
      if (declaredMethod == (MethodInfo) null)
        throw Error.UnableToDispatchAddOrUpdate((object) type);
      declaredMethod.Invoke((object) set, new object[2]
      {
        (object) identifierExpression,
        (object) entities
      });
    }

    private static void AddOrUpdate<TEntity>(
      this DbSet<TEntity> set,
      IEnumerable<PropertyPath> identifyingProperties,
      InternalSet<TEntity> internalSet,
      params TEntity[] entities)
      where TEntity : class
    {
      IEnumerable<PropertyPath> keyProperties = DbSetMigrationsExtensions.GetKeyProperties<TEntity>(typeof (TEntity), internalSet);
      ParameterExpression parameter = Expression.Parameter(typeof (TEntity));
      foreach (TEntity entity1 in entities)
      {
        TEntity entity = entity1;
        Expression body = identifyingProperties.Select<PropertyPath, BinaryExpression>((Func<PropertyPath, BinaryExpression>) (pi => Expression.Equal((Expression) Expression.Property((Expression) parameter, pi.Single<PropertyInfo>()), (Expression) Expression.Constant(pi.Last<PropertyInfo>().GetValue((object) entity, (object[]) null))))).Aggregate<BinaryExpression, Expression>((Expression) null, (Func<Expression, BinaryExpression, Expression>) ((current, predicate) =>
        {
          if (current != null)
            return (Expression) Expression.AndAlso(current, (Expression) predicate);
          return (Expression) predicate;
        }));
        TEntity entity2 = set.SingleOrDefault<TEntity>(Expression.Lambda<Func<TEntity, bool>>(body, parameter));
        if ((object) entity2 != null)
        {
          foreach (PropertyPath source in keyProperties)
            source.Single<PropertyInfo>().GetPropertyInfoForSet().SetValue((object) (TEntity) entity, source.Single<PropertyInfo>().GetValue((object) entity2, (object[]) null), (object[]) null);
          internalSet.InternalContext.Owner.Entry<TEntity>(entity2).CurrentValues.SetValues((object) (TEntity) entity);
        }
        else
          internalSet.Add((object) (TEntity) entity);
      }
    }

    private static IEnumerable<PropertyPath> GetKeyProperties<TEntity>(
      Type entityType,
      InternalSet<TEntity> internalSet)
      where TEntity : class
    {
      return internalSet.InternalContext.GetEntitySetAndBaseTypeForType(typeof (TEntity)).EntitySet.ElementType.KeyMembers.Select<EdmMember, PropertyPath>((Func<EdmMember, PropertyPath>) (km => new PropertyPath(entityType.GetAnyProperty(km.Name))));
    }
  }
}
