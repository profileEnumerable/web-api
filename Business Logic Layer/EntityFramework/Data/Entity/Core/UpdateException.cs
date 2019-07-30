// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.UpdateException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Objects;
using System.Runtime.Serialization;

namespace System.Data.Entity.Core
{
  /// <summary>Exception during save changes to store</summary>
  [Serializable]
  public class UpdateException : DataException
  {
    [NonSerialized]
    private readonly ReadOnlyCollection<ObjectStateEntry> _stateEntries;

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Core.UpdateException" />.
    /// </summary>
    public UpdateException()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Core.UpdateException" /> with a specialized error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UpdateException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.UpdateException" /> class that uses a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception. </param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public UpdateException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.UpdateException" /> class that uses a specified error message, a reference to the inner exception, and an enumerable collection of
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// objects.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception. </param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    /// <param name="stateEntries">
    /// The collection of <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> objects.
    /// </param>
    public UpdateException(
      string message,
      Exception innerException,
      IEnumerable<ObjectStateEntry> stateEntries)
      : base(message, innerException)
    {
      this._stateEntries = new ReadOnlyCollection<ObjectStateEntry>((IList<ObjectStateEntry>) new List<ObjectStateEntry>(stateEntries));
    }

    /// <summary>
    /// Gets the <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> objects for this
    /// <see cref="T:System.Data.Entity.Core.UpdateException" />
    /// .
    /// </summary>
    /// <returns>
    /// A collection of <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" /> objects comprised of either a single entity and 0 or more relationships, or 0 entities and 1 or more relationships.
    /// </returns>
    public ReadOnlyCollection<ObjectStateEntry> StateEntries
    {
      get
      {
        return this._stateEntries;
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Core.UpdateException" /> with serialized data.
    /// </summary>
    /// <param name="info">
    /// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
    /// </param>
    /// <param name="context">
    /// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.
    /// </param>
    protected UpdateException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
