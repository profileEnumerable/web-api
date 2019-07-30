// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.CommandTrees.DbExpression
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Data.Entity.Spatial;
using System.Globalization;

namespace System.Data.Entity.Core.Common.CommandTrees
{
  /// <summary>Represents the base type for all expressions.</summary>
  public abstract class DbExpression
  {
    private readonly TypeUsage _type;
    private readonly DbExpressionKind _kind;

    internal DbExpression()
    {
    }

    internal DbExpression(DbExpressionKind kind, TypeUsage type, bool forceNullable = true)
    {
      DbExpression.CheckExpressionKind(kind);
      this._kind = kind;
      if (forceNullable && !TypeSemantics.IsNullable(type))
        type = type.ShallowCopy(new FacetValues()
        {
          Nullable = (FacetValueContainer<bool?>) new bool?(true)
        });
      this._type = type;
    }

    /// <summary>Gets the type metadata for the result type of the expression.</summary>
    /// <returns>The type metadata for the result type of the expression.</returns>
    public virtual TypeUsage ResultType
    {
      get
      {
        return this._type;
      }
    }

    /// <summary>Gets the kind of the expression, which indicates the operation of this expression.</summary>
    /// <returns>The kind of the expression, which indicates the operation of this expression.</returns>
    public virtual DbExpressionKind ExpressionKind
    {
      get
      {
        return this._kind;
      }
    }

    /// <summary>Implements the visitor pattern for expressions that do not produce a result value.</summary>
    /// <param name="visitor">
    /// An instance of <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionVisitor" />.
    /// </param>
    public abstract void Accept(DbExpressionVisitor visitor);

