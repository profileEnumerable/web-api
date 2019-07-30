// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InternalReferenceEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.Internal
{
  internal class InternalReferenceEntry : InternalNavigationEntry
  {
    private static readonly ConcurrentDictionary<Type, Action<IRelatedEnd, object>> _entityReferenceValueSetters = new ConcurrentDictionary<Type, Action<IRelatedEnd, object>>();
    public static readonly MethodInfo SetValueOnEntityReferenceMethod = typeof (InternalReferenceEntry).GetOnlyDeclaredMethod("SetValueOnEntityReference");

    public InternalReferenceEntry(
      InternalEntityEntry internalEntityEntry,
      NavigationEntryMetadata navigationMetadata)
      : base(internalEntityEntry, navigationMetadata)
    {
    }

    protected override object GetNavigationPropertyFromRelatedEnd(object entity)
    {
      IEnumerator enumerator = this.RelatedEnd.GetEnumerator();
      if (!enumerator.MoveNext())
        return (object) null;
      return enumerator.Current;
    }

    protected virtual void SetNavigationPropertyOnRelatedEnd(object value)
    {
      Type type = this.RelatedEnd.GetType();
      Action<IRelatedEnd, object> action;
      if (!InternalReferenceEntry._entityReferenceValueSetters.TryGetValue(type, out action))
      {
        action = (Action<IRelatedEnd, object>) Delegate.CreateDelegate(typeof (Action<IRelatedEnd, object>), InternalReferenceEntry.SetValueOnEntityReferenceMethod.MakeGenericMethod(((IEnumerable<Type>) type.GetGenericArguments()).Single<Type>()));
        InternalReferenceEntry._entityReferenceValueSetters.TryAdd(type, action);
      }
      action(this.RelatedEnd, value);
    }

    private static void SetValueOnEntityReference<TRelatedEntity>(
      IRelatedEnd entityReference,
      object value)
      where TRelatedEntity : class
    {
      ((EntityReference<TRelatedEntity>) entityReference).Value = (TRelatedEntity) value;
    }

    public override object CurrentValue
    {
      get
      {
        return base.CurrentValue;
      }
      set
      {
        if (this.RelatedEnd != null && this.InternalEntityEntry.State != EntityState.Deleted)
        {
          this.SetNavigationPropertyOnRelatedEnd(value);
        }
        else
        {
          if (this.Setter == null)
            throw Error.DbPropertyEntry_SettingEntityRefNotSupported((object) this.Name, (object) this.InternalEntityEntry.EntityType.Name, (object) this.InternalEntityEntry.State);
          this.Setter(this.InternalEntityEntry.Entity, value);
        }
      }
    }

    public override DbMemberEntry CreateDbMemberEntry()
    {
      return (DbMemberEntry) new DbReferenceEntry(this);
    }

    public override DbMemberEntry<TEntity, TProperty> CreateDbMemberEntry<TEntity, TProperty>()
    {
      return (DbMemberEntry<TEntity, TProperty>) new DbReferenceEntry<TEntity, TProperty>(this);
    }
  }
}
