// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.History.HistoryContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace System.Data.Entity.Migrations.History
{
  /// <summary>
  /// This class is used by Code First Migrations to read and write migration history
  /// from the database.
  /// To customize the definition of the migrations history table you can derive from
  /// this class and override OnModelCreating. Derived instances can either be registered
  /// on a per migrations configuration basis using <see cref="M:System.Data.Entity.Migrations.DbMigrationsConfiguration.SetHistoryContextFactory(System.String,System.Func{System.Data.Common.DbConnection,System.String,System.Data.Entity.Migrations.History.HistoryContext})" />,
  /// or globally using <see cref="M:System.Data.Entity.DbConfiguration.SetDefaultHistoryContext(System.Func{System.Data.Common.DbConnection,System.String,System.Data.Entity.Migrations.History.HistoryContext})" />.
  /// </summary>
  public class HistoryContext : DbContext, IDbModelCacheKeyProvider
  {
    internal static readonly Func<DbConnection, string, HistoryContext> DefaultFactory = (Func<DbConnection, string, HistoryContext>) ((e, d) => new HistoryContext(e, d));
    /// <summary>
    /// The default name used for the migrations history table.
    /// </summary>
    public const string DefaultTableName = "__MigrationHistory";
    internal const int ContextKeyMaxLength = 300;
    internal const int MigrationIdMaxLength = 150;
    private readonly string _defaultSchema;

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    internal HistoryContext()
    {
      this.InternalContext.InitializerDisabled = true;
    }

    /// <summary>
    /// Initializes a new instance of the HistoryContext class.
    /// If you are creating a derived history context you will generally expose a constructor
    /// that accepts these same parameters and passes them to this base constructor.
    /// </summary>
    /// <param name="existingConnection">
    /// An existing connection to use for the new context.
    /// </param>
    /// <param name="defaultSchema">
    /// The default schema of the model being migrated.
    /// This schema will be used for the migrations history table unless a different schema is configured in OnModelCreating.
    /// </param>
    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public HistoryContext(DbConnection existingConnection, string defaultSchema)
      : base(existingConnection, false)
    {
      this._defaultSchema = defaultSchema;
      this.Configuration.ValidateOnSaveEnabled = false;
      this.InternalContext.InitializerDisabled = true;
    }

    /// <summary>
    /// Gets the key used to locate a model that was previously built for this context. This is used
    /// to avoid processing OnModelCreating and calculating the model every time a new context instance is created.
    /// By default this property returns the default schema.
    /// In most cases you will not need to override this property. However, if your implementation of OnModelCreating
    /// contains conditional logic that results in a different model being built for the same database provider and
    /// default schema you should override this property and calculate an appropriate key.
    /// </summary>
    public virtual string CacheKey
    {
      get
      {
        return this._defaultSchema;
      }
    }

    /// <summary>
    /// Gets the default schema of the model being migrated.
    /// This schema will be used for the migrations history table unless a different schema is configured in OnModelCreating.
    /// </summary>
    protected string DefaultSchema
    {
      get
      {
        return this._defaultSchema;
      }
    }

    /// <summary>
    /// Gets or sets a <see cref="T:System.Data.Entity.DbSet`1" /> that can be used to read and write <see cref="T:System.Data.Entity.Migrations.History.HistoryRow" /> instances.
    /// </summary>
    public virtual IDbSet<HistoryRow> History { get; set; }

    /// <summary>
    /// Applies the default configuration for the migrations history table. If you override
    /// this method it is recommended that you call this base implementation before applying your
    /// custom configuration.
    /// </summary>
    /// <param name="modelBuilder"> The builder that defines the model for the context being created. </param>
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.HasDefaultSchema(this._defaultSchema);
      modelBuilder.Entity<HistoryRow>().ToTable("__MigrationHistory");
      modelBuilder.Entity<HistoryRow>().HasKey(h => new
      {
        MigrationId = h.MigrationId,
        ContextKey = h.ContextKey
      });
      modelBuilder.Entity<HistoryRow>().Property((Expression<Func<HistoryRow, string>>) (h => h.MigrationId)).HasMaxLength(new int?(150)).IsRequired();
      modelBuilder.Entity<HistoryRow>().Property((Expression<Func<HistoryRow, string>>) (h => h.ContextKey)).HasMaxLength(new int?(300)).IsRequired();
      modelBuilder.Entity<HistoryRow>().Property((Expression<Func<HistoryRow, byte[]>>) (h => h.Model)).IsRequired().IsMaxLength();
      modelBuilder.Entity<HistoryRow>().Property((Expression<Func<HistoryRow, string>>) (h => h.ProductVersion)).HasMaxLength(new int?(32)).IsRequired();
    }
  }
}
