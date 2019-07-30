// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DataClasses.StructuralObject
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Core.Common.Utils;
using System.Data.Entity.Resources;
using System.Data.Entity.Spatial;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace System.Data.Entity.Core.Objects.DataClasses
{
  /// <summary>
  /// This class contains the common methods need for an date object.
  /// </summary>
  [DataContract(IsReference = true)]
  [Serializable]
  public abstract class StructuralObject : INotifyPropertyChanging, INotifyPropertyChanged
  {
    /// <summary>
    /// Public constant name used for change tracking
    /// Providing this definition allows users to use this constant instead of
    /// hard-coding the string. This helps to ensure the property name is correct
    /// and allows faster comparisons in places where we are looking for this specific string.
    /// Users can still use the case-sensitive string directly instead of the constant,
    /// it will just be slightly slower on comparison.
    /// Including the dash (-) character around the name ensures that this will not conflict with
    /// a real data property, because -EntityKey- is not a valid identifier name
    /// </summary>
    public const string EntityKeyPropertyName = "-EntityKey-";

    /// <summary>Notification that a property has been changed.</summary>
    /// <remarks>
    /// The PropertyChanged event can indicate all properties on the
    /// object have changed by using either a null reference
    /// (Nothing in Visual Basic) or String.Empty as the property name
    /// in the PropertyChangedEventArgs.
    /// </remarks>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>Notification that a property is about to be changed.</summary>
    /// <remarks>
    /// The PropertyChanging event can indicate all properties on the
    /// object are changing by using either a null reference
    /// (Nothing in Visual Basic) or String.Empty as the property name
    /// in the PropertyChangingEventArgs.
    /// </remarks>
    public event PropertyChangingEventHandler PropertyChanging;

    /// <summary>
    /// Raises the <see cref="E:System.Data.Entity.Core.Objects.DataClasses.StructuralObject.PropertyChanged" /> event.
    /// </summary>
    /// <param name="property">The name of the changed property.</param>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property")]
    protected virtual void OnPropertyChanged(string property)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(property));
    }

    /// <summary>
    /// Raises the <see cref="E:System.Data.Entity.Core.Objects.DataClasses.StructuralObject.PropertyChanging" /> event.
    /// </summary>
    /// <param name="property">The name of the property changing.</param>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property")]
    protected virtual void OnPropertyChanging(string property)
    {
      if (this.PropertyChanging == null)
        return;
      this.PropertyChanging((object) this, new PropertyChangingEventArgs(property));
    }

    /// <summary>Returns the minimum date time value supported by the data source.</summary>
    /// <returns>
    /// A <see cref="T:System.DateTime" /> value that is the minimum date time that is supported by the data source.
    /// </returns>
    protected static DateTime DefaultDateTimeValue()
    {
      return DateTime.Now;
    }

    /// <summary>Raises an event that is used to report that a property change is pending.</summary>
    /// <param name="property">The name of the changing property.</param>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property")]
    protected virtual void ReportPropertyChanging(string property)
    {
      Check.NotEmpty(property, nameof (property));
      this.OnPropertyChanging(property);
    }

    /// <summary>Raises an event that is used to report that a property change has occurred.</summary>
    /// <param name="property">The name for the changed property.</param>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Property")]
    protected virtual void ReportPropertyChanged(string property)
    {
      Check.NotEmpty(property, nameof (property));
      this.OnPropertyChanged(property);
    }

    /// <summary>Returns a complex type for the specified property.</summary>
    /// <remarks>
    /// Unlike most of the other helper methods in this class, this one is not static
    /// because it references the SetValidValue for complex objects, which is also not static
    /// because it needs a reference to this.
    /// </remarks>
    /// <returns>A complex type object for the property.</returns>
    /// <param name="currentValue">A complex object that inherits from complex object.</param>
    /// <param name="property">The name of the complex property that is the complex object.</param>
    /// <param name="isNullable">Indicates whether the type supports null values.</param>
    /// <param name="isInitialized">Indicates whether the type is initialized.</param>
    /// <typeparam name="T">The type of the complex object being requested.</typeparam>
    protected internal T GetValidValue<T>(
      T currentValue,
      string property,
      bool isNullable,
      bool isInitialized)
      where T : ComplexObject, new()
    {
      if (!isNullable && !isInitialized)
        currentValue = this.SetValidValue<T>(currentValue, new T(), property);
      return currentValue;
    }

    internal abstract void ReportComplexPropertyChanging(
      string entityMemberName,
      ComplexObject complexObject,
      string complexMemberName);

    internal abstract void ReportComplexPropertyChanged(
      string entityMemberName,
      ComplexObject complexObject,
      string complexMemberName);

    internal abstract bool IsChangeTracked { get; }

    /// <summary>Determines whether the specified byte arrays contain identical values.</summary>
    /// <returns>true if both arrays are of the same length and contain the same byte values or if both arrays are null; otherwise, false.</returns>
    /// <param name="first">The first byte array value to compare.</param>
    /// <param name="second">The second byte array to compare.</param>
    protected internal static bool BinaryEquals(byte[] first, byte[] second)
    {
      if (object.ReferenceEquals((object) first, (object) second))
        return true;
      if (first == null || second == null)
        return false;
      return ByValueEqualityComparer.CompareBinaryValues(first, second);
    }

    /// <summary>Returns a copy of the current byte value.</summary>
    /// <returns>
    /// A copy of the current <see cref="T:System.Byte" /> value.
    /// </returns>
    /// <param name="currentValue">The current byte array value.</param>
    protected internal static byte[] GetValidValue(byte[] currentValue)
    {
      if (currentValue == null)
        return (byte[]) null;
      return (byte[]) currentValue.Clone();
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Byte[]" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Byte" /> value being validated.
    /// </returns>
    /// <param name="value">The value passed into the property setter.</param>
    /// <param name="isNullable">Flag indicating if this property is allowed to be null.</param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    /// <exception cref="T:System.Data.ConstraintException">If value is null for a non nullable value.</exception>
    protected internal static byte[] SetValidValue(
      byte[] value,
      bool isNullable,
      string propertyName)
    {
      if (value != null)
        return (byte[]) value.Clone();
      if (!isNullable)
        EntityUtil.ThrowPropertyIsNotNullable(propertyName);
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Byte[]" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Byte" /> value being set.
    /// </returns>
    /// <param name="value">The value being set.</param>
    /// <param name="isNullable">Indicates whether the property is nullable.</param>
    protected internal static byte[] SetValidValue(byte[] value, bool isNullable)
    {
      return StructuralObject.SetValidValue(value, isNullable, (string) null);
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Boolean" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Boolean" /> value being set.
    /// </returns>
    /// <param name="value">The Boolean value.</param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static bool SetValidValue(bool value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Boolean" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Boolean" /> value being set.
    /// </returns>
    /// <param name="value">The Boolean value.</param>
    protected internal static bool SetValidValue(bool value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Boolean" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Boolean" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Boolean" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static bool? SetValidValue(bool? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Boolean" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Boolean" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Boolean" /> value.
    /// </param>
    protected internal static bool? SetValidValue(bool? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Byte" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Byte" /> that is set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Byte" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static byte SetValidValue(byte value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Byte" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Byte" /> value that is set.
    /// </returns>
    /// <param name="value">The value that is being validated.</param>
    protected internal static byte SetValidValue(byte value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Byte" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Byte" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Byte" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static byte? SetValidValue(byte? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Byte" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Byte" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Byte" /> value.
    /// </param>
    protected internal static byte? SetValidValue(byte? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.SByte" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.SByte" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.SByte" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    [CLSCompliant(false)]
    protected internal static sbyte SetValidValue(sbyte value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.SByte" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.SByte" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.SByte" /> value.
    /// </param>
    [CLSCompliant(false)]
    protected internal static sbyte SetValidValue(sbyte value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.SByte" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.SByte" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.SByte" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    [CLSCompliant(false)]
    protected internal static sbyte? SetValidValue(sbyte? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.SByte" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.SByte" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.SByte" /> value.
    /// </param>
    [CLSCompliant(false)]
    protected internal static sbyte? SetValidValue(sbyte? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.DateTime" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.DateTime" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.DateTime" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static DateTime SetValidValue(DateTime value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.DateTime" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.DateTime" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.DateTime" /> value.
    /// </param>
    protected internal static DateTime SetValidValue(DateTime value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.DateTime" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.DateTime" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.DateTime" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static DateTime? SetValidValue(DateTime? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.DateTime" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.DateTime" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.DateTime" /> value.
    /// </param>
    protected internal static DateTime? SetValidValue(DateTime? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.TimeSpan" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.TimeSpan" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.TimeSpan" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static TimeSpan SetValidValue(TimeSpan value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.TimeSpan" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.TimeSpan" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.TimeSpan" /> value.
    /// </param>
    protected internal static TimeSpan SetValidValue(TimeSpan value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.TimeSpan" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.TimeSpan" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.TimeSpan" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static TimeSpan? SetValidValue(TimeSpan? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.TimeSpan" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.TimeSpan" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.TimeSpan" /> value.
    /// </param>
    protected internal static TimeSpan? SetValidValue(TimeSpan? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.DateTimeOffset" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.DateTimeOffset" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.DateTimeOffset" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static DateTimeOffset SetValidValue(
      DateTimeOffset value,
      string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.DateTimeOffset" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.DateTimeOffset" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.DateTimeOffset" /> value.
    /// </param>
    protected internal static DateTimeOffset SetValidValue(DateTimeOffset value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.DateTimeOffset" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.DateTimeOffset" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.DateTimeOffset" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static DateTimeOffset? SetValidValue(
      DateTimeOffset? value,
      string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.DateTimeOffset" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.DateTimeOffset" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.DateTimeOffset" /> value.
    /// </param>
    protected internal static DateTimeOffset? SetValidValue(DateTimeOffset? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Decimal" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Decimal" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Decimal" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static Decimal SetValidValue(Decimal value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Decimal" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Decimal" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Decimal" /> value.
    /// </param>
    protected internal static Decimal SetValidValue(Decimal value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Decimal" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Decimal" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Decimal" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static Decimal? SetValidValue(Decimal? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Decimal" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Decimal" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Decimal" /> value.
    /// </param>
    protected internal static Decimal? SetValidValue(Decimal? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Double" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Double" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Double" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static double SetValidValue(double value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Double" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Double" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Double" /> value.
    /// </param>
    protected internal static double SetValidValue(double value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Double" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Double" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Double" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static double? SetValidValue(double? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Double" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Double" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Double" /> value.
    /// </param>
    protected internal static double? SetValidValue(double? value)
    {
      return value;
    }

    /// <summary>Makes sure the Single value being set for a property is valid.</summary>
    /// <returns>
    /// The <see cref="T:System.Single" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Single" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static float SetValidValue(float value, string propertyName)
    {
      return value;
    }

    /// <summary>Makes sure the Single value being set for a property is valid.</summary>
    /// <returns>
    /// The <see cref="T:System.Single" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Single" /> value.
    /// </param>
    protected internal static float SetValidValue(float value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Single" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Single" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Single" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static float? SetValidValue(float? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Single" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Single" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Single" /> value.
    /// </param>
    protected internal static float? SetValidValue(float? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Guid" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Guid" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Guid" /> value.
    /// </param>
    /// <param name="propertyName">Name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static Guid SetValidValue(Guid value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Guid" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Guid" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Guid" /> value.
    /// </param>
    protected internal static Guid SetValidValue(Guid value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Guid" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Guid" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Guid" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static Guid? SetValidValue(Guid? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Guid" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Guid" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Guid" /> value.
    /// </param>
    protected internal static Guid? SetValidValue(Guid? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int16" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Int16" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Int16" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static short SetValidValue(short value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int16" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Int16" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Int16" /> value.
    /// </param>
    protected internal static short SetValidValue(short value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int16" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Int16" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Int16" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static short? SetValidValue(short? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int16" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Int16" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Int16" /> value.
    /// </param>
    protected internal static short? SetValidValue(short? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int32" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Int32" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Int32" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static int SetValidValue(int value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int32" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Int32" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Int32" /> value.
    /// </param>
    protected internal static int SetValidValue(int value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int32" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Int32" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Int32" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static int? SetValidValue(int? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int32" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Int32" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Int32" /> value.
    /// </param>
    protected internal static int? SetValidValue(int? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int64" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Int64" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Int64" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static long SetValidValue(long value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int64" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Int64" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Int64" /> value.
    /// </param>
    protected internal static long SetValidValue(long value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int64" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Int64" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Int64" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static long? SetValidValue(long? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.Int64" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The nullable <see cref="T:System.Int64" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The nullable <see cref="T:System.Int64" /> value.
    /// </param>
    protected internal static long? SetValidValue(long? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.UInt16" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.UInt16" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.UInt16" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    [CLSCompliant(false)]
    protected internal static ushort SetValidValue(ushort value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.UInt16" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.UInt16" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.UInt16" /> value.
    /// </param>
    [CLSCompliant(false)]
    protected internal static ushort SetValidValue(ushort value)
    {
      return value;
    }

    /// <summary>Makes sure the UInt16 value being set for a property is valid.</summary>
    /// <returns>The nullable UInt16 value being set.</returns>
    /// <param name="value">The nullable UInt16 value.</param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    protected internal static ushort? SetValidValue(ushort? value, string propertyName)
    {
      return value;
    }

    /// <summary>Makes sure the UInt16 value being set for a property is valid.</summary>
    /// <returns>The nullable UInt16 value being set.</returns>
    /// <param name="value">The nullable UInt16 value.</param>
    [CLSCompliant(false)]
    protected internal static ushort? SetValidValue(ushort? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.UInt32" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.UInt32" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.UInt32" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    [CLSCompliant(false)]
    protected internal static uint SetValidValue(uint value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.UInt32" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.UInt32" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.UInt32" /> value.
    /// </param>
    [CLSCompliant(false)]
    protected internal static uint SetValidValue(uint value)
    {
      return value;
    }

    /// <summary>Makes sure the UInt32 value being set for a property is valid.</summary>
    /// <returns>The nullable UInt32 value being set.</returns>
    /// <param name="value">The nullable UInt32 value.</param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    [CLSCompliant(false)]
    protected internal static uint? SetValidValue(uint? value, string propertyName)
    {
      return value;
    }

    /// <summary>Makes sure the UInt32 value being set for a property is valid.</summary>
    /// <returns>The nullable UInt32 value being set.</returns>
    /// <param name="value">The nullable UInt32 value.</param>
    [CLSCompliant(false)]
    protected internal static uint? SetValidValue(uint? value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.UInt64" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.UInt64" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.UInt64" /> value.
    /// </param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    [CLSCompliant(false)]
    protected internal static ulong SetValidValue(ulong value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.UInt64" /> value being set for a property is valid.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.UInt64" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.UInt64" /> value.
    /// </param>
    [CLSCompliant(false)]
    protected internal static ulong SetValidValue(ulong value)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.UInt64" /> value being set for a property is valid.
    /// </summary>
    /// <returns>The nullable UInt64 value being set.</returns>
    /// <param name="value">The nullable UInt64 value.</param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyName")]
    [CLSCompliant(false)]
    protected internal static ulong? SetValidValue(ulong? value, string propertyName)
    {
      return value;
    }

    /// <summary>
    /// Makes sure the <see cref="T:System.UInt64" /> value being set for a property is valid.
    /// </summary>
    /// <returns>The nullable UInt64 value being set.</returns>
    /// <param name="value">The nullable UInt64 value.</param>
    [CLSCompliant(false)]
    protected internal static ulong? SetValidValue(ulong? value)
    {
      return value;
    }

    /// <summary>Validates that the property is not null, and throws if it is.</summary>
    /// <returns>The validated property.</returns>
    /// <param name="value">The string value to be checked.</param>
    /// <param name="isNullable">Flag indicating if this property is allowed to be null.</param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    /// <exception cref="T:System.Data.ConstraintException">The string value is null for a non-nullable string.</exception>
    protected internal static string SetValidValue(
      string value,
      bool isNullable,
      string propertyName)
    {
      if (value == null && !isNullable)
        EntityUtil.ThrowPropertyIsNotNullable(propertyName);
      return value;
    }

    /// <summary>Validates that the property is not null, and throws if it is.</summary>
    /// <returns>
    /// The validated <see cref="T:System.String" /> value.
    /// </returns>
    /// <param name="value">The string value to be checked.</param>
    /// <param name="isNullable">Flag indicating if this property is allowed to be null.</param>
    protected internal static string SetValidValue(string value, bool isNullable)
    {
      return StructuralObject.SetValidValue(value, isNullable, (string) null);
    }

    /// <summary>Validates that the property is not null, and throws if it is.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Spatial.DbGeography" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Data.Entity.Spatial.DbGeography" /> value to be checked.
    /// </param>
    /// <param name="isNullable">Flag indicating if this property is allowed to be null.</param>
    /// <param name="propertyName">Name of the property that is being validated.</param>
    /// <exception cref="T:System.Data.ConstraintException">The value is null for a non-nullable property.</exception>
    protected internal static DbGeography SetValidValue(
      DbGeography value,
      bool isNullable,
      string propertyName)
    {
      if (value == null && !isNullable)
        EntityUtil.ThrowPropertyIsNotNullable(propertyName);
      return value;
    }

    /// <summary>Validates that the property is not null, and throws if it is.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Spatial.DbGeography" /> value being set.
    /// </returns>
    /// <param name="value">
    /// <see cref="T:System.Data.Entity.Spatial.DbGeography" /> value to be checked.
    /// </param>
    /// <param name="isNullable">Flag indicating if this property is allowed to be null.</param>
    /// <exception cref="T:System.Data.ConstraintException">The value is null for a non-nullable property.</exception>
    protected internal static DbGeography SetValidValue(
      DbGeography value,
      bool isNullable)
    {
      return StructuralObject.SetValidValue(value, isNullable, (string) null);
    }

    /// <summary>Validates that the property is not null, and throws if it is.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Spatial.DbGeometry" /> value being set.
    /// </returns>
    /// <param name="value">
    /// <see cref="T:System.Data.Entity.Spatial.DbGeometry" /> value to be checked.
    /// </param>
    /// <param name="isNullable">Flag indicating if this property is allowed to be null.</param>
    /// <param name="propertyName">The name of the property that is being validated.</param>
    /// <exception cref="T:System.Data.ConstraintException">The value is null for a non-nullable property.</exception>
    protected internal static DbGeometry SetValidValue(
      DbGeometry value,
      bool isNullable,
      string propertyName)
    {
      if (value == null && !isNullable)
        EntityUtil.ThrowPropertyIsNotNullable(propertyName);
      return value;
    }

    /// <summary>Validates that the property is not null, and throws if it is.</summary>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Spatial.DbGeometry" /> value being set.
    /// </returns>
    /// <param name="value">
    /// The <see cref="T:System.Data.Entity.Spatial.DbGeometry" /> value to be checked.
    /// </param>
    /// <param name="isNullable">Flag indicating if this property is allowed to be null.</param>
    /// <exception cref="T:System.Data.ConstraintException">The value is null for a non-nullable property.</exception>
    protected internal static DbGeometry SetValidValue(DbGeometry value, bool isNullable)
    {
      return StructuralObject.SetValidValue(value, isNullable, (string) null);
    }

    /// <summary>Sets a complex object for the specified property.</summary>
    /// <returns>A complex type that derives from complex object.</returns>
    /// <param name="oldValue">The original complex object for the property, if any.</param>
    /// <param name="newValue">The complex object is being set.</param>
    /// <param name="property">The complex property that is being set to the complex object.</param>
    /// <typeparam name="T">The type of the object being replaced.</typeparam>
    protected internal T SetValidValue<T>(T oldValue, T newValue, string property) where T : ComplexObject
    {
      if ((object) newValue == null && this.IsChangeTracked)
        throw new InvalidOperationException(Strings.ComplexObject_NullableComplexTypesNotSupported((object) property));
      oldValue?.DetachFromParent();
      newValue?.AttachToParent(this, property);
      return newValue;
    }

    /// <summary>Verifies that a complex object is not null.</summary>
    /// <returns>The complex object being validated.</returns>
    /// <param name="complexObject">The complex object that is being validated.</param>
    /// <param name="propertyName">The complex property on the parent object that is associated with  complexObject .</param>
    /// <typeparam name="TComplex">The type of the complex object being verified.</typeparam>
    protected internal static TComplex VerifyComplexObjectIsNotNull<TComplex>(
      TComplex complexObject,
      string propertyName)
      where TComplex : ComplexObject
    {
      if ((object) complexObject == null)
        EntityUtil.ThrowPropertyIsNotNullable(propertyName);
      return complexObject;
    }
  }
}
