// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InternalCollectionEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Internal
{
  internal class InternalCollectionEntry : InternalNavigationEntry
  {
    private static readonly ConcurrentDictionary<Type, Func<InternalCollectionEntry, object>> _entryFactories = new ConcurrentDictionary<Type, Func<InternalCollectionEntry, object>>();

    public InternalCollectionEntry(
      InternalEntityEntry internalEntityEntry,
      NavigationEntryMetadata navigationMetadata)
      : base(internalEntityEntry, navigationMetadata)
    {
    }

    protected override object GetNavigationPropertyFromRelatedEnd(object entity)
    {
      return (object) this.RelatedEnd;
    }

    public override object CurrentValue
    {
      get
      {
        return base.CurrentValue;
      }
      set
      {
        if (this.Setter != null)
          this.Setter(this.InternalEntityEntry.Entity, value);
        else if (this.InternalEntityEntry.IsDetached || !object.ReferenceEquals((object) this.RelatedEnd, value))
          throw Error.DbCollectionEntry_CannotSetCollectionProp((object) this.Name, (object) this.InternalEntityEntry.Entity.GetType().ToString());
      }
    }

    public override DbMemberEntry CreateDbMemberEntry()
    {
      return (DbMemberEntry) new DbCollectionEntry(this);
    }

    public override DbMemberEntry<TEntity, TProperty> CreateDbMemberEntry<TEntity, TProperty>()
    {
      return this.CreateDbCollectionEntry<TEntity, TProperty>(this.EntryMetadata.ElementType);
    }

    public virtual DbCollectionEntry<TEntity, TElement> CreateDbCollectionEntry<TEntity, TElement>() where TEntity : class
    {
      return new DbCollectionEntry<TEntity, TElement>(this);
    }

    private DbMemberEntry<TEntity, TProperty> CreateDbCollectionEntry<TEntity, TProperty>(
      Type elementType)
      where TEntity : class
    {
      Type key = typeof (DbMemberEntry<TEntity, TProperty>);
      Func<InternalCollectionEntry, object> func;
      if (!InternalCollectionEntry._entryFactories.TryGetValue(key, out func))
      {
        Type type = typeof (DbCollectionEntry<,>).MakeGenericType(typeof (TEntity), elementType);
        if (!key.IsAssignableFrom(type))
          throw Error.DbEntityEntry_WrongGenericForCollectionNavProp((object) typeof (TProperty), (object) this.Name, (object) this.EntryMetadata.DeclaringType, (object) typeof (ICollection<>).MakeGenericType(elementType));
        func = (Func<InternalCollectionEntry, object>) Delegate.CreateDelegate(typeof (Func<InternalCollectionEntry, object>), type.GetDeclaredMethod("Create", typeof (InternalCollectionEntry)));
        InternalCollectionEntry._entryFactories.TryAdd(key, func);
      }
      return (DbMemberEntry<TEntity, TProperty>) func(this);
    }
  }
}
