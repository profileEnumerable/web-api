// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.DbContextConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Internal;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure
{
  /// <summary>
  /// Returned by the Configuration method of <see cref="T:System.Data.Entity.DbContext" /> to provide access to configuration
  /// options for the context.
  /// </summary>
  public class DbContextConfiguration
  {
    private readonly InternalContext _internalContext;

    internal DbContextConfiguration(InternalContext internalContext)
    {
      this._internalContext = internalContext;
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

    /// <summary>
    /// Gets or sets the value that determines whether SQL functions and commands should be always executed in a transaction.
    /// </summary>
    /// <remarks>
    /// This flag determines whether a new transaction will be started when methods such as <see cref="M:System.Data.Entity.Database.ExecuteSqlCommand(System.String,System.Object[])" />
    /// are executed outside of a transaction.
    /// Note that this does not change the behavior of <see cref="M:System.Data.Entity.DbContext.SaveChanges" />.
    /// </remarks>
    /// <value>The default transactional behavior.</value>
    public bool EnsureTransactionsForFunctionsAndCommands
    {
      get
      {
        return this._internalContext.EnsureTransactionsForFunctionsAndCommands;
      }
      set
      {
        this._internalContext.EnsureTransactionsForFunctionsAndCommands = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether lazy loading of relationships exposed as
    /// navigation properties is enabled.  Lazy loading is enabled by default.
    /// </summary>
    /// <value>
    /// <c>true</c> if lazy loading is enabled; otherwise, <c>false</c> .
    /// </value>
    public bool LazyLoadingEnabled
    {
      get
      {
        return this._internalContext.LazyLoadingEnabled;
      }
      set
      {
        this._internalContext.LazyLoadingEnabled = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the framework will create instances of
    /// dynamically generated proxy classes whenever it creates an instance of an entity type.
    /// Note that even if proxy creation is enabled with this flag, proxy instances will only
    /// be created for entity types that meet the requirements for being proxied.
    /// Proxy creation is enabled by default.
    /// </summary>
    /// <value>
    /// <c>true</c> if proxy creation is enabled; otherwise, <c>false</c> .
    /// </value>
    public bool ProxyCreationEnabled
    {
      get
      {
        return this._internalContext.ProxyCreationEnabled;
      }
      set
      {
        this._internalContext.ProxyCreationEnabled = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether database null semantics are exhibited when comparing
    /// two operands, both of which are potentially nullable. The default value is false.
    /// 
    /// For example (operand1 == operand2) will be translated as:
    /// 
    /// (operand1 = operand2)
    /// 
    /// if UseDatabaseNullSemantics is true, respectively
    /// 
    /// (((operand1 = operand2) AND (NOT (operand1 IS NULL OR operand2 IS NULL))) OR ((operand1 IS NULL) AND (operand2 IS NULL)))
    /// 
    /// if UseDatabaseNullSemantics is false.
    /// </summary>
    /// <value>
    /// <c>true</c> if database null comparison behavior is enabled, otherwise <c>false</c> .
    /// </value>
    public bool UseDatabaseNullSemantics
    {
      get
      {
        return this._internalContext.UseDatabaseNullSemantics;
      }
      set
      {
        this._internalContext.UseDatabaseNullSemantics = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="M:System.Data.Entity.Infrastructure.DbChangeTracker.DetectChanges" />
    /// method is called automatically by methods of <see cref="T:System.Data.Entity.DbContext" /> and related classes.
    /// The default value is true.
    /// </summary>
    /// <value>
    /// <c>true</c> if should be called automatically; otherwise, <c>false</c>.
    /// </value>
    public bool AutoDetectChangesEnabled
    {
      get
      {
        return this._internalContext.AutoDetectChangesEnabled;
      }
      set
      {
        this._internalContext.AutoDetectChangesEnabled = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether tracked entities should be validated automatically when
    /// <see cref="M:System.Data.Entity.DbContext.SaveChanges" /> is invoked.
    /// The default value is true.
    /// </summary>
    public bool ValidateOnSaveEnabled
    {
      get
      {
        return this._internalContext.ValidateOnSaveEnabled;
      }
      set
      {
        this._internalContext.ValidateOnSaveEnabled = value;
      }
    }
  }
}
