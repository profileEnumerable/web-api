// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Validation.DbEntityValidationException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace System.Data.Entity.Validation
{
  /// <summary>
  /// Exception thrown from <see cref="M:System.Data.Entity.DbContext.SaveChanges" /> when validating entities fails.
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "SerializeObjectState used instead")]
  [Serializable]
  public class DbEntityValidationException : DataException
  {
    [NonSerialized]
    private DbEntityValidationException.DbEntityValidationExceptionState _state = new DbEntityValidationException.DbEntityValidationExceptionState();

    /// <summary>
    /// Initializes a new instance of DbEntityValidationException.
    /// </summary>
    public DbEntityValidationException()
      : this(Strings.DbEntityValidationException_ValidationFailed)
    {
    }

    /// <summary>
    /// Initializes a new instance of DbEntityValidationException.
    /// </summary>
    /// <param name="message"> The exception message. </param>
    public DbEntityValidationException(string message)
      : this(message, Enumerable.Empty<DbEntityValidationResult>())
    {
    }

    /// <summary>
    /// Initializes a new instance of DbEntityValidationException.
    /// </summary>
    /// <param name="message"> The exception message. </param>
    /// <param name="entityValidationResults"> Validation results. </param>
    public DbEntityValidationException(
      string message,
      IEnumerable<DbEntityValidationResult> entityValidationResults)
      : base(message)
    {
      Check.NotNull<IEnumerable<DbEntityValidationResult>>(entityValidationResults, nameof (entityValidationResults));
      this._state.InititializeValidationResults(entityValidationResults);
      this.SubscribeToSerializeObjectState();
    }

    /// <summary>
    /// Initializes a new instance of DbEntityValidationException.
    /// </summary>
    /// <param name="message"> The exception message. </param>
    /// <param name="innerException"> The inner exception. </param>
    public DbEntityValidationException(string message, Exception innerException)
      : this(message, Enumerable.Empty<DbEntityValidationResult>(), innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of DbEntityValidationException.
    /// </summary>
    /// <param name="message"> The exception message. </param>
    /// <param name="entityValidationResults"> Validation results. </param>
    /// <param name="innerException"> The inner exception. </param>
    public DbEntityValidationException(
      string message,
      IEnumerable<DbEntityValidationResult> entityValidationResults,
      Exception innerException)
      : base(message, innerException)
    {
      Check.NotNull<IEnumerable<DbEntityValidationResult>>(entityValidationResults, nameof (entityValidationResults));
      this._state.InititializeValidationResults(entityValidationResults);
      this.SubscribeToSerializeObjectState();
    }

    /// <summary>Validation results.</summary>
    public IEnumerable<DbEntityValidationResult> EntityValidationErrors
    {
      get
      {
        return this._state.EntityValidationErrors;
      }
    }

    private void SubscribeToSerializeObjectState()
    {
      this.SerializeObjectState += (EventHandler<SafeSerializationEventArgs>) ((exception, eventArgs) => eventArgs.AddSerializedState((ISafeSerializationData) this._state));
    }

    [Serializable]
    private class DbEntityValidationExceptionState : ISafeSerializationData
    {
      private IList<DbEntityValidationResult> _entityValidationResults;

      internal void InititializeValidationResults(
        IEnumerable<DbEntityValidationResult> entityValidationResults)
      {
        this._entityValidationResults = entityValidationResults == null ? (IList<DbEntityValidationResult>) new List<DbEntityValidationResult>() : (IList<DbEntityValidationResult>) entityValidationResults.ToList<DbEntityValidationResult>();
      }

      public IEnumerable<DbEntityValidationResult> EntityValidationErrors
      {
        get
        {
          return (IEnumerable<DbEntityValidationResult>) this._entityValidationResults;
        }
      }

      public void CompleteDeserialization(object deserialized)
      {
        DbEntityValidationException validationException = (DbEntityValidationException) deserialized;
        validationException._state = this;
        validationException.SubscribeToSerializeObjectState();
      }
    }
  }
}
