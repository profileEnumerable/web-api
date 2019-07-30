// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.TypeConventionWithHavingConfiguration`2
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
  /// the entity types in a model that inherit from a common, specified type and a
  /// captured value.
  /// </summary>
  /// <typeparam name="T"> The common type of the entity types that this convention applies to. </typeparam>
  /// <typeparam name="TValue"> Type of the captured value. </typeparam>
  public class TypeConventionWithHavingConfiguration<T, TValue>
    where T : class
    where TValue : class
  {
    private readonly ConventionsConfiguration _conventionsConfiguration;
    private readonly IEnumerable<Func<Type, bool>> _predicates;
    private readonly Func<Type, TValue> _capturingPredicate;

    internal TypeConventionWithHavingConfiguration(
      ConventionsConfiguration conventionsConfiguration,
      IEnumerable<Func<Type, bool>> predicates,
      Func<Type, TValue> capturingPredicate)
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

    internal Func<Type, TValue> CapturingPredicate
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
    /// An action that performs configuration against a <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionTypeConfiguration`1" />
    /// using a captured value.
    /// </param>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public void Configure(
      Action<ConventionTypeConfiguration<T>, TValue> entityConfigurationAction)
    {
      Check.NotNull<Action<ConventionTypeConfiguration<T>, TValue>>(entityConfigurationAction, nameof (entityConfigurationAction));
      this._conventionsConfiguration.Add((IConvention) new TypeConventionWithHaving<T, TValue>(this._predicates, this._capturingPredicate, entityConfigurationAction));
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
