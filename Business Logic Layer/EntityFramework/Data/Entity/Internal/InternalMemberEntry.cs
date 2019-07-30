// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.InternalMemberEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Internal.Validation;
using System.Data.Entity.Validation;
using System.Linq;

namespace System.Data.Entity.Internal
{
  internal abstract class InternalMemberEntry
  {
    private readonly InternalEntityEntry _internalEntityEntry;
    private readonly MemberEntryMetadata _memberMetadata;

    protected InternalMemberEntry(
      InternalEntityEntry internalEntityEntry,
      MemberEntryMetadata memberMetadata)
    {
      this._internalEntityEntry = internalEntityEntry;
      this._memberMetadata = memberMetadata;
    }

    public virtual string Name
    {
      get
      {
        return this._memberMetadata.MemberName;
      }
    }

    public abstract object CurrentValue { get; set; }

    public virtual InternalEntityEntry InternalEntityEntry
    {
      get
      {
        return this._internalEntityEntry;
      }
    }

    public virtual MemberEntryMetadata EntryMetadata
    {
      get
      {
        return this._memberMetadata;
      }
    }

    public virtual IEnumerable<DbValidationError> GetValidationErrors()
    {
      ValidationProvider validationProvider = this.InternalEntityEntry.InternalContext.ValidationProvider;
      PropertyValidator propertyValidator = validationProvider.GetPropertyValidator(this._internalEntityEntry, this);
      if (propertyValidator == null)
        return Enumerable.Empty<DbValidationError>();
      return propertyValidator.Validate(validationProvider.GetEntityValidationContext(this._internalEntityEntry, (IDictionary<object, object>) null), this);
    }

    public abstract DbMemberEntry CreateDbMemberEntry();

    public abstract DbMemberEntry<TEntity, TProperty> CreateDbMemberEntry<TEntity, TProperty>() where TEntity : class;
  }
}
