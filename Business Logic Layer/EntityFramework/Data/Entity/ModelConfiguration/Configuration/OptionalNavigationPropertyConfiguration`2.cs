// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.OptionalNavigationPropertyConfiguration`2
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
  /// <summary>
  /// Configures an optional relationship from an entity type.
  /// </summary>
  /// <typeparam name="TEntityType"> The entity type that the relationship originates from. </typeparam>
  /// <typeparam name="TTargetEntityType"> The entity type that the relationship targets. </typeparam>
  public class OptionalNavigationPropertyConfiguration<TEntityType, TTargetEntityType>
    where TEntityType : class
    where TTargetEntityType : class
  {
    private readonly NavigationPropertyConfiguration _navigationPropertyConfiguration;

    internal OptionalNavigationPropertyConfiguration(
      NavigationPropertyConfiguration navigationPropertyConfiguration)
    {
      navigationPropertyConfiguration.Reset();
      this._navigationPropertyConfiguration = navigationPropertyConfiguration;
      this._navigationPropertyConfiguration.RelationshipMultiplicity = new RelationshipMultiplicity?(RelationshipMultiplicity.ZeroOrOne);
    }

    /// <summary>
    /// Configures the relationship to be optional:many with a navigation property on the other side of the relationship.
    /// </summary>
    /// <param name="navigationPropertyExpression"> An lambda expression representing the navigation property on the other end of the relationship. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public DependentNavigationPropertyConfiguration<TEntityType> WithMany(
      Expression<Func<TTargetEntityType, ICollection<TEntityType>>> navigationPropertyExpression)
    {
      Check.NotNull<Expression<Func<TTargetEntityType, ICollection<TEntityType>>>>(navigationPropertyExpression, nameof (navigationPropertyExpression));
      this._navigationPropertyConfiguration.InverseNavigationProperty = navigationPropertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>();
      return this.WithMany();
    }

    /// <summary>
    /// Configures the relationship to be optional:many without a navigation property on the other side of the relationship.
    /// </summary>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    public DependentNavigationPropertyConfiguration<TEntityType> WithMany()
    {
      this._navigationPropertyConfiguration.InverseEndKind = new RelationshipMultiplicity?(RelationshipMultiplicity.Many);
      return new DependentNavigationPropertyConfiguration<TEntityType>(this._navigationPropertyConfiguration);
    }

    /// <summary>
    /// Configures the relationship to be optional:required with a navigation property on the other side of the relationship.
    /// </summary>
    /// <param name="navigationPropertyExpression"> An lambda expression representing the navigation property on the other end of the relationship. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public ForeignKeyNavigationPropertyConfiguration WithRequired(
      Expression<Func<TTargetEntityType, TEntityType>> navigationPropertyExpression)
    {
      Check.NotNull<Expression<Func<TTargetEntityType, TEntityType>>>(navigationPropertyExpression, nameof (navigationPropertyExpression));
      this._navigationPropertyConfiguration.InverseNavigationProperty = navigationPropertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>();
      return this.WithRequired();
    }

    /// <summary>
    /// Configures the relationship to be optional:required without a navigation property on the other side of the relationship.
    /// </summary>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    public ForeignKeyNavigationPropertyConfiguration WithRequired()
    {
      this._navigationPropertyConfiguration.InverseEndKind = new RelationshipMultiplicity?(RelationshipMultiplicity.One);
      return new ForeignKeyNavigationPropertyConfiguration(this._navigationPropertyConfiguration);
    }

    /// <summary>
    /// Configures the relationship to be optional:optional with a navigation property on the other side of the relationship.
    /// The entity type being configured will be the dependent and contain a foreign key to the principal.
    /// The entity type that the relationship targets will be the principal in the relationship.
    /// </summary>
    /// <param name="navigationPropertyExpression"> An lambda expression representing the navigation property on the other end of the relationship. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ForeignKeyNavigationPropertyConfiguration WithOptionalDependent(
      Expression<Func<TTargetEntityType, TEntityType>> navigationPropertyExpression)
    {
      Check.NotNull<Expression<Func<TTargetEntityType, TEntityType>>>(navigationPropertyExpression, nameof (navigationPropertyExpression));
      this._navigationPropertyConfiguration.InverseNavigationProperty = navigationPropertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>();
      return this.WithOptionalDependent();
    }

    /// <summary>
    /// Configures the relationship to be optional:optional without a navigation property on the other side of the relationship.
    /// The entity type being configured will be the dependent and contain a foreign key to the principal.
    /// The entity type that the relationship targets will be the principal in the relationship.
    /// </summary>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    public ForeignKeyNavigationPropertyConfiguration WithOptionalDependent()
    {
      this._navigationPropertyConfiguration.InverseEndKind = new RelationshipMultiplicity?(RelationshipMultiplicity.ZeroOrOne);
      this._navigationPropertyConfiguration.Constraint = IndependentConstraintConfiguration.Instance;
      this._navigationPropertyConfiguration.IsNavigationPropertyDeclaringTypePrincipal = new bool?(false);
      return new ForeignKeyNavigationPropertyConfiguration(this._navigationPropertyConfiguration);
    }

    /// <summary>
    /// Configures the relationship to be optional:optional with a navigation property on the other side of the relationship.
    /// The entity type being configured will be the principal in the relationship.
    /// The entity type that the relationship targets will be the dependent and contain a foreign key to the principal.
    /// </summary>
    /// <param name="navigationPropertyExpression"> A lambda expression representing the navigation property on the other end of the relationship. </param>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
    public ForeignKeyNavigationPropertyConfiguration WithOptionalPrincipal(
      Expression<Func<TTargetEntityType, TEntityType>> navigationPropertyExpression)
    {
      Check.NotNull<Expression<Func<TTargetEntityType, TEntityType>>>(navigationPropertyExpression, nameof (navigationPropertyExpression));
      this._navigationPropertyConfiguration.InverseNavigationProperty = navigationPropertyExpression.GetSimplePropertyAccess().Single<PropertyInfo>();
      return this.WithOptionalPrincipal();
    }

    /// <summary>
    /// Configures the relationship to be optional:optional without a navigation property on the other side of the relationship.
    /// The entity type being configured will be the principal in the relationship.
    /// The entity type that the relationship targets will be the dependent and contain a foreign key to the principal.
    /// </summary>
    /// <returns> A configuration object that can be used to further configure the relationship. </returns>
    public ForeignKeyNavigationPropertyConfiguration WithOptionalPrincipal()
    {
      this._navigationPropertyConfiguration.InverseEndKind = new RelationshipMultiplicity?(RelationshipMultiplicity.ZeroOrOne);
      this._navigationPropertyConfiguration.Constraint = IndependentConstraintConfiguration.Instance;
      this._navigationPropertyConfiguration.IsNavigationPropertyDeclaringTypePrincipal = new bool?(true);
      return new ForeignKeyNavigationPropertyConfiguration(this._navigationPropertyConfiguration);
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
