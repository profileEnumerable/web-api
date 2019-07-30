// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ManyToManyModificationStoredProcedureConfiguration`2
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Allows configuration to be performed for a stored procedure that is used to modify a many to many relationship.
  /// </summary>
  /// <typeparam name="TEntityType">The type of the entity that the relationship is being configured from.</typeparam>
  /// <typeparam name="TTargetEntityType">The type of the entity that the other end of the relationship targets.</typeparam>
  public class ManyToManyModificationStoredProcedureConfiguration<TEntityType, TTargetEntityType> : ModificationStoredProcedureConfigurationBase
    where TEntityType : class
    where TTargetEntityType : class
  {
    internal ManyToManyModificationStoredProcedureConfiguration()
    {
    }

    /// <summary>Sets the name of the stored procedure.</summary>
    /// <param name="procedureName">Name of the procedure.</param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    public ManyToManyModificationStoredProcedureConfiguration<TEntityType, TTargetEntityType> HasName(
      string procedureName)
    {
      Check.NotEmpty(procedureName, nameof (procedureName));
      this.Configuration.HasName(procedureName);
      return this;
    }

    /// <summary>Sets the name of the stored procedure.</summary>
    /// <param name="procedureName">Name of the procedure.</param>
    /// <param name="schemaName">Name of the schema.</param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    public ManyToManyModificationStoredProcedureConfiguration<TEntityType, TTargetEntityType> HasName(
      string procedureName,
      string schemaName)
    {
      Check.NotEmpty(procedureName, nameof (procedureName));
      Check.NotEmpty(schemaName, nameof (schemaName));
      this.Configuration.HasName(procedureName, schemaName);
      return this;
    }

    /// <summary>Configures the parameter for the left key value(s).</summary>
    /// <typeparam name="TProperty">The type of the property to configure.</typeparam>
    /// <param name="propertyExpression"> A lambda expression representing the property to be configured. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ManyToManyModificationStoredProcedureConfiguration<TEntityType, TTargetEntityType> LeftKeyParameter<TProperty>(
      Expression<Func<TEntityType, TProperty>> propertyExpression,
      string parameterName)
      where TProperty : struct
    {
      Check.NotNull<Expression<Func<TEntityType, TProperty>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this.Configuration.Parameter(propertyExpression.GetSimplePropertyAccess(), parameterName, (string) null, false);
      return this;
    }

    /// <summary>Configures the parameter for the left key value(s).</summary>
    /// <typeparam name="TProperty">The type of the property to configure.</typeparam>
    /// <param name="propertyExpression"> A lambda expression representing the property to be configured. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ManyToManyModificationStoredProcedureConfiguration<TEntityType, TTargetEntityType> LeftKeyParameter<TProperty>(
      Expression<Func<TEntityType, TProperty?>> propertyExpression,
      string parameterName)
      where TProperty : struct
    {
      Check.NotNull<Expression<Func<TEntityType, TProperty?>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this.Configuration.Parameter(propertyExpression.GetSimplePropertyAccess(), parameterName, (string) null, false);
      return this;
    }

    /// <summary>Configures the parameter for the left key value(s).</summary>
    /// <param name="propertyExpression"> A lambda expression representing the property to be configured. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ManyToManyModificationStoredProcedureConfiguration<TEntityType, TTargetEntityType> LeftKeyParameter(
      Expression<Func<TEntityType, string>> propertyExpression,
      string parameterName)
    {
      Check.NotNull<Expression<Func<TEntityType, string>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this.Configuration.Parameter(propertyExpression.GetSimplePropertyAccess(), parameterName, (string) null, false);
      return this;
    }

    /// <summary>Configures the parameter for the left key value(s).</summary>
    /// <param name="propertyExpression"> A lambda expression representing the property to be configured. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ManyToManyModificationStoredProcedureConfiguration<TEntityType, TTargetEntityType> LeftKeyParameter(
      Expression<Func<TEntityType, byte[]>> propertyExpression,
      string parameterName)
    {
      Check.NotNull<Expression<Func<TEntityType, byte[]>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this.Configuration.Parameter(propertyExpression.GetSimplePropertyAccess(), parameterName, (string) null, false);
      return this;
    }

    /// <summary>Configures the parameter for the right key value(s).</summary>
    /// <typeparam name="TProperty">The type of the property to configure.</typeparam>
    /// <param name="propertyExpression"> A lambda expression representing the property to be configured. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ManyToManyModificationStoredProcedureConfiguration<TEntityType, TTargetEntityType> RightKeyParameter<TProperty>(
      Expression<Func<TTargetEntityType, TProperty>> propertyExpression,
      string parameterName)
      where TProperty : struct
    {
      Check.NotNull<Expression<Func<TTargetEntityType, TProperty>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this.Configuration.Parameter(propertyExpression.GetSimplePropertyAccess(), parameterName, (string) null, true);
      return this;
    }

    /// <summary>Configures the parameter for the right key value(s).</summary>
    /// <typeparam name="TProperty">The type of the property to configure.</typeparam>
    /// <param name="propertyExpression"> A lambda expression representing the property to be configured. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ManyToManyModificationStoredProcedureConfiguration<TEntityType, TTargetEntityType> RightKeyParameter<TProperty>(
      Expression<Func<TTargetEntityType, TProperty?>> propertyExpression,
      string parameterName)
      where TProperty : struct
    {
      Check.NotNull<Expression<Func<TTargetEntityType, TProperty?>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this.Configuration.Parameter(propertyExpression.GetSimplePropertyAccess(), parameterName, (string) null, true);
      return this;
    }

    /// <summary>Configures the parameter for the right key value(s).</summary>
    /// <param name="propertyExpression"> A lambda expression representing the property to be configured. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ManyToManyModificationStoredProcedureConfiguration<TEntityType, TTargetEntityType> RightKeyParameter(
      Expression<Func<TTargetEntityType, string>> propertyExpression,
      string parameterName)
    {
      Check.NotNull<Expression<Func<TTargetEntityType, string>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this.Configuration.Parameter(propertyExpression.GetSimplePropertyAccess(), parameterName, (string) null, true);
      return this;
    }

    /// <summary>Configures the parameter for the right key value(s).</summary>
    /// <param name="propertyExpression"> A lambda expression representing the property to be configured. C#: t =&gt; t.MyProperty VB.Net: Function(t) t.MyProperty </param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ManyToManyModificationStoredProcedureConfiguration<TEntityType, TTargetEntityType> RightKeyParameter(
      Expression<Func<TTargetEntityType, byte[]>> propertyExpression,
      string parameterName)
    {
      Check.NotNull<Expression<Func<TTargetEntityType, byte[]>>>(propertyExpression, nameof (propertyExpression));
      Check.NotEmpty(parameterName, nameof (parameterName));
      this.Configuration.Parameter(propertyExpression.GetSimplePropertyAccess(), parameterName, (string) null, true);
      return this;
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

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
