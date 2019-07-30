// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.PropertyConstraintException
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace System.Data.Entity.Core
{
  /// <summary>
  /// Property constraint exception class. Note that this class has state - so if you change even
  /// its internals, it can be a breaking change
  /// </summary>
  [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "SerializeObjectState used instead")]
  [Serializable]
  public sealed class PropertyConstraintException : ConstraintException
  {
    [NonSerialized]
    private PropertyConstraintException.PropertyConstraintExceptionState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.PropertyConstraintException" /> class with default message.
    /// </summary>
    public PropertyConstraintException()
    {
      this.SubscribeToSerializeObjectState();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.PropertyConstraintException" /> class with supplied message.
    /// </summary>
    /// <param name="message">A localized error message.</param>
    public PropertyConstraintException(string message)
      : base(message)
    {
      this.SubscribeToSerializeObjectState();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.PropertyConstraintException" /> class with supplied message and inner exception.
    /// </summary>
    /// <param name="message">A localized error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PropertyConstraintException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.SubscribeToSerializeObjectState();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.PropertyConstraintException" /> class.
    /// </summary>
    /// <param name="message">A localized error message.</param>
    /// <param name="propertyName">The name of the property.</param>
    public PropertyConstraintException(string message, string propertyName)
      : base(message)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      this._state.PropertyName = propertyName;
      this.SubscribeToSerializeObjectState();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Core.PropertyConstraintException" /> class.
    /// </summary>
    /// <param name="message">A localized error message.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="innerException">The inner exception.</param>
    public PropertyConstraintException(
      string message,
      string propertyName,
      Exception innerException)
      : base(message, innerException)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      this._state.PropertyName = propertyName;
      this.SubscribeToSerializeObjectState();
    }

    /// <summary>Gets the name of the property that violated the constraint.</summary>
    /// <returns>The name of the property that violated the constraint.</returns>
    public string PropertyName
    {
      get
      {
        return this._state.PropertyName;
      }
    }

    private void SubscribeToSerializeObjectState()
    {
      this.SerializeObjectState += (EventHandler<SafeSerializationEventArgs>) ((_, a) => a.AddSerializedState((ISafeSerializationData) this._state));
    }

    [Serializable]
    private struct PropertyConstraintExceptionState : ISafeSerializationData
    {
      public string PropertyName { get; set; }

      public void CompleteDeserialization(object deserialized)
      {
        PropertyConstraintException constraintException = (PropertyConstraintException) deserialized;
        constraintException._state = this;
        constraintException.SubscribeToSerializeObjectState();
      }
    }
  }
}
