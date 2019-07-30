// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Interception.IDbConfigurationInterceptor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure.DependencyResolution;

namespace System.Data.Entity.Infrastructure.Interception
{
  /// <summary>
  /// An object that implements this interface can be registered with <see cref="T:System.Data.Entity.Infrastructure.Interception.DbInterception" /> to
  /// receive notifications when Entity Framework loads the application's <see cref="T:System.Data.Entity.DbConfiguration" />.
  /// </summary>
  /// <remarks>
  /// Interceptors can also be registered in the config file of the application.
  /// See http://go.microsoft.com/fwlink/?LinkId=260883 for more information about Entity Framework configuration.
  /// </remarks>
  public interface IDbConfigurationInterceptor : IDbInterceptor
  {
    /// <summary>
    /// Occurs during EF initialization after the <see cref="T:System.Data.Entity.DbConfiguration" /> has been constructed but just before
    /// it is locked ready for use. Use this event to inspect and/or override services that have been
    /// registered before the configuration is locked. Note that an interceptor of this type should be used carefully
    /// since it may prevent tooling from discovering the same configuration that is used at runtime.
    /// </summary>
    /// <remarks>
    /// Handlers can only be added before EF starts to use the configuration and so handlers should
    /// generally be added as part of application initialization. Do not access the DbConfiguration
    /// static methods inside the handler; instead use the the members of <see cref="T:System.Data.Entity.Infrastructure.DependencyResolution.DbConfigurationLoadedEventArgs" />
    /// to get current services and/or add overrides.
    /// </remarks>
    /// <param name="loadedEventArgs">Arguments to the event that this interceptor mirrors.</param>
    /// <param name="interceptionContext">Contextual information about the event.</param>
    void Loaded(
      DbConfigurationLoadedEventArgs loadedEventArgs,
      DbConfigurationInterceptionContext interceptionContext);
  }
}
