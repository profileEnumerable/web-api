// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.TypeConventionConfiguration`1
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
  /// the entity types in a model that inherit from a common, specified type.
  /// </summary>
  /// <typeparam name="T"> The common type of the entity types that this convention applies to. </typeparam>
  public class TypeConventionConfiguration<T> where T : class
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
    /// An <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.TypeConventionConfiguration`1" /> instance so that multiple calls can be chained.
    /// </returns>
    public TypeConventionConfiguration<T> Where(
      Func<Type, bool> predicate)
    {
      Check.NotNull<Func<Type, bool>>(predicate, nameof (predicate));
      return new TypeConventionConfiguration<T>(this._conventionsConfiguration, this._predicates.Append<Func<Type, bool>>(predicate));
    }

    /// <summary>
    /// Filters the entity types that this convention applies to based on a predicate
    /// while capturing a value to use later during configuration.
    /// </summary>
    /// <typeparam name="TValue"> Type of the captured value. </typeparam>
    /// <param name="capturingPredicate">
    /// A function to capture a value for each entity type. If the value is null, the
    /// entity type will be filtered out.
    /// </param>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.TypeConventionWithHavingConfiguration`2" /> instance so that multiple calls can be chained.
    /// </returns>
    public TypeConventionWithHavingConfiguration<T, TValue> Having<TValue>(
      Func<Type, TValue> capturingPredicate)
      where TValue : class
    {
      Check.NotNull<Func<Type, TValue>>(capturingPredicate, nameof (capturingPredicate));
      return new TypeConventionWithHavingConfiguration<T, TValue>(this._conventionsConfiguration, this._predicates, capturingPredicate);
    }

    /// <summary>
    /// Allows configuration of the entity types that this convention applies to.
    /// </summary>
    /// <param name="entityConfigurationAction">
    /// An action that performs configuration against a
    /// <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration`1" />
    /// .
    /// </param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public void Configure(
      Action<ConventionTypeConfiguration<T>> entityConfigurationAction)
    {
      Check.NotNull<Action<ConventionTypeConfiguration<T>>>(entityConfigurationAction, nameof (entityConfigurationAction));
      this._conventionsConfiguration.Add((IConvention) new TypeConvention<T>(this._predicates, entityConfigurationAction));
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
