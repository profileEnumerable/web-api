// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.DefaultModelCacheKey
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Internal
{
  internal sealed class DefaultModelCacheKey : IDbModelCacheKey
  {
    private readonly Type _contextType;
    private readonly string _providerName;
    private readonly Type _providerType;
    private readonly string _customKey;

    public DefaultModelCacheKey(
      Type contextType,
      string providerName,
      Type providerType,
      string customKey)
    {
      this._contextType = contextType;
      this._providerName = providerName;
      this._providerType = providerType;
      this._customKey = customKey;
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      DefaultModelCacheKey other = obj as DefaultModelCacheKey;
      if (other != null)
        return this.Equals(other);
      return false;
    }

    public override int GetHashCode()
    {
      return this._contextType.GetHashCode() * 397 ^ this._providerName.GetHashCode() ^ this._providerType.GetHashCode() ^ (!string.IsNullOrWhiteSpace(this._customKey) ? this._customKey.GetHashCode() : 0);
    }

    private bool Equals(DefaultModelCacheKey other)
    {
      if (this._contextType == other._contextType && string.Equals(this._providerName, other._providerName) && object.Equals((object) this._providerType, (object) other._providerType))
        return string.Equals(this._customKey, other._customKey);
      return false;
    }
  }
}
