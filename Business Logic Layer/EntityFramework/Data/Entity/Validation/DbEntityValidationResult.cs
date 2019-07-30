// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Validation.DbEntityValidationResult
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Internal;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Validation
{
  /// <summary>Represents validation results for single entity.</summary>
  [Serializable]
  public class DbEntityValidationResult
  {
    [NonSerialized]
    private readonly InternalEntityEntry _entry;
    private readonly List<DbValidationError> _validationErrors;

    /// <summary>
    /// Creates an instance of <see cref="T:System.Data.Entity.Validation.DbEntityValidationResult" /> class.
    /// </summary>
    /// <param name="entry"> Entity entry the results applies to. Never null. </param>
    /// <param name="validationErrors">
    /// List of <see cref="T:System.Data.Entity.Validation.DbValidationError" /> instances. Never null. Can be empty meaning the entity is valid.
    /// </param>
    public DbEntityValidationResult(
      DbEntityEntry entry,
      IEnumerable<DbValidationError> validationErrors)
    {
      Check.NotNull<DbEntityEntry>(entry, nameof (entry));
      Check.NotNull<IEnumerable<DbValidationError>>(validationErrors, nameof (validationErrors));
      this._entry = entry.InternalEntry;
      this._validationErrors = validationErrors.ToList<DbValidationError>();
    }

    internal DbEntityValidationResult(
      InternalEntityEntry entry,
      IEnumerable<DbValidationError> validationErrors)
    {
      this._entry = entry;
      this._validationErrors = validationErrors.ToList<DbValidationError>();
    }

    /// <summary>
    /// Gets an instance of <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry" /> the results applies to.
    /// </summary>
    public DbEntityEntry Entry
    {
      get
      {
        if (this._entry == null)
          return (DbEntityEntry) null;
        return new DbEntityEntry(this._entry);
      }
    }

    /// <summary>Gets validation errors. Never null.</summary>
    public ICollection<DbValidationError> ValidationErrors
    {
      get
      {
        return (ICollection<DbValidationError>) this._validationErrors;
      }
    }

    /// <summary>Gets an indicator if the entity is valid.</summary>
    public bool IsValid
    {
      get
      {
        return !this._validationErrors.Any<DbValidationError>();
      }
    }
  }
}
