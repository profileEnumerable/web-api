// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Infrastructure.ModificationCommandTreeGenerator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Internal;
using System.Data.Entity.ModelConfiguration.Edm;
using System.Data.Entity.Spatial;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Migrations.Infrastructure
{
  internal class ModificationCommandTreeGenerator
  {
    private readonly DbCompiledModel _compiledModel;
    private readonly DbConnection _connection;
    private readonly MetadataWorkspace _metadataWorkspace;

    public ModificationCommandTreeGenerator(DbModel model, DbConnection connection = null)
    {
      this._compiledModel = new DbCompiledModel(model);
      this._connection = connection;
      using (DbContext context = this.CreateContext())
        this._metadataWorkspace = ((IObjectContextAdapter) context).ObjectContext.MetadataWorkspace;
    }

    private DbContext CreateContext()
    {
      if (this._connection != null)
        return (DbContext) new ModificationCommandTreeGenerator.TempDbContext(this._connection, this._compiledModel);
      return (DbContext) new ModificationCommandTreeGenerator.TempDbContext(this._compiledModel);
    }

    public IEnumerable<DbInsertCommandTree> GenerateAssociationInsert(
      string associationIdentity)
    {
      return this.GenerateAssociation<DbInsertCommandTree>(associationIdentity, EntityState.Added);
    }

    public IEnumerable<DbDeleteCommandTree> GenerateAssociationDelete(
      string associationIdentity)
    {
      return this.GenerateAssociation<DbDeleteCommandTree>(associationIdentity, EntityState.Deleted);
    }

    private IEnumerable<TCommandTree> GenerateAssociation<TCommandTree>(
      string associationIdentity,
      EntityState state)
      where TCommandTree : DbCommandTree
    {
      AssociationType associationType = this._metadataWorkspace.GetItem<AssociationType>(associationIdentity, DataSpace.CSpace);
      using (DbContext context = this.CreateContext())
      {
        EntityType sourceEntityType = associationType.SourceEnd.GetEntityType();
        object sourceEntity = this.InstantiateAndAttachEntity(sourceEntityType, context);
        EntityType targetEntityType = associationType.TargetEnd.GetEntityType();
        object targetEntity = sourceEntityType.GetRootType() == targetEntityType.GetRootType() ? sourceEntity : this.InstantiateAndAttachEntity(targetEntityType, context);
        ObjectStateManager objectStateManager = ((IObjectContextAdapter) context).ObjectContext.ObjectStateManager;
        objectStateManager.ChangeRelationshipState(sourceEntity, targetEntity, associationType.FullName, associationType.TargetEnd.Name, state == EntityState.Deleted ? state : EntityState.Added);
        using (CommandTracer commandTracer = new CommandTracer(context))
        {
          context.SaveChanges();
          foreach (DbCommandTree commandTree in commandTracer.CommandTrees)
            yield return (TCommandTree) commandTree;
        }
      }
    }

    private object InstantiateAndAttachEntity(EntityType entityType, DbContext context)
    {
      Type clrType = EntityTypeExtensions.GetClrType(entityType);
      DbSet set = context.Set(clrType);
      object entity = this.InstantiateEntity(entityType, context, clrType, set);
      ModificationCommandTreeGenerator.SetFakeReferenceKeyValues(entity, entityType);
      ModificationCommandTreeGenerator.SetFakeKeyValues(entity, entityType);
      set.Attach(entity);
      return entity;
    }

    private object InstantiateEntity(
      EntityType entityType,
      DbContext context,
      Type clrType,
      DbSet set)
    {
      object structuralObject;
      if (!clrType.IsAbstract())
      {
        structuralObject = set.Create();
      }
      else
      {
        EntityType entityType1 = this._metadataWorkspace.GetItems<EntityType>(DataSpace.CSpace).First<EntityType>((Func<EntityType, bool>) (et =>
        {
          if (entityType.IsAncestorOf(et))
            return !et.Abstract;
          return false;
        }));
        structuralObject = context.Set(EntityTypeExtensions.GetClrType(entityType1)).Create();
      }
      ModificationCommandTreeGenerator.InstantiateComplexProperties(structuralObject, (IEnumerable<EdmProperty>) entityType.Properties);
      return structuralObject;
    }

    public IEnumerable<DbModificationCommandTree> GenerateInsert(
      string entityIdentity)
    {
      return this.Generate(entityIdentity, EntityState.Added);
    }

    public IEnumerable<DbModificationCommandTree> GenerateUpdate(
      string entityIdentity)
    {
      return this.Generate(entityIdentity, EntityState.Modified);
    }

    public IEnumerable<DbModificationCommandTree> GenerateDelete(
      string entityIdentity)
    {
      return this.Generate(entityIdentity, EntityState.Deleted);
    }

    private IEnumerable<DbModificationCommandTree> Generate(
      string entityIdentity,
      EntityState state)
    {
      EntityType entityType = this._metadataWorkspace.GetItem<EntityType>(entityIdentity, DataSpace.CSpace);
      using (DbContext context = this.CreateContext())
      {
        object entity = this.InstantiateAndAttachEntity(entityType, context);
        if (state != EntityState.Deleted)
          context.Entry(entity).State = state;
        this.ChangeRelationshipStates(context, entityType, entity, state);
        if (state == EntityState.Deleted)
          context.Entry(entity).State = state;
        this.HandleTableSplitting(context, entityType, entity, state);
        using (CommandTracer commandTracer = new CommandTracer(context))
        {
          ((IObjectContextAdapter) context).ObjectContext.SaveChanges(SaveOptions.None);
          foreach (DbCommandTree commandTree in commandTracer.CommandTrees)
            yield return (DbModificationCommandTree) commandTree;
        }
      }
    }

    private void ChangeRelationshipStates(
      DbContext context,
      EntityType entityType,
      object entity,
      EntityState state)
    {
      ObjectStateManager objectStateManager = ((IObjectContextAdapter) context).ObjectContext.ObjectStateManager;
      foreach (AssociationType associationType in this._metadataWorkspace.GetItems<AssociationType>(DataSpace.CSpace).Where<AssociationType>((Func<AssociationType, bool>) (at =>
      {
        if (at.IsForeignKey || at.IsManyToMany())
          return false;
        if (!at.SourceEnd.GetEntityType().IsAssignableFrom((EdmType) entityType))
          return at.TargetEnd.GetEntityType().IsAssignableFrom((EdmType) entityType);
        return true;
      })))
      {
        AssociationEndMember principalEnd;
        AssociationEndMember dependentEnd;
        if (!associationType.TryGuessPrincipalAndDependentEnds(out principalEnd, out dependentEnd))
        {
          principalEnd = associationType.SourceEnd;
          dependentEnd = associationType.TargetEnd;
        }
        if (dependentEnd.GetEntityType().IsAssignableFrom((EdmType) entityType))
        {
          EntityType entityType1 = principalEnd.GetEntityType();
          Type clrType = EntityTypeExtensions.GetClrType(entityType1);
          DbSet set = context.Set(clrType);
          object obj1 = set.Local.Cast<object>().SingleOrDefault<object>();
          if (obj1 == null || object.ReferenceEquals(entity, obj1) && state == EntityState.Added)
          {
            obj1 = this.InstantiateEntity(entityType1, context, clrType, set);
            ModificationCommandTreeGenerator.SetFakeReferenceKeyValues(obj1, entityType1);
            set.Attach(obj1);
          }
          if (principalEnd.IsRequired() && state == EntityState.Modified)
          {
            object obj2 = this.InstantiateEntity(entityType1, context, clrType, set);
            ModificationCommandTreeGenerator.SetFakeKeyValues(obj2, entityType1);
            set.Attach(obj2);
            objectStateManager.ChangeRelationshipState(entity, obj2, associationType.FullName, principalEnd.Name, EntityState.Deleted);
          }
          objectStateManager.ChangeRelationshipState(entity, obj1, associationType.FullName, principalEnd.Name, state == EntityState.Deleted ? state : EntityState.Added);
        }
      }
    }

    private void HandleTableSplitting(
      DbContext context,
      EntityType entityType,
      object entity,
      EntityState state)
    {
      foreach (AssociationType associationType in this._metadataWorkspace.GetItems<AssociationType>(DataSpace.CSpace).Where<AssociationType>((Func<AssociationType, bool>) (at =>
      {
        if (at.IsForeignKey && at.IsRequiredToRequired() && !at.IsSelfReferencing() && (at.SourceEnd.GetEntityType().IsAssignableFrom((EdmType) entityType) || at.TargetEnd.GetEntityType().IsAssignableFrom((EdmType) entityType)))
          return this._metadataWorkspace.GetItems<AssociationType>(DataSpace.SSpace).All<AssociationType>((Func<AssociationType, bool>) (fk => fk.Name != at.Name));
        return false;
      })))
      {
        AssociationEndMember principalEnd;
        AssociationEndMember dependentEnd;
        if (!associationType.TryGuessPrincipalAndDependentEnds(out principalEnd, out dependentEnd))
        {
          principalEnd = associationType.SourceEnd;
          dependentEnd = associationType.TargetEnd;
        }
        bool flag = false;
        EntityType entityType1;
        if (principalEnd.GetEntityType().GetRootType() == entityType.GetRootType())
        {
          flag = true;
          entityType1 = dependentEnd.GetEntityType();
        }
        else
          entityType1 = principalEnd.GetEntityType();
        object entity1 = this.InstantiateAndAttachEntity(entityType1, context);
        if (!flag)
        {
          switch (state)
          {
            case EntityState.Added:
              context.Entry(entity).State = EntityState.Modified;
              continue;
            case EntityState.Deleted:
              context.Entry(entity).State = EntityState.Unchanged;
              continue;
            default:
              continue;
          }
        }
        else if (state != EntityState.Modified)
          context.Entry(entity1).State = state;
      }
    }

    private static void SetFakeReferenceKeyValues(object entity, EntityType entityType)
    {
      foreach (EdmProperty keyProperty in entityType.KeyProperties)
      {
        PropertyInfo clrPropertyInfo = keyProperty.GetClrPropertyInfo();
        object referenceKeyValue = ModificationCommandTreeGenerator.GetFakeReferenceKeyValue(keyProperty.UnderlyingPrimitiveType.PrimitiveTypeKind);
        if (referenceKeyValue != null)
          clrPropertyInfo.GetPropertyInfoForSet().SetValue(entity, referenceKeyValue, (object[]) null);
      }
    }

    private static object GetFakeReferenceKeyValue(PrimitiveTypeKind primitiveTypeKind)
    {
      switch (primitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
          return (object) new byte[0];
        case PrimitiveTypeKind.String:
          return (object) "42";
        case PrimitiveTypeKind.Geometry:
          return (object) DefaultSpatialServices.Instance.GeometryFromText("POINT (4 2)");
        case PrimitiveTypeKind.Geography:
          return (object) DefaultSpatialServices.Instance.GeographyFromText("POINT (4 2)");
        default:
          return (object) null;
      }
    }

    private static void SetFakeKeyValues(object entity, EntityType entityType)
    {
      foreach (EdmProperty keyProperty in entityType.KeyProperties)
      {
        PropertyInfo clrPropertyInfo = keyProperty.GetClrPropertyInfo();
        object fakeKeyValue = ModificationCommandTreeGenerator.GetFakeKeyValue(keyProperty.UnderlyingPrimitiveType.PrimitiveTypeKind);
        clrPropertyInfo.GetPropertyInfoForSet().SetValue(entity, fakeKeyValue, (object[]) null);
      }
    }

    private static object GetFakeKeyValue(PrimitiveTypeKind primitiveTypeKind)
    {
      switch (primitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
          return (object) new byte[1]{ (byte) 66 };
        case PrimitiveTypeKind.Boolean:
          return (object) true;
        case PrimitiveTypeKind.Byte:
          return (object) (byte) 66;
        case PrimitiveTypeKind.DateTime:
          return (object) DateTime.Now;
        case PrimitiveTypeKind.Decimal:
          return (object) new Decimal(42);
        case PrimitiveTypeKind.Double:
          return (object) 42.0;
        case PrimitiveTypeKind.Guid:
          return (object) Guid.NewGuid();
        case PrimitiveTypeKind.Single:
          return (object) 42f;
        case PrimitiveTypeKind.SByte:
          return (object) (sbyte) 42;
        case PrimitiveTypeKind.Int16:
          return (object) (short) 42;
        case PrimitiveTypeKind.Int32:
          return (object) 42;
        case PrimitiveTypeKind.Int64:
          return (object) 42L;
        case PrimitiveTypeKind.String:
          return (object) "42'";
        case PrimitiveTypeKind.Time:
          return (object) TimeSpan.FromMilliseconds(42.0);
        case PrimitiveTypeKind.DateTimeOffset:
          return (object) DateTimeOffset.Now;
        case PrimitiveTypeKind.Geometry:
          return (object) DefaultSpatialServices.Instance.GeometryFromText("POINT (4 3)");
        case PrimitiveTypeKind.Geography:
          return (object) DefaultSpatialServices.Instance.GeographyFromText("POINT (4 3)");
        default:
          return (object) null;
      }
    }

    private static void InstantiateComplexProperties(
      object structuralObject,
      IEnumerable<EdmProperty> properties)
    {
      foreach (EdmProperty property in properties)
      {
        if (property.IsComplexType)
        {
          PropertyInfo clrPropertyInfo = property.GetClrPropertyInfo();
          object instance = Activator.CreateInstance(clrPropertyInfo.PropertyType);
          ModificationCommandTreeGenerator.InstantiateComplexProperties(instance, (IEnumerable<EdmProperty>) property.ComplexType.Properties);
          clrPropertyInfo.GetPropertyInfoForSet().SetValue(structuralObject, instance, (object[]) null);
        }
      }
    }

    private class TempDbContext : DbContext
    {
      [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
      public TempDbContext(DbCompiledModel model)
        : base(model)
      {
        this.InternalContext.InitializerDisabled = true;
      }

      [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
      public TempDbContext(DbConnection connection, DbCompiledModel model)
        : base(connection, model, false)
      {
        this.InternalContext.InitializerDisabled = true;
      }
    }
  }
}
