// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.TransactionContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// This class is used by <see cref="T:System.Data.Entity.Infrastructure.CommitFailureHandler" /> to write and read transaction tracing information
  /// from the database.
  /// To customize the definition of the transaction table you can derive from
  /// this class and override <see cref="M:System.Data.Entity.Infrastructure.TransactionContext.OnModelCreating(System.Data.Entity.DbModelBuilder)" />. Derived classes can be registered
  /// using <see cref="T:System.Data.Entity.DbConfiguration" />.
  /// </summary>
  /// <remarks>
  /// By default EF will poll the resolved <see cref="T:System.Data.Entity.Infrastructure.TransactionContext" /> to check wether the database schema is compatible and
  /// will try to modify it accordingly if it's not. To disable this check call
  /// <code>Database.SetInitializer&lt;TTransactionContext&gt;(null)</code> where TTransactionContext is the type of the resolved context.
  /// </remarks>
  public class TransactionContext : DbContext
  {
    private const string _defaultTableName = "__TransactionHistory";

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Data.Entity.Infrastructure.TransactionContext" /> class.
    /// </summary>
    /// <param name="existingConnection">The connection used by the context for which the transactions will be recorded.</param>
    public TransactionContext(DbConnection existingConnection)
      : base(existingConnection, false)
    {
      this.Configuration.ValidateOnSaveEnabled = false;
    }

    /// <summary>
    /// Gets or sets a <see cref="T:System.Data.Entity.DbSet`1" /> that can be used to read and write <see cref="T:System.Data.Entity.Infrastructure.TransactionRow" /> instances.
    /// </summary>
    public virtual IDbSet<TransactionRow> Transactions { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Entity<TransactionRow>().ToTable("__TransactionHistory");
    }
  }
}
