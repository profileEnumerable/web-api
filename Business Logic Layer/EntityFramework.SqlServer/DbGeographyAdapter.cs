// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.DbGeographyAdapter
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Data.Entity.Core;
using System.Data.Entity.Spatial;
using System.Data.Entity.SqlServer.Resources;
using System.Data.Entity.SqlServer.Utilities;

namespace System.Data.Entity.SqlServer
{
  internal class DbGeographyAdapter : IDbSpatialValue
  {
    private readonly DbGeography _value;

    internal DbGeographyAdapter(DbGeography value)
    {
      this._value = value;
    }

    public bool IsGeography
    {
      get
      {
        return true;
      }
    }

    public object ProviderValue
    {
      get
      {
        return ((Func<object>) (() => this._value.ProviderValue)).NullIfNotImplemented<object>();
      }
    }

    public int? CoordinateSystemId
    {
      get
      {
        return ((Func<int?>) (() => new int?(this._value.CoordinateSystemId))).NullIfNotImplemented<int?>();
      }
    }

    public string WellKnownText
    {
      get
      {
        return ((Func<string>) (() => this._value.Provider.AsTextIncludingElevationAndMeasure(this._value))).NullIfNotImplemented<string>() ?? ((Func<string>) (() => this._value.AsText())).NullIfNotImplemented<string>();
      }
    }

    public byte[] WellKnownBinary
    {
      get
      {
        return ((Func<byte[]>) (() => this._value.AsBinary())).NullIfNotImplemented<byte[]>();
      }
    }

    public string GmlString
    {
      get
      {
        return ((Func<string>) (() => this._value.AsGml())).NullIfNotImplemented<string>();
      }
    }

    public Exception NotSqlCompatible()
    {
      return (Exception) new ProviderIncompatibleException(Strings.SqlProvider_GeographyValueNotSqlCompatible);
    }
  }
}
