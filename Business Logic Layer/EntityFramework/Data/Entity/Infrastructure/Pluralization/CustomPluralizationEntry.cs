// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Pluralization.CustomPluralizationEntry
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Infrastructure.Pluralization
{
  /// <summary>
  /// Represents a custom pluralization term to be used by the <see cref="T:System.Data.Entity.Infrastructure.Pluralization.EnglishPluralizationService" />
  /// </summary>
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pluralization")]
  public class CustomPluralizationEntry
  {
    /// <summary>Get the singular.</summary>
    public string Singular { get; private set; }

    /// <summary>Get the plural.</summary>
    public string Plural { get; private set; }

    /// <summary>Create a new instance</summary>
    /// <param name="singular">A non null or empty string representing the singular.</param>
    /// <param name="plural">A non null or empty string representing the plural.</param>
    public CustomPluralizationEntry(string singular, string plural)
    {
      Check.NotEmpty(singular, nameof (singular));
      Check.NotEmpty(plural, nameof (plural));
      this.Singular = singular;
      this.Plural = plural;
    }
  }
}
