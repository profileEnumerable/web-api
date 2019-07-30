// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.ModelConfiguration.Configuration.CascadableNavigationPropertyConfiguration
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.ModelConfiguration.Configuration.Properties.Navigation;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.ModelConfiguration.Configuration
{
  /// <summary>
  /// Configures a relationship that can support cascade on delete functionality.
  /// </summary>
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cascadable")]
  public abstract class CascadableNavigationPropertyConfiguration
  {
    private readonly NavigationPropertyConfiguration _navigationPropertyConfiguration;

    internal CascadableNavigationPropertyConfiguration(
      NavigationPropertyConfiguration navigationPropertyConfiguration)
    {
      this._navigationPropertyConfiguration = navigationPropertyConfiguration;
    }

    /// <summary>
    /// Configures cascade delete to be on for the relationship.
    /// </summary>
    public void WillCascadeOnDelete()
    {
      this.WillCascadeOnDelete(true);
    }

    /// <summary>
    /// Configures whether or not cascade delete is on for the relationship.
    /// </summary>
    /// <param name="value"> Value indicating if cascade delete is on or not. </param>
    public void WillCascadeOnDelete(bool value)
    {
      this._navigationPropertyConfiguration.DeleteAction = new OperationAction?(value ? OperationAction.Cascade : OperationAction.None);
    }

    internal NavigationPropertyConfiguration NavigationPropertyConfiguration
    {
      get
      {
        return this._navigationPropertyConfiguration;
      }
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
