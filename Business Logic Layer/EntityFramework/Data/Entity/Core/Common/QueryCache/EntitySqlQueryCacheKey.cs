// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.QueryCache.EntitySqlQueryCacheKey
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Core.Common.QueryCache
{
  internal sealed class EntitySqlQueryCacheKey : QueryCacheKey
  {
    private readonly int _hashCode;
    private readonly string _defaultContainer;
    private readonly string _eSqlStatement;
    private readonly string _parametersToken;
    private readonly int _parameterCount;
    private readonly string _includePathsToken;
    private readonly MergeOption _mergeOption;
    private readonly Type _resultType;
    private readonly bool _streaming;

    internal EntitySqlQueryCacheKey(
      string defaultContainerName,
      string eSqlStatement,
      int parameterCount,
      string parametersToken,
      string includePathsToken,
      MergeOption mergeOption,
      bool streaming,
      Type resultType)
    {
      this._defaultContainer = defaultContainerName;
      this._eSqlStatement = eSqlStatement;
      this._parameterCount = parameterCount;
      this._parametersToken = parametersToken;
      this._includePathsToken = includePathsToken;
      this._mergeOption = mergeOption;
      this._streaming = streaming;
      this._resultType = resultType;
      int num = this._eSqlStatement.GetHashCode() ^ this._mergeOption.GetHashCode();
      if (this._parametersToken != null)
        num ^= this._parametersToken.GetHashCode();
      if (this._includePathsToken != null)
        num ^= this._includePathsToken.GetHashCode();
      if (this._defaultContainer != null)
        num ^= this._defaultContainer.GetHashCode();
      this._hashCode = num;
    }

    public override bool Equals(object otherObject)
    {
      if (typeof (EntitySqlQueryCacheKey) != otherObject.GetType())
        return false;
      EntitySqlQueryCacheKey sqlQueryCacheKey = (EntitySqlQueryCacheKey) otherObject;
      if (this._parameterCount == sqlQueryCacheKey._parameterCount && this._mergeOption == sqlQueryCacheKey._mergeOption && (this._streaming == sqlQueryCacheKey._streaming && this.Equals(sqlQueryCacheKey._defaultContainer, this._defaultContainer)) && (this.Equals(sqlQueryCacheKey._eSqlStatement, this._eSqlStatement) && this.Equals(sqlQueryCacheKey._includePathsToken, this._includePathsToken) && this.Equals(sqlQueryCacheKey._parametersToken, this._parametersToken)))
        return object.Equals((object) sqlQueryCacheKey._resultType, (object) this._resultType);
      return false;
    }

    public override int GetHashCode()
    {
      return this._hashCode;
    }

    public override string ToString()
    {
      return string.Join("|", this._defaultContainer, this._eSqlStatement, this._parametersToken, this._includePathsToken, Enum.GetName(typeof (MergeOption), (object) this._mergeOption));
    }
  }
}