    /// <summary>Implements the visitor pattern for expressions that produce a result value of a specific type.</summary>
    /// <returns>
    /// The type of the result produced by <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionVisitor" />.
    /// </returns>
    /// <param name="visitor">
    /// An instance of <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpressionVisitor" />.
    /// </param>
    /// <typeparam name="TResultType">The type of the result produced by visitor.</typeparam>
    public abstract TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor);

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current DbExpression instance.
    /// </summary>
    /// <returns>
    /// True if the specified <see cref="T:System.Object" /> is equal to the current DbExpression instance; otherwise, false.
    /// </returns>
    /// <param name="obj">
    /// The object to compare to the current <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" />.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <summary>Serves as a hash function for the type.</summary>
    /// <returns>A hash code for the current expression.</returns>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified binary value, which may be null
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified binary value.
    /// </returns>
    /// <param name="value">The binary value on which the returned expression should be based.</param>
    public static DbExpression FromBinary(byte[] value)
    {
      if (value == null)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Binary);
      return (DbExpression) DbExpressionBuilder.Constant((object) value);
    }

    /// <summary>Enables implicit casting from a byte array.</summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(byte[] value)
    {
      return DbExpression.FromBinary(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified (nullable) Boolean value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified Boolean value.
    /// </returns>
    /// <param name="value">The Boolean value on which the returned expression should be based.</param>
    public static DbExpression FromBoolean(bool? value)
    {
      if (!value.HasValue)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Boolean);
      if (!value.Value)
        return (DbExpression) DbExpressionBuilder.False;
      return (DbExpression) DbExpressionBuilder.True;
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(bool? value)
    {
      return DbExpression.FromBoolean(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified (nullable) byte value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified byte value.
    /// </returns>
    /// <param name="value">The byte value on which the returned expression should be based.</param>
    public static DbExpression FromByte(byte? value)
    {
      if (!value.HasValue)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Byte);
      return (DbExpression) DbExpressionBuilder.Constant((object) value.Value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(byte? value)
    {
      return DbExpression.FromByte(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified (nullable)
    /// <see cref="T:System.DateTime" />
    /// value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified DateTime value.
    /// </returns>
    /// <param name="value">The DateTime value on which the returned expression should be based.</param>
    public static DbExpression FromDateTime(DateTime? value)
    {
      if (!value.HasValue)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.DateTime);
      return (DbExpression) DbExpressionBuilder.Constant((object) value.Value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <param name="value">The expression to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(DateTime? value)
    {
      return DbExpression.FromDateTime(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified (nullable)
    /// <see cref="T:System.DateTimeOffset" />
    /// value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified DateTimeOffset value.
    /// </returns>
    /// <param name="value">The DateTimeOffset value on which the returned expression should be based.</param>
    public static DbExpression FromDateTimeOffset(DateTimeOffset? value)
    {
      if (!value.HasValue)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.DateTimeOffset);
      return (DbExpression) DbExpressionBuilder.Constant((object) value.Value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(DateTimeOffset? value)
    {
      return DbExpression.FromDateTimeOffset(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified (nullable) decimal value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified decimal value.
    /// </returns>
    /// <param name="value">The decimal value on which the returned expression should be based.</param>
    public static DbExpression FromDecimal(Decimal? value)
    {
      if (!value.HasValue)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Decimal);
      return (DbExpression) DbExpressionBuilder.Constant((object) value.Value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(Decimal? value)
    {
      return DbExpression.FromDecimal(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified (nullable) double value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified double value.
    /// </returns>
    /// <param name="value">The double value on which the returned expression should be based.</param>
    public static DbExpression FromDouble(double? value)
    {
      if (!value.HasValue)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Double);
      return (DbExpression) DbExpressionBuilder.Constant((object) value.Value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(double? value)
    {
      return DbExpression.FromDouble(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified
    /// <see cref="T:System.Data.Entity.Spatial.DbGeography" />
    /// value, which may be null.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified DbGeography value.
    /// </returns>
    /// <param name="value">The DbGeography value on which the returned expression should be based.</param>
    public static DbExpression FromGeography(DbGeography value)
    {
      if (value == null)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Geography);
      return (DbExpression) DbExpressionBuilder.Constant((object) value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Data.Entity.Spatial.DbGeography" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(DbGeography value)
    {
      return DbExpression.FromGeography(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified
    /// <see cref="T:System.Data.Entity.Spatial.DbGeometry" />
    /// value, which may be null.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified DbGeometry value.
    /// </returns>
    /// <param name="value">The DbGeometry value on which the returned expression should be based.</param>
    public static DbExpression FromGeometry(DbGeometry value)
    {
      if (value == null)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Geometry);
      return (DbExpression) DbExpressionBuilder.Constant((object) value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Data.Entity.Spatial.DbGeometry" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(DbGeometry value)
    {
      return DbExpression.FromGeometry(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified (nullable)
    /// <see cref="T:System.Guid" />
    /// value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified Guid value.
    /// </returns>
    /// <param name="value">The Guid value on which the returned expression should be based.</param>
    public static DbExpression FromGuid(Guid? value)
    {
      if (!value.HasValue)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Guid);
      return (DbExpression) DbExpressionBuilder.Constant((object) value.Value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(Guid? value)
    {
      return DbExpression.FromGuid(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified (nullable) Int16 value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified Int16 value.
    /// </returns>
    /// <param name="value">The Int16 value on which the returned expression should be based.</param>
    public static DbExpression FromInt16(short? value)
    {
      if (!value.HasValue)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Int16);
      return (DbExpression) DbExpressionBuilder.Constant((object) value.Value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(short? value)
    {
      return DbExpression.FromInt16(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified (nullable) Int32 value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified Int32 value.
    /// </returns>
    /// <param name="value">The Int32 value on which the returned expression should be based.</param>
    public static DbExpression FromInt32(int? value)
    {
      if (!value.HasValue)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Int32);
      return (DbExpression) DbExpressionBuilder.Constant((object) value.Value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(int? value)
    {
      return DbExpression.FromInt32(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified (nullable) Int64 value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified Int64 value.
    /// </returns>
    /// <param name="value">The Int64 value on which the returned expression should be based.</param>
    public static DbExpression FromInt64(long? value)
    {
      if (!value.HasValue)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Int64);
      return (DbExpression) DbExpressionBuilder.Constant((object) value.Value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(long? value)
    {
      return DbExpression.FromInt64(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified (nullable) Single value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified Single value.
    /// </returns>
    /// <param name="value">The Single value on which the returned expression should be based.</param>
    public static DbExpression FromSingle(float? value)
    {
      if (!value.HasValue)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.Single);
      return (DbExpression) DbExpressionBuilder.Constant((object) value.Value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.Nullable`1" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(float? value)
    {
      return DbExpression.FromSingle(value);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified string value.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbExpression" /> that represents the specified string value.
    /// </returns>
    /// <param name="value">The string value on which the returned expression should be based.</param>
    public static DbExpression FromString(string value)
    {
      if (value == null)
        return (DbExpression) DbExpressionBuilder.CreatePrimitiveNullExpression(PrimitiveTypeKind.String);
      return (DbExpression) DbExpressionBuilder.Constant((object) value);
    }

    /// <summary>
    /// Enables implicit casting from <see cref="T:System.String" />.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value.</returns>
    public static implicit operator DbExpression(string value)
    {
      return DbExpression.FromString(value);
    }

    internal static void CheckExpressionKind(DbExpressionKind kind)
    {
      if (kind < DbExpressionKind.All || DbExpressionKindHelper.Last < kind)
      {
        string name = typeof (DbExpressionKind).Name;
        throw new ArgumentOutOfRangeException(name, Strings.ADP_InvalidEnumerationValue((object) name, (object) ((int) kind).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
    }
  }
}
