// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlTypesAssemblyLoader
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Collections.Generic;
using System.Data.Entity.SqlServer.Resources;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.SqlServer
{
  internal class SqlTypesAssemblyLoader
  {
    private static readonly SqlTypesAssemblyLoader _instance = new SqlTypesAssemblyLoader((IEnumerable<string>) null);
    private readonly IEnumerable<string> _preferredSqlTypesAssemblies;
    private readonly Lazy<SqlTypesAssembly> _latestVersion;

    public static SqlTypesAssemblyLoader DefaultInstance
    {
      get
      {
        return SqlTypesAssemblyLoader._instance;
      }
    }

    public SqlTypesAssemblyLoader(IEnumerable<string> assemblyNames = null)
    {
      IEnumerable<string> strings = assemblyNames;
      if (strings == null)
        strings = (IEnumerable<string>) new string[2]
        {
          "Microsoft.SqlServer.Types, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91",
          "Microsoft.SqlServer.Types, Version=10.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
        };
      this._preferredSqlTypesAssemblies = strings;
      this._latestVersion = new Lazy<SqlTypesAssembly>(new Func<SqlTypesAssembly>(this.BindToLatest), true);
    }

    public SqlTypesAssemblyLoader(SqlTypesAssembly assembly)
    {
      this._latestVersion = new Lazy<SqlTypesAssembly>((Func<SqlTypesAssembly>) (() => assembly), true);
    }

    public virtual SqlTypesAssembly TryGetSqlTypesAssembly()
    {
      return this._latestVersion.Value;
    }

    public virtual SqlTypesAssembly GetSqlTypesAssembly()
    {
      SqlTypesAssembly sqlTypesAssembly = this._latestVersion.Value;
      if (sqlTypesAssembly == null)
        throw new InvalidOperationException(Strings.SqlProvider_SqlTypesAssemblyNotFound);
      return sqlTypesAssembly;
    }

    public virtual bool TryGetSqlTypesAssembly(Assembly assembly, out SqlTypesAssembly sqlAssembly)
    {
      if (this.IsKnownAssembly(assembly))
      {
        sqlAssembly = new SqlTypesAssembly(assembly);
        return true;
      }
      sqlAssembly = (SqlTypesAssembly) null;
      return false;
    }

    private SqlTypesAssembly BindToLatest()
    {
      Assembly sqlSpatialAssembly = (Assembly) null;
      object obj;
      if (SqlProviderServices.SqlServerTypesAssemblyName == null)
        obj = (object) this._preferredSqlTypesAssemblies;
      else
        obj = (object) new string[1]
        {
          SqlProviderServices.SqlServerTypesAssemblyName
        };
      foreach (string assemblyName in (IEnumerable<string>) obj)
      {
        AssemblyName assemblyRef = new AssemblyName(assemblyName);
        try
        {
          sqlSpatialAssembly = Assembly.Load(assemblyRef);
          break;
        }
        catch (FileNotFoundException ex)
        {
        }
        catch (FileLoadException ex)
        {
        }
      }
      if (sqlSpatialAssembly != (Assembly) null)
        return new SqlTypesAssembly(sqlSpatialAssembly);
      return (SqlTypesAssembly) null;
    }

    private bool IsKnownAssembly(Assembly assembly)
    {
      foreach (string sqlTypesAssembly in this._preferredSqlTypesAssemblies)
      {
        if (SqlTypesAssemblyLoader.AssemblyNamesMatch(assembly.FullName, new AssemblyName(sqlTypesAssembly)))
          return true;
      }
      return false;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    private static bool AssemblyNamesMatch(
      string infoRowProviderAssemblyName,
      AssemblyName targetAssemblyName)
    {
      if (string.IsNullOrWhiteSpace(infoRowProviderAssemblyName))
        return false;
      AssemblyName assemblyName;
      try
      {
        assemblyName = new AssemblyName(infoRowProviderAssemblyName);
      }
      catch (Exception ex)
      {
        return false;
      }
      if (!string.Equals(targetAssemblyName.Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase) || targetAssemblyName.Version == (Version) null || (assemblyName.Version == (Version) null || targetAssemblyName.Version.Major != assemblyName.Version.Major) || targetAssemblyName.Version.Minor != assemblyName.Version.Minor)
        return false;
      byte[] publicKeyToken = targetAssemblyName.GetPublicKeyToken();
      if (publicKeyToken != null)
        return ((IEnumerable<byte>) publicKeyToken).SequenceEqual<byte>((IEnumerable<byte>) assemblyName.GetPublicKeyToken());
      return false;
    }
  }
}
