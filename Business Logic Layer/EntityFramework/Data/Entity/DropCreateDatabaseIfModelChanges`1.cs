// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.DropCreateDatabaseIfModelChanges`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Internal;
using System.Data.Entity.Utilities;

namespace System.Data.Entity
{
  /// <summary>
  /// An implementation of IDatabaseInitializer that will <b>DELETE</b>, recreate, and optionally re-seed the
  /// database only if the model has changed since the database was created.
  /// </summary>
  /// <typeparam name="TContext"> The type of the context. </typeparam>
  /// <remarks>
  /// Whether or not the model has changed is determined by the <see cref="M:System.Data.Entity.Database.CompatibleWithModel(System.Boolean)" />
  /// method.
  /// To seed the database create a derived class and override the Seed method.
  /// </remarks>
  public class DropCreateDatabaseIfModelChanges<TContext> : IDatabaseInitializer<TContext>
    where TContext : DbContext
  {
    static DropCreateDatabaseIfModelChanges()
    {
      DbConfigurationManager.Instance.EnsureLoadedForContext(typeof (TContext));
    }

    /// <summary>
    /// Executes the strategy to initialize the database for the given context.
    /// </summary>
    /// <param name="context"> The context. </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="context" />
    /// is
    /// <c>null</c>
    /// .
    /// </exception>
    public virtual void InitializeDatabase(TContext context)
    {
      Check.NotNull<TContext>(context, nameof (context));
      DatabaseExistenceState existenceState = new DatabaseTableChecker().AnyModelTableExists(context.InternalContext);
      if (existenceState == DatabaseExistenceState.Exists)
      {
        if (context.Database.CompatibleWithModel(true))
          return;
        context.Database.Delete();
        existenceState = DatabaseExistenceState.DoesNotExist;
      }
      context.Database.Create(existenceState);
      this.Seed(context);
      context.SaveChanges();
    }

    /// <summary>
    /// A method that should be overridden to actually add data to the context for seeding.
    /// The default implementation does nothing.
    /// </summary>
    /// <param name="context"> The context to seed. </param>
    protected virtual void Seed(TContext context)
    {
    }
  }
}
