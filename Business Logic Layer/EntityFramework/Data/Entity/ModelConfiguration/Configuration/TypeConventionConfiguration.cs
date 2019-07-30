// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.TypeConventionConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Allows configuration to be performed for a lightweight convention based on
  /// the entity types in a model.
  /// </summary>
  public class TypeConventionConfiguration
  {
    private readonly ConventionsConfiguration _conventionsConfiguration;
    private readonly IEnumerable<Func<Type, bool>> _predicates;

    internal TypeConventionConfiguration(ConventionsConfiguration conventionsConfiguration)
      : this(conventionsConfiguration, Enumerable.Empty<Func<Type, bool>>())
    {
    }

    private TypeConventionConfiguration(
      ConventionsConfiguration conventionsConfiguration,
      IEnumerable<Func<Type, bool>> predicates)
    {
      this._conventionsConfiguration = conventionsConfiguration;
      this._predicates = predicates;
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

    /// <summary>
    /// Filters the entity types that this convention applies to based on a
    /// predicate.
    /// </summary>
    /// <param name="predicate"> A function to test each entity type for a condition. </param>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.TypeConventionConfiguration" /> instance so that multiple calls can be chained.
    /// </returns>
    public TypeConventionConfiguration Where(Func<Type, bool> predicate)
    {
      Check.NotNull<Func<Type, bool>>(predicate, nameof (predicate));
      return new TypeConventionConfiguration(this._conventionsConfiguration, this._predicates.Append<Func<Type, bool>>(predicate));
    }

    /// <summary>
    /// Filters the entity types that this convention applies to based on a predicate
    /// while capturing a value to use later during configuration.
    /// </summary>
    /// <typeparam name="T"> Type of the captured value. </typeparam>
    /// <param name="capturingPredicate">
    /// A function to capture a value for each entity type. If the value is null, the
    /// entity type will be filtered out.
    /// </param>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.TypeConventionWithHavingConfiguration`1" /> instance so that multiple calls can be chained.
    /// </returns>
    public TypeConventionWithHavingConfiguration<T> Having<T>(
      Func<Type, T> capturingPredicate)
      where T : class
    {
      Check.NotNull<Func<Type, T>>(capturingPredicate, nameof (capturingPredicate));
      return new TypeConventionWithHavingConfiguration<T>(this._conventionsConfiguration, this._predicates, capturingPredicate);
    }

    /// <summary>
    /// Allows configuration of the entity types that this convention applies to.
    /// </summary>
    /// <param name="entityConfigurationAction">
    /// An action that performs configuration against a
    /// <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration" />
    /// .
    /// </param>
    public void Configure(
      Action<ConventionTypeConfiguration> entityConfigurationAction)
    {
      Check.NotNull<Action<ConventionTypeConfiguration>>(entityConfigurationAction, nameof (entityConfigurationAction));
      this._conventionsConfiguration.Add((IConvention) new TypeConvention(this._predicates, entityConfigurationAction));
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
