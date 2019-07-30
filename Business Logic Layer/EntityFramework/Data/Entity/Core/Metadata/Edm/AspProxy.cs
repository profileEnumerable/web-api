// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.AspProxy
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.SchemaObjectModel;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class AspProxy
  {
    private static readonly byte[] _systemWebPublicKeyToken = ScalarType.ConvertToByteArray("b03f5f7f11d50a3a");
    private const string BUILD_MANAGER_TYPE_NAME = "System.Web.Compilation.BuildManager";
    private const string AspNetAssemblyName = "System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
    private Assembly _webAssembly;
    private bool _triedLoadingWebAssembly;

    internal bool IsAspNetEnvironment()
    {
      if (!this.TryInitializeWebAssembly())
        return false;
      try
      {
        return this.InternalMapWebPath("~") != null;
      }
      catch (SecurityException ex)
      {
        return false;
      }
      catch (Exception ex)
      {
        if (ex.IsCatchableExceptionType())
          return false;
        throw;
      }
    }

    public bool TryInitializeWebAssembly()
    {
      if (this._webAssembly != (Assembly) null)
        return true;
      if (this._triedLoadingWebAssembly)
        return false;
      this._triedLoadingWebAssembly = true;
      if (!AspProxy.IsSystemWebLoaded())
        return false;
      try
      {
        this._webAssembly = Assembly.Load("System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
        return this._webAssembly != (Assembly) null;
      }
      catch (Exception ex)
      {
        if (!ex.IsCatchableExceptionType())
          throw;
      }
      return false;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public static bool IsSystemWebLoaded()
    {
      try
      {
        return ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).Any<Assembly>((Func<Assembly, bool>) (a =>
        {
          if (a.GetName().Name == "System.Web" && a.GetName().GetPublicKeyToken() != null)
            return ((IEnumerable<byte>) a.GetName().GetPublicKeyToken()).SequenceEqual<byte>((IEnumerable<byte>) AspProxy._systemWebPublicKeyToken);
          return false;
        }));
      }
      catch
      {
      }
      return false;
    }

    private void InitializeWebAssembly()
    {
      if (!this.TryInitializeWebAssembly())
        throw new InvalidOperationException(Strings.UnableToDetermineApplicationContext);
    }

    internal string MapWebPath(string path)
    {
      path = this.InternalMapWebPath(path);
      if (path == null)
        throw new InvalidOperationException(Strings.InvalidUseOfWebPath((object) "~"));
      return path;
    }

    internal string InternalMapWebPath(string path)
    {
      this.InitializeWebAssembly();
      try
      {
        return (string) this._webAssembly.GetType("System.Web.Hosting.HostingEnvironment", true).GetDeclaredMethod("MapPath", typeof (string)).Invoke((object) null, new object[1]
        {
          (object) path
        });
      }
      catch (TargetException ex)
      {
        throw new InvalidOperationException(Strings.UnableToDetermineApplicationContext, (Exception) ex);
      }
      catch (ArgumentException ex)
      {
        throw new InvalidOperationException(Strings.UnableToDetermineApplicationContext, (Exception) ex);
      }
      catch (TargetInvocationException ex)
      {
        throw new InvalidOperationException(Strings.UnableToDetermineApplicationContext, (Exception) ex);
      }
      catch (TargetParameterCountException ex)
      {
        throw new InvalidOperationException(Strings.UnableToDetermineApplicationContext, (Exception) ex);
      }
      catch (MethodAccessException ex)
      {
        throw new InvalidOperationException(Strings.UnableToDetermineApplicationContext, (Exception) ex);
      }
      catch (MemberAccessException ex)
      {
        throw new InvalidOperationException(Strings.UnableToDetermineApplicationContext, (Exception) ex);
      }
      catch (TypeLoadException ex)
      {
        throw new InvalidOperationException(Strings.UnableToDetermineApplicationContext, (Exception) ex);
      }
    }

    internal bool HasBuildManagerType()
    {
      Type buildManager;
      return this.TryGetBuildManagerType(out buildManager);
    }

    private bool TryGetBuildManagerType(out Type buildManager)
    {
      this.InitializeWebAssembly();
      buildManager = this._webAssembly.GetType("System.Web.Compilation.BuildManager", false);
      return buildManager != (Type) null;
    }

    internal IEnumerable<Assembly> GetBuildManagerReferencedAssemblies()
    {
      MethodInfo assembliesMethod = this.GetReferencedAssembliesMethod();
      if (assembliesMethod == (MethodInfo) null)
        return (IEnumerable<Assembly>) new List<Assembly>();
      try
      {
        ICollection source = (ICollection) assembliesMethod.Invoke((object) null, (object[]) null);
        if (source == null)
          return (IEnumerable<Assembly>) new List<Assembly>();
        return source.Cast<Assembly>();
      }
      catch (TargetException ex)
      {
        throw new InvalidOperationException(Strings.UnableToDetermineApplicationContext, (Exception) ex);
      }
      catch (TargetInvocationException ex)
      {
        throw new InvalidOperationException(Strings.UnableToDetermineApplicationContext, (Exception) ex);
      }
      catch (MethodAccessException ex)
      {
        throw new InvalidOperationException(Strings.UnableToDetermineApplicationContext, (Exception) ex);
      }
    }

    internal MethodInfo GetReferencedAssembliesMethod()
    {
      Type buildManager;
      if (!this.TryGetBuildManagerType(out buildManager))
        throw new InvalidOperationException(Strings.UnableToFindReflectedType((object) "System.Web.Compilation.BuildManager", (object) "System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));
      return buildManager.GetDeclaredMethod("GetReferencedAssemblies");
    }
  }
}
