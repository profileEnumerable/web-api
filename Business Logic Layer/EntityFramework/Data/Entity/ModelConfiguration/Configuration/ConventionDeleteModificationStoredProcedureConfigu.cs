// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConventionDeleteModificationStoredProcedureConfiguration
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
  /// Creates a convention that configures stored procedures to be used to delete entities in the database.
  /// </summary>
  public class ConventionDeleteModificationStoredProcedureConfiguration : ConventionModificationStoredProcedureConfiguration
  {
    private readonly Type _type;

    internal ConventionDeleteModificationStoredProcedureConfiguration(Type type)
    {
      this._type = type;
    }

    /// <summary> Configures the name of the stored procedure. </summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="procedureName"> The stored procedure name. </param>
    public ConventionDeleteModificationStoredProcedureConfiguration HasName(
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
    public ConventionDeleteModificationStoredProcedureConfiguration HasName(
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
    /// <param name="parameterName">The name of the parameter.</param>
    public ConventionDeleteModificationStoredProcedureConfiguration Parameter(
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
    /// <param name="parameterName">The name of the parameter.</param>
    public ConventionDeleteModificationStoredProcedureConfiguration Parameter(
      PropertyInfo propertyInfo,
      string parameterName)
    {
      Check.NotEmpty(parameterName, nameof (parameterName));
      if (propertyInfo != (PropertyInfo) null)
        this.Configuration.Parameter(new PropertyPath(propertyInfo), parameterName, (string) null, false);
      return this;
    }

    /// <summary>Configures the output parameter that returns the rows affected by this stored procedure.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="parameterName">The name of the parameter.</param>
    public ConventionDeleteModificationStoredProcedureConfiguration RowsAffectedParameter(
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
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
