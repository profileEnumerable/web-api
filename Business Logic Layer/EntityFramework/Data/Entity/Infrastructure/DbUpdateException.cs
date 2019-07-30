// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbUpdateException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Internal;
using System.Data.Entity.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Exception thrown by <see cref="T:System.Data.Entity.DbContext" /> when the saving of changes to the database fails.
  /// Note that state entries referenced by this exception are not serialized due to security and accesses to the
  /// state entries after serialization will return null.
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "SerializeObjectState used instead")]
  [Serializable]
  public class DbUpdateException : DataException
  {
    [NonSerialized]
    private readonly InternalContext _internalContext;
    [NonSerialized]
    private DbUpdateException.DbUpdateExceptionState _state;

    internal DbUpdateException(
      InternalContext internalContext,
      UpdateException innerException,
      bool involvesIndependentAssociations)
      : base(involvesIndependentAssociations ? Strings.DbContext_IndependentAssociationUpdateException : innerException.Message, (Exception) innerException)
    {
      this._internalContext = internalContext;
      this._state.InvolvesIndependentAssociations = involvesIndependentAssociations;
      this.SubscribeToSerializeObjectState();
    }

    /// <summary>
    /// Gets <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry" /> objects that represents the entities that could not
    /// be saved to the database.
    /// </summary>
    /// <returns> The entries representing the entities that could not be saved. </returns>
    public IEnumerable<DbEntityEntry> Entries
    {
      get
      {
        UpdateException innerException = this.InnerException as UpdateException;
        if (this._state.InvolvesIndependentAssociations || this._internalContext == null || (innerException == null || innerException.StateEntries == null))
          return Enumerable.Empty<DbEntityEntry>();
        return innerException.StateEntries.Select<ObjectStateEntry, DbEntityEntry>((Func<ObjectStateEntry, DbEntityEntry>) (e => new DbEntityEntry(new InternalEntityEntry(this._internalContext, (System.Data.Entity.Internal.IEntityStateEntry) new StateEntryAdapter(e)))));
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.DbUpdateException" /> class.
    /// </summary>
    public DbUpdateException()
    {
      this.SubscribeToSerializeObjectState();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.DbUpdateException" /> class.
    /// </summary>
    /// <param name="message"> The message. </param>
    public DbUpdateException(string message)
      : base(message)
    {
      this.SubscribeToSerializeObjectState();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.DbUpdateException" /> class.
    /// </summary>
    /// <param name="message"> The message. </param>
    /// <param name="innerException"> The inner exception. </param>
    public DbUpdateException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.SubscribeToSerializeObjectState();
    }

    private void SubscribeToSerializeObjectState()
    {
      this.SerializeObjectState += (EventHandler<SafeSerializationEventArgs>) ((exception, eventArgs) => eventArgs.AddSerializedState((ISafeSerializationData) this._state));
    }

    [Serializable]
    private struct DbUpdateExceptionState : ISafeSerializationData
    {
      public bool InvolvesIndependentAssociations { get; set; }

      public void CompleteDeserialization(object deserialized)
      {
        DbUpdateException dbUpdateException = (DbUpdateException) deserialized;
        dbUpdateException._state = this;
        dbUpdateException.SubscribeToSerializeObjectState();
      }
    }
  }
}
