// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.PropertyConventionWithHavingConfiguration`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Allows configuration to be performed for a lightweight convention based on
  /// the properties of entity types in a model and a captured value.
  /// </summary>
  /// <typeparam name="T">The type of the captured value.</typeparam>
  public class PropertyConventionWithHavingConfiguration<T> where T : class
  {
    private readonly ConventionsConfiguration _conventionsConfiguration;
    private readonly IEnumerable<Func<PropertyInfo, bool>> _predicates;
    private readonly Func<PropertyInfo, T> _capturingPredicate;

    internal PropertyConventionWithHavingConfiguration(
      ConventionsConfiguration conventionsConfiguration,
      IEnumerable<Func<PropertyInfo, bool>> predicates,
      Func<PropertyInfo, T> capturingPredicate)
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

    internal IEnumerable<Func<PropertyInfo, bool>> Predicates
    {
      get
      {
        return this._predicates;
      }
    }

    internal Func<PropertyInfo, T> CapturingPredicate
    {
      get
      {
        return this._capturingPredicate;
      }
    }

    /// <summary>
    /// Allows configuration of the properties that this convention applies to.
    /// </summary>
    /// <param name="propertyConfigurationAction">
    /// An action that performs configuration against a <see cref="T:System.Data.Entity.ModelConfiguration.Configuration.ConventionPrimitivePropertyConfiguration" />
    /// using a captured value.
    /// </param>
    public void Configure(
      Action<ConventionPrimitivePropertyConfiguration, T> propertyConfigurationAction)
    {
      Check.NotNull<Action<ConventionPrimitivePropertyConfiguration, T>>(propertyConfigurationAction, nameof (propertyConfigurationAction));
      this._conventionsConfiguration.Add((IConvention) new PropertyConventionWithHaving<T>(this._predicates, this._capturingPredicate, propertyConfigurationAction));
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
