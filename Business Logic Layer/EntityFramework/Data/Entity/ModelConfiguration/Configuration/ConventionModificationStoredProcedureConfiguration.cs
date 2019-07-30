// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.ConventionModificationStoredProcedureConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Creates a convention that configures stored procedures to be used to modify entities in the database.
  /// </summary>
  public abstract class ConventionModificationStoredProcedureConfiguration
  {
    private readonly ModificationStoredProcedureConfiguration _configuration = new ModificationStoredProcedureConfiguration();

    internal ConventionModificationStoredProcedureConfiguration()
    {
    }

    internal ModificationStoredProcedureConfiguration Configuration
    {
      get
      {
        return this._configuration;
      }
    }
  }
}
