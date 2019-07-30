// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.AssociationModificationStoredProcedureConfiguration`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Allows configuration to be performed for a stored procedure that is used to modify a relationship.
  /// </summary>
  /// <typeparam name="TEntityType">The type of the entity that the relationship is being configured from.</typeparam>
  public class AssociationModificationStoredProcedureConfiguration<TEntityType> where TEntityType : class
  {
    private readonly PropertyInfo _navigationPropertyInfo;
    private readonly ModificationStoredProcedureConfiguration _configuration;

    internal AssociationModificationStoredProcedureConfiguration(
      PropertyInfo navigationPropertyInfo,
      ModificationStoredProcedureConfiguration configuration)
    {
      this._navigationPropertyInfo = navigationPropertyInfo;
      this._configuration = configuration;
    }

    /// <summary>Configures a parameter for this stored procedure.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="propertyExpression"> A lambda expression representing the property to configure the parameter for. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public AssociationModificationStoredProcedureConfiguration<TEntityType> Parameter<TProperty>(
      Expression<Func<TEntityType, TProperty>> propertyExpression,
      string parameterName)
      where TProperty : struct
    {
      Check.NotNull<Expression<Func<TEntityType, TProperty>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this._configuration.Parameter(new PropertyPath(((IEnumerable<PropertyInfo>) new PropertyInfo[1]
      {
        this._navigationPropertyInfo
      }).Concat<PropertyInfo>((IEnumerable<PropertyInfo>) propertyExpression.GetSimplePropertyAccess())), parameterName, (string) null, false);
      return this;
    }

    /// <summary>Configures a parameter for this stored procedure.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="propertyExpression"> A lambda expression representing the property to configure the parameter for. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public AssociationModificationStoredProcedureConfiguration<TEntityType> Parameter<TProperty>(
      Expression<Func<TEntityType, TProperty?>> propertyExpression,
      string parameterName)
      where TProperty : struct
    {
      Check.NotNull<Expression<Func<TEntityType, TProperty?>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this._configuration.Parameter(new PropertyPath(((IEnumerable<PropertyInfo>) new PropertyInfo[1]
      {
        this._navigationPropertyInfo
      }).Concat<PropertyInfo>((IEnumerable<PropertyInfo>) propertyExpression.GetSimplePropertyAccess())), parameterName, (string) null, false);
      return this;
    }

    /// <summary>Configures a parameter for this stored procedure.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="propertyExpression"> A lambda expression representing the property to configure the parameter for. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">The name of the parameter.</param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public AssociationModificationStoredProcedureConfiguration<TEntityType> Parameter(
      Expression<Func<TEntityType, string>> propertyExpression,
      string parameterName)
    {
      Check.NotNull<Expression<Func<TEntityType, string>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this._configuration.Parameter(new PropertyPath(((IEnumerable<PropertyInfo>) new PropertyInfo[1]
      {
        this._navigationPropertyInfo
      }).Concat<PropertyInfo>((IEnumerable<PropertyInfo>) propertyExpression.GetSimplePropertyAccess())), parameterName, (string) null, false);
      return this;
    }

    /// <summary>Configures a parameter for this stored procedure.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="propertyExpression"> A lambda expression representing the property to configure the parameter for. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">The name of the parameter.</param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public AssociationModificationStoredProcedureConfiguration<TEntityType> Parameter(
      Expression<Func<TEntityType, byte[]>> propertyExpression,
      string parameterName)
    {
      Check.NotNull<Expression<Func<TEntityType, byte[]>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this._configuration.Parameter(new PropertyPath(((IEnumerable<PropertyInfo>) new PropertyInfo[1]
      {
        this._navigationPropertyInfo
      }).Concat<PropertyInfo>((IEnumerable<PropertyInfo>) propertyExpression.GetSimplePropertyAccess())), parameterName, (string) null, false);
      return this;
    }
  }
}
