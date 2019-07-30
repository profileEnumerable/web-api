// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConventionModificationStoredProceduresConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Creates a convention that configures stored procedures to be used to modify entities in the database.
  /// </summary>
  public class ConventionModificationStoredProceduresConfiguration
  {
    private readonly ModificationStoredProceduresConfiguration _configuration = new ModificationStoredProceduresConfiguration();
    private readonly Type _type;

    internal ConventionModificationStoredProceduresConfiguration(Type type)
    {
      this._type = type;
    }

    internal ModificationStoredProceduresConfiguration Configuration
    {
      get
      {
        return this._configuration;
      }
    }

    /// <summary>Configures stored procedure used to insert entities.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="modificationStoredProcedureConfigurationAction">A lambda expression that performs configuration for the stored procedure.</param>
    public ConventionModificationStoredProceduresConfiguration Insert(
      Action<ConventionInsertModificationStoredProcedureConfiguration> modificationStoredProcedureConfigurationAction)
    {
      Check.NotNull<Action<ConventionInsertModificationStoredProcedureConfiguration>>(modificationStoredProcedureConfigurationAction, nameof (modificationStoredProcedureConfigurationAction));
      ConventionInsertModificationStoredProcedureConfiguration procedureConfiguration = new ConventionInsertModificationStoredProcedureConfiguration(this._type);
      modificationStoredProcedureConfigurationAction(procedureConfiguration);
      this._configuration.Insert(procedureConfiguration.Configuration);
      return this;
    }

    /// <summary>Configures stored procedure used to update entities.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="modificationStoredProcedureConfigurationAction">A lambda expression that performs configuration for the stored procedure.</param>
    public ConventionModificationStoredProceduresConfiguration Update(
      Action<ConventionUpdateModificationStoredProcedureConfiguration> modificationStoredProcedureConfigurationAction)
    {
      Check.NotNull<Action<ConventionUpdateModificationStoredProcedureConfiguration>>(modificationStoredProcedureConfigurationAction, nameof (modificationStoredProcedureConfigurationAction));
      ConventionUpdateModificationStoredProcedureConfiguration procedureConfiguration = new ConventionUpdateModificationStoredProcedureConfiguration(this._type);
      modificationStoredProcedureConfigurationAction(procedureConfiguration);
      this._configuration.Update(procedureConfiguration.Configuration);
      return this;
    }

    /// <summary>Configures stored procedure used to delete entities.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="modificationStoredProcedureConfigurationAction">A lambda expression that performs configuration for the stored procedure.</param>
    public ConventionModificationStoredProceduresConfiguration Delete(
      Action<ConventionDeleteModificationStoredProcedureConfiguration> modificationStoredProcedureConfigurationAction)
    {
      Check.NotNull<Action<ConventionDeleteModificationStoredProcedureConfiguration>>(modificationStoredProcedureConfigurationAction, nameof (modificationStoredProcedureConfigurationAction));
      ConventionDeleteModificationStoredProcedureConfiguration procedureConfiguration = new ConventionDeleteModificationStoredProcedureConfiguration(this._type);
      modificationStoredProcedureConfigurationAction(procedureConfiguration);
      this._configuration.Delete(procedureConfiguration.Configuration);
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
