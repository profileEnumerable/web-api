// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbComplexPropertyEntry`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Internal;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Instances of this class are returned from the ComplexProperty method of
  /// <see cref="T:System.Data.Entity.Infrastructure.DbEntityEntry`1" /> and allow access to the state of a complex property.
  /// </summary>
  /// <typeparam name="TEntity"> The type of the entity to which this property belongs. </typeparam>
  /// <typeparam name="TComplexProperty"> The type of the property. </typeparam>
  public class DbComplexPropertyEntry<TEntity, TComplexProperty> : DbPropertyEntry<TEntity, TComplexProperty>
    where TEntity : class
  {
    internal static DbComplexPropertyEntry<TEntity, TComplexProperty> Create(
      InternalPropertyEntry internalPropertyEntry)
    {
      return (DbComplexPropertyEntry<TEntity, TComplexProperty>) internalPropertyEntry.CreateDbMemberEntry<TEntity, TComplexProperty>();
    }

    internal DbComplexPropertyEntry(InternalPropertyEntry internalPropertyEntry)
      : base(internalPropertyEntry)
    {
    }

    /// <summary>
    /// Returns a new instance of the non-generic <see cref="T:System.Data.Entity.Infrastructure.DbComplexPropertyEntry" /> class for
    /// the property represented by this object.
    /// </summary>
    /// <param name="entry">The object representing the property.</param>
    /// <returns> A non-generic version. </returns>
    [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Intentionally just implicit to reduce API clutter.")]
    public static implicit operator DbComplexPropertyEntry(
      DbComplexPropertyEntry<TEntity, TComplexProperty> entry)
    {
      return DbComplexPropertyEntry.Create(entry.InternalPropertyEntry);
    }

    /// <summary>
    /// Gets an object that represents a nested property of this property.
    /// This method can be used for both scalar or complex properties.
    /// </summary>
    /// <param name="propertyName"> The name of the nested property. </param>
    /// <returns> An object representing the nested property. </returns>
    public DbPropertyEntry Property(string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      return DbPropertyEntry.Create(this.InternalPropertyEntry.Property(propertyName, (Type) null, false));
    }

    /// <summary>
    /// Gets an object that represents a nested property of this property.
    /// This method can be used for both scalar or complex properties.
    /// </summary>
    /// <typeparam name="TNestedProperty"> The type of the nested property. </typeparam>
    /// <param name="propertyName"> The name of the nested property. </param>
    /// <returns> An object representing the nested property. </returns>
    public DbPropertyEntry<TEntity, TNestedProperty> Property<TNestedProperty>(
      string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      return DbPropertyEntry<TEntity, TNestedProperty>.Create(this.InternalPropertyEntry.Property(propertyName, typeof (TNestedProperty), false));
    }

    /// <summary>
    /// Gets an object that represents a nested property of this property.
    /// This method can be used for both scalar or complex properties.
    /// </summary>
    /// <typeparam name="TNestedProperty"> The type of the nested property. </typeparam>
    /// <param name="property"> An expression representing the nested property. </param>
    /// <returns> An object representing the nested property. </returns>
    [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", Justification = "Rule predates more fluent naming conventions.", MessageId = "0#")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public DbPropertyEntry<TEntity, TNestedProperty> Property<TNestedProperty>(
      Expression<Func<TComplexProperty, TNestedProperty>> property)
    {
      Check.NotNull<Expression<Func<TComplexProperty, TNestedProperty>>>(property, nameof (property));
      return this.Property<TNestedProperty>(DbHelpers.ParsePropertySelector<TComplexProperty, TNestedProperty>(property, nameof (Property), nameof (property)));
    }

    /// <summary>
    /// Gets an object that represents a nested complex property of this property.
    /// </summary>
    /// <param name="propertyName"> The name of the nested property. </param>
    /// <returns> An object representing the nested property. </returns>
    public DbComplexPropertyEntry ComplexProperty(string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      return DbComplexPropertyEntry.Create(this.InternalPropertyEntry.Property(propertyName, (Type) null, true));
    }

    /// <summary>
    /// Gets an object that represents a nested complex property of this property.
    /// </summary>
    /// <typeparam name="TNestedComplexProperty"> The type of the nested property. </typeparam>
    /// <param name="propertyName"> The name of the nested property. </param>
    /// <returns> An object representing the nested property. </returns>
    public DbComplexPropertyEntry<TEntity, TNestedComplexProperty> ComplexProperty<TNestedComplexProperty>(
      string propertyName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      return DbComplexPropertyEntry<TEntity, TNestedComplexProperty>.Create(this.InternalPropertyEntry.Property(propertyName, typeof (TNestedComplexProperty), true));
    }

    /// <summary>
    /// Gets an object that represents a nested complex property of this property.
    /// </summary>
    /// <typeparam name="TNestedComplexProperty"> The type of the nested property. </typeparam>
    /// <param name="property"> An expression representing the nested property. </param>
    /// <returns> An object representing the nested property. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", Justification = "Rule predates more fluent naming conventions.", MessageId = "0#")]
    public DbComplexPropertyEntry<TEntity, TNestedComplexProperty> ComplexProperty<TNestedComplexProperty>(
      Expression<Func<TComplexProperty, TNestedComplexProperty>> property)
    {
      Check.NotNull<Expression<Func<TComplexProperty, TNestedComplexProperty>>>(property, nameof (property));
      return this.ComplexProperty<TNestedComplexProperty>(DbHelpers.ParsePropertySelector<TComplexProperty, TNestedComplexProperty>(property, "Property", nameof (property)));
    }
  }
}
