// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Design.AppConfigReader
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Internal.ConfigFile;
using System.Data.Entity.Utilities;
using System.Linq;

namespace System.Data.Entity.Infrastructure.Design
{
  /// <summary>
  /// Provides utility methods for reading from an App.config or Web.config file.
  /// </summary>
  public class AppConfigReader
  {
    private readonly System.Configuration.Configuration _configuration;

    /// <summary>
    /// Initializes a new instance of <see cref="T:System.Data.Entity.Infrastructure.Design.AppConfigReader" />.
    /// </summary>
    /// <param name="configuration">The configuration to read from.</param>
    public AppConfigReader(System.Configuration.Configuration configuration)
    {
      Check.NotNull<System.Configuration.Configuration>(configuration, nameof (configuration));
      this._configuration = configuration;
    }

    /// <summary>
    /// Gets the specified provider services from the configuration.
    /// </summary>
    /// <param name="invariantName">The invariant name of the provider services.</param>
    /// <returns>The provider services type name, or null if not found.</returns>
    public string GetProviderServices(string invariantName)
    {
      return ((EntityFrameworkSection) this._configuration.GetSection("entityFramework")).Providers.Cast<ProviderElement>().Where<ProviderElement>((Func<ProviderElement, bool>) (p => p.InvariantName == invariantName)).Select<ProviderElement, string>((Func<ProviderElement, string>) (p => p.ProviderTypeName)).FirstOrDefault<string>();
    }
  }
}
