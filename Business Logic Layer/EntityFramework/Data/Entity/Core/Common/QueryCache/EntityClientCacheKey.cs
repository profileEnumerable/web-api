// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.QueryCache.EntityClientCacheKey
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.Internal;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text;

namespace System.Data.Entity.Core.Common.QueryCache
{
  internal sealed class EntityClientCacheKey : QueryCacheKey
  {
    private readonly CommandType _commandType;
    private readonly string _eSqlStatement;
    private readonly string _parametersToken;
    private readonly int _parameterCount;
    private readonly int _hashCode;

    internal EntityClientCacheKey(EntityCommand entityCommand)
    {
      this._commandType = entityCommand.CommandType;
      this._eSqlStatement = entityCommand.CommandText;
      this._parametersToken = EntityClientCacheKey.GetParametersToken(entityCommand);
      this._parameterCount = entityCommand.Parameters.Count;
      this._hashCode = this._commandType.GetHashCode() ^ this._eSqlStatement.GetHashCode() ^ this._parametersToken.GetHashCode();
    }

    public override bool Equals(object otherObject)
    {
      if (typeof (EntityClientCacheKey) != otherObject.GetType())
        return false;
      EntityClientCacheKey entityClientCacheKey = (EntityClientCacheKey) otherObject;
      if (this._commandType == entityClientCacheKey._commandType && this._parameterCount == entityClientCacheKey._parameterCount && this.Equals(entityClientCacheKey._eSqlStatement, this._eSqlStatement))
        return this.Equals(entityClientCacheKey._parametersToken, this._parametersToken);
      return false;
    }

    public override int GetHashCode()
    {
      return this._hashCode;
    }

    private static string GetTypeUsageToken(TypeUsage type)
    {
      return !object.ReferenceEquals((object) type, (object) DbTypeMap.AnsiString) ? (!object.ReferenceEquals((object) type, (object) DbTypeMap.AnsiStringFixedLength) ? (!object.ReferenceEquals((object) type, (object) DbTypeMap.String) ? (!object.ReferenceEquals((object) type, (object) DbTypeMap.StringFixedLength) ? (!object.ReferenceEquals((object) type, (object) DbTypeMap.Xml) ? (!TypeSemantics.IsEnumerationType(type) ? type.EdmType.Name : type.EdmType.FullName) : "String") : "StringFixedLength") : "String") : "AnsiStringFixedLength") : "AnsiString";
    }

    private static string GetParametersToken(EntityCommand entityCommand)
    {
      if (entityCommand.Parameters == null || entityCommand.Parameters.Count == 0)
        return "@@0";
      Dictionary<string, TypeUsage> parameterTypeUsage = entityCommand.GetParameterTypeUsage();
      if (1 == parameterTypeUsage.Count)
        return "@@1:" + entityCommand.Parameters[0].ParameterName + ":" + EntityClientCacheKey.GetTypeUsageToken(parameterTypeUsage[entityCommand.Parameters[0].ParameterName]);
      StringBuilder stringBuilder = new StringBuilder(entityCommand.Parameters.Count * 20);
      stringBuilder.Append("@@");
      stringBuilder.Append(entityCommand.Parameters.Count);
      stringBuilder.Append(":");
      string str = "";
      foreach (KeyValuePair<string, TypeUsage> keyValuePair in parameterTypeUsage)
      {
        stringBuilder.Append(str);
        stringBuilder.Append(keyValuePair.Key);
        stringBuilder.Append(":");
        stringBuilder.Append(EntityClientCacheKey.GetTypeUsageToken(keyValuePair.Value));
        str = ";";
      }
      return stringBuilder.ToString();
    }

    public override string ToString()
    {
      return string.Join("|", Enum.GetName(typeof (CommandType), (object) this._commandType), this._eSqlStatement, this._parametersToken);
    }
  }
}
