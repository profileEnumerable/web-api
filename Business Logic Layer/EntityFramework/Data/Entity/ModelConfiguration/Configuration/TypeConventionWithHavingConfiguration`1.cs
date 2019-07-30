// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.TypeConventionWithHavingConfiguration`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Allows configuration to be performed for a lightweight convention based on
  /// the entity types in a model and a captured value.
  /// </summary>
  /// <typeparam name="T"> Type of the captured value. </typeparam>
  public class TypeConventionWithHavingConfiguration<T> where T : class
  {
    private readonly ConventionsConfiguration _conventionsConfiguration;
    private readonly IEnumerable<Func<Type, bool>> _predicates;
    private readonly Func<Type, T> _capturingPredicate;

    internal TypeConventionWithHavingConfiguration(
      ConventionsConfiguration conventionsConfiguration,
      IEnumerable<Func<Type, bool>> predicates,
      Func<Type, T> capturingPredicate)
    {
      this._conventionsConfiguration = conventionsConfiguration;
      this._predicates = predicates;
      this._capturingPredicate = capturingPredicate;
    }

    internal ConventionsConfiguration ConventionsConfiguration
    {
      get
      {
        return this._conventionsConfiguration;
      }
    }

    internal IEnumerable<Func<Type, bool>> Predicates
    {
      get
      {
        return this._predicates;
      }
    }

    internal Func<Type, T> CapturingPredicate
    {
      get
      {
        return this._capturingPredicate;
      }
    }

    /// <summary>
    /// Allows configuration of the entity types that this convention applies to.
    /// </summary>
    /// <param name="entityConfigurationAction">
    /// An action that performs configuration against a <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" />
    /// using a captured value.
    /// </param>
    public void Configure(
      Action<ConventionTypeConfiguration, T> entityConfigurationAction)
    {
      Check.NotNull<Action<ConventionTypeConfiguration, T>>(entityConfigurationAction, nameof (entityConfigurationAction));
      this._conventionsConfiguration.Add((IConvention) new TypeConventionWithHaving<T>(this._predicates, this._capturingPredicate, entityConfigurationAction));
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
