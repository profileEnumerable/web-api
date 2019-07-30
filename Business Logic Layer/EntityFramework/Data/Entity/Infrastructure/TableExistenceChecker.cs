// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.TableExistenceChecker
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  ///     Implemented by Entity Framework providers and used to check whether or not tables exist
  ///     in a given database. This is used by database initializers when determining whether or not to
  ///     treat an existing database as empty such that tables should be created.
  /// </summary>
  public abstract class TableExistenceChecker
  {
    /// <summary>
    ///     When overridden in a derived class checks where the given tables exist in the database
    ///     for the given connection.
    /// </summary>
    /// <param name="context">
    ///     The context for which table checking is being performed, usually used to obtain an appropriate
    ///     <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterceptionContext" />.
    /// </param>
    /// <param name="connection">
    ///     A connection to the database. May be open or closed; should be closed again if opened. Do not
    ///     dispose.
    /// </param>
    /// <param name="modelTables">The tables to check for existence.</param>
    /// <param name="edmMetadataContextTableName">The name of the EdmMetadata table to check for existence.</param>
    /// <returns>True if any of the model tables or EdmMetadata table exists.</returns>
    public abstract bool AnyModelTableExistsInDatabase(
      ObjectContext context,
      DbConnection connection,
      IEnumerable<EntitySet> modelTables,
      string edmMetadataContextTableName);

    /// <summary>
    ///     Helper method to get the table name for the given s-space <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" />.
    /// </summary>
    /// <param name="modelTable">The s-space entity set for the table.</param>
    /// <returns>The table name.</returns>
    protected virtual string GetTableName(EntitySet modelTable)
    {
      if (!modelTable.MetadataProperties.Contains("Table") || modelTable.MetadataProperties["Table"].Value == null)
        return modelTable.Name;
      return (string) modelTable.MetadataProperties["Table"].Value;
    }
  }
}
