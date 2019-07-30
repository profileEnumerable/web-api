// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ManyNavigationPropertyConfiguration`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>Configures a many relationship from an entity type.</summary>
  /// <typeparam name="TEntityType"> The entity type that the relationship originates from. </typeparam>
  /// <typeparam name="TTargetEntityType"> The entity type that the relationship targets. </typeparam>
  public class ManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType>
    where TEntityType : class
    where TTargetEntityType : class
  {
    private readonly NavigationPropertyConfiguration _navigationPropertyConfiguration;

    internal ManyNavigationPropertyConfiguration(
      NavigationPropertyConfiguration navigationPropertyConfiguration)
    {
      navigationPropertyConfiguration.Reset();
      this._navigationPropertyConfiguration = navigationPropertyConfiguration;
      this._navigationPropertyConfiguration.RelationshipMultiplicity = new RelationshipMultiplicity?(RelationshipMultiplicity.Many);
    }

    /// <summary>
    /// Configures the relationship to be many:many with a navigation property on the other side of the relationship.
    /// </summary>
    /// <param name="navigationPropertyExpression"> An lambda expression representing the navigation property on the other end of the relationship. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public ManyToManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType> WithMany(
      Expression<Func<TTargetEntityType, ICollection<TEntityType>>> navigationPropertyExpression)
    {
      Check.NotNull<Expression<Func<TTargetEntityType, ICollection<TEntityType>>>>(navigationPropertyExpression, nameof (navigationPropertyExpression));
      this._navigationPropertyConfiguration.InverseNavigationProperty = navigationPropertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>();
      return this.WithMany();
    }

    /// <summary>
    /// Configures the relationship to be many:many without a navigation property on the other side of the relationship.
    /// </summary>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    public ManyToManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType> WithMany()
    {
      this._navigationPropertyConfiguration.InverseEndKind = new RelationshipMultiplicity?(RelationshipMultiplicity.Many);
      return new ManyToManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType>(this._navigationPropertyConfiguration);
    }

    /// <summary>
    /// Configures the relationship to be many:required with a navigation property on the other side of the relationship.
    /// </summary>
    /// <param name="navigationPropertyExpression"> An lambda expression representing the navigation property on the other end of the relationship. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public DependentNavigationPropertyConfiguration<TTargetEntityType> WithRequired(
      Expression<Func<TTargetEntityType, TEntityType>> navigationPropertyExpression)
    {
      Check.NotNull<Expression<Func<TTargetEntityType, TEntityType>>>(navigationPropertyExpression, nameof (navigationPropertyExpression));
      this._navigationPropertyConfiguration.InverseNavigationProperty = navigationPropertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>();
      return this.WithRequired();
    }

    /// <summary>
    /// Configures the relationship to be many:required without a navigation property on the other side of the relationship.
    /// </summary>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    public DependentNavigationPropertyConfiguration<TTargetEntityType> WithRequired()
    {
      this._navigationPropertyConfiguration.InverseEndKind = new RelationshipMultiplicity?(RelationshipMultiplicity.One);
      return new DependentNavigationPropertyConfiguration<TTargetEntityType>(this._navigationPropertyConfiguration);
    }

    /// <summary>
    /// Configures the relationship to be many:optional with a navigation property on the other side of the relationship.
    /// </summary>
    /// <param name="navigationPropertyExpression"> An lambda expression representing the navigation property on the other end of the relationship. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public DependentNavigationPropertyConfiguration<TTargetEntityType> WithOptional(
      Expression<Func<TTargetEntityType, TEntityType>> navigationPropertyExpression)
    {
      Check.NotNull<Expression<Func<TTargetEntityType, TEntityType>>>(navigationPropertyExpression, nameof (navigationPropertyExpression));
      this._navigationPropertyConfiguration.InverseNavigationProperty = navigationPropertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>();
      return this.WithOptional();
    }

    /// <summary>
    /// Configures the relationship to be many:optional without a navigation property on the other side of the relationship.
    /// </summary>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    public DependentNavigationPropertyConfiguration<TTargetEntityType> WithOptional()
    {
      this._navigationPropertyConfiguration.InverseEndKind = new RelationshipMultiplicity?(RelationshipMultiplicity.ZeroOrOne);
      return new DependentNavigationPropertyConfiguration<TTargetEntityType>(this._navigationPropertyConfiguration);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString()
    {
      return base.ToString();
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the current instance.
    /// </summary>
    /// <returns>The exact runtime type of the current instance.</returns>
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
