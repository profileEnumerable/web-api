// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Infrastructure.Design.Executor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace System.Data.Entity.Infrastructure.Design
{
  internal class Executor : MarshalByRefObject
  {
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    private readonly Assembly _assembly;

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "anonymousArguments")]
    public Executor(string assemblyFile, IDictionary<string, object> anonymousArguments)
    {
      Check.NotEmpty(assemblyFile, nameof (assemblyFile));
      this._assembly = Assembly.Load(AssemblyName.GetAssemblyName(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyFile)));
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    internal virtual string GetProviderServicesInternal(string invariantName)
    {
      DbConfiguration.LoadConfiguration(this._assembly);
      IDbDependencyResolver dependencyResolver = DbConfiguration.DependencyResolver;
      DbProviderServices providerServices = (DbProviderServices) null;
      try
      {
        providerServices = dependencyResolver.GetService<DbProviderServices>((object) invariantName);
      }
      catch
      {
      }
      return providerServices?.GetType().AssemblyQualifiedName;
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
    public class GetProviderServices : MarshalByRefObject
    {
      [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "anonymousArguments")]
      public GetProviderServices(
        Executor executor,
        object handler,
        string invariantName,
        IDictionary<string, object> anonymousArguments)
      {
        Check.NotNull<Executor>(executor, nameof (executor));
        Check.NotNull<object>(handler, nameof (handler));
        Check.NotEmpty(invariantName, nameof (invariantName));
        new WrappedHandler(handler).SetResult((object) executor.GetProviderServicesInternal(invariantName));
      }
    }
  }
}
