// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConventionUpdateModificationStoredProcedureConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Creates a convention that configures stored procedures to be used to update entities in the database.
  /// </summary>
  public class ConventionUpdateModificationStoredProcedureConfiguration : ConventionModificationStoredProcedureConfiguration
  {
    private readonly Type _type;

    internal ConventionUpdateModificationStoredProcedureConfiguration(Type type)
    {
      this._type = type;
    }

    /// <summary> Configures the name of the stored procedure. </summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="procedureName"> The stored procedure name. </param>
    public ConventionUpdateModificationStoredProcedureConfiguration HasName(
      string procedureName)
    {
      Check.NotEmpty(procedureName, nameof (procedureName));
      this.Configuration.HasName(procedureName);
      return this;
    }

    /// <summary>Configures the name of the stored procedure.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="procedureName">The stored procedure name.</param>
    /// <param name="schemaName">The schema name.</param>
    public ConventionUpdateModificationStoredProcedureConfiguration HasName(
      string procedureName,
      string schemaName)
    {
      Check.NotEmpty(procedureName, nameof (procedureName));
      Check.NotEmpty(schemaName, nameof (schemaName));
      this.Configuration.HasName(procedureName, schemaName);
      return this;
    }

    /// <summary>Configures a parameter for this stored procedure.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="propertyName"> The name of the property to configure the parameter for. </param>
    /// <param name="parameterName"> The name of the parameter. </param>
    public ConventionUpdateModificationStoredProcedureConfiguration Parameter(
      string propertyName,
      string parameterName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      Check.NotEmpty(parameterName, nameof (parameterName));
      return this.Parameter(this._type.GetAnyProperty(propertyName), parameterName);
    }

    /// <summary>Configures a parameter for this stored procedure.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="propertyInfo"> The property to configure the parameter for. </param>
    /// <param name="parameterName"> The name of the parameter. </param>
    public ConventionUpdateModificationStoredProcedureConfiguration Parameter(
      PropertyInfo propertyInfo,
      string parameterName)
    {
      Check.NotEmpty(parameterName, nameof (parameterName));
      if (propertyInfo != (PropertyInfo) null)
        this.Configuration.Parameter(new PropertyPath(propertyInfo), parameterName, (string) null, false);
      return this;
    }

    /// <summary>Configures a parameter for this stored procedure.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="propertyName"> The name of the property to configure the parameter for. </param>
    /// <param name="currentValueParameterName">The current value parameter name.</param>
    /// <param name="originalValueParameterName">The original value parameter name.</param>
    public ConventionUpdateModificationStoredProcedureConfiguration Parameter(
      string propertyName,
      string currentValueParameterName,
      string originalValueParameterName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      Check.NotEmpty(currentValueParameterName, nameof (currentValueParameterName));
      Check.NotEmpty(originalValueParameterName, nameof (originalValueParameterName));
      return this.Parameter(this._type.GetAnyProperty(propertyName), currentValueParameterName, originalValueParameterName);
    }

    /// <summary>Configures a parameter for this stored procedure.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="propertyInfo"> The property to configure the parameter for. </param>
    /// <param name="currentValueParameterName">The current value parameter name.</param>
    /// <param name="originalValueParameterName">The original value parameter name.</param>
    public ConventionUpdateModificationStoredProcedureConfiguration Parameter(
      PropertyInfo propertyInfo,
      string currentValueParameterName,
      string originalValueParameterName)
    {
      Check.NotEmpty(currentValueParameterName, nameof (currentValueParameterName));
      Check.NotEmpty(originalValueParameterName, nameof (originalValueParameterName));
      if (propertyInfo != (PropertyInfo) null)
        this.Configuration.Parameter(new PropertyPath(propertyInfo), currentValueParameterName, originalValueParameterName, false);
      return this;
    }

    /// <summary>
    /// Configures a column of the result for this stored procedure to map to a property.
    /// This is used for database generated columns.
    /// </summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="propertyName"> The name of the property to configure the result for. </param>
    /// <param name="columnName">The name of the result column.</param>
    public ConventionUpdateModificationStoredProcedureConfiguration Result(
      string propertyName,
      string columnName)
    {
      Check.NotEmpty(propertyName, nameof (propertyName));
      Check.NotEmpty(columnName, nameof (columnName));
      this.Configuration.Result(new PropertyPath(this._type.GetAnyProperty(propertyName)), columnName);
      return this;
    }

    /// <summary>
    /// Configures a column of the result for this stored procedure to map to a property.
    /// This is used for database generated columns.
    /// </summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="propertyInfo"> The property to configure the result for. </param>
    /// <param name="columnName">The name of the result column.</param>
    public ConventionUpdateModificationStoredProcedureConfiguration Result(
      PropertyInfo propertyInfo,
      string columnName)
    {
      Check.NotNull<PropertyInfo>(propertyInfo, nameof (propertyInfo));
      Check.NotEmpty(columnName, nameof (columnName));
      this.Configuration.Result(new PropertyPath(propertyInfo), columnName);
      return this;
    }

    /// <summary>Configures the output parameter that returns the rows affected by this stored procedure.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="parameterName">The name of the parameter.</param>
    public ConventionUpdateModificationStoredProcedureConfiguration RowsAffectedParameter(
      string parameterName)
    {
      Check.NotEmpty(parameterName, nameof (parameterName));
      this.Configuration.RowsAffectedParameter(parameterName);
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
