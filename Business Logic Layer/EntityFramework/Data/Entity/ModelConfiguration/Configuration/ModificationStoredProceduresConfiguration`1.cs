// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ModificationStoredProceduresConfiguration`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Allows configuration to be performed for a stored procedure that is used to modify entities.
  /// </summary>
  /// <typeparam name="TEntityType">The type of the entity that the stored procedure can be used to modify.</typeparam>
  public class ModificationStoredProceduresConfiguration<TEntityType> where TEntityType : class
  {
    private readonly ModificationStoredProceduresConfiguration _configuration = new ModificationStoredProceduresConfiguration();

    internal ModificationStoredProceduresConfiguration()
    {
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
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ModificationStoredProceduresConfiguration<TEntityType> Insert(
      Action<InsertModificationStoredProcedureConfiguration<TEntityType>> modificationStoredProcedureConfigurationAction)
    {
      Check.NotNull<Action<InsertModificationStoredProcedureConfiguration<TEntityType>>>(modificationStoredProcedureConfigurationAction, nameof (modificationStoredProcedureConfigurationAction));
      InsertModificationStoredProcedureConfiguration<TEntityType> procedureConfiguration = new InsertModificationStoredProcedureConfiguration<TEntityType>();
      modificationStoredProcedureConfigurationAction(procedureConfiguration);
      this._configuration.Insert(procedureConfiguration.Configuration);
      return this;
    }

    /// <summary>Configures stored procedure used to update entities.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="modificationStoredProcedureConfigurationAction">A lambda expression that performs configuration for the stored procedure.</param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ModificationStoredProceduresConfiguration<TEntityType> Update(
      Action<UpdateModificationStoredProcedureConfiguration<TEntityType>> modificationStoredProcedureConfigurationAction)
    {
      Check.NotNull<Action<UpdateModificationStoredProcedureConfiguration<TEntityType>>>(modificationStoredProcedureConfigurationAction, nameof (modificationStoredProcedureConfigurationAction));
      UpdateModificationStoredProcedureConfiguration<TEntityType> procedureConfiguration = new UpdateModificationStoredProcedureConfiguration<TEntityType>();
      modificationStoredProcedureConfigurationAction(procedureConfiguration);
      this._configuration.Update(procedureConfiguration.Configuration);
      return this;
    }

    /// <summary>Configures stored procedure used to delete entities.</summary>
    /// <returns> The same configuration instance so that multiple calls can be chained. </returns>
    /// <param name="modificationStoredProcedureConfigurationAction">A lambda expression that performs configuration for the stored procedure.</param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public ModificationStoredProceduresConfiguration<TEntityType> Delete(
      Action<DeleteModificationStoredProcedureConfiguration<TEntityType>> modificationStoredProcedureConfigurationAction)
    {
      Check.NotNull<Action<DeleteModificationStoredProcedureConfiguration<TEntityType>>>(modificationStoredProcedureConfigurationAction, nameof (modificationStoredProcedureConfigurationAction));
      DeleteModificationStoredProcedureConfiguration<TEntityType> procedureConfiguration = new DeleteModificationStoredProcedureConfiguration<TEntityType>();
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
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new Type GetType()
    {
      return base.GetType();
    }
  }
}
