// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.WrappedEntityKey
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;
using System.Linq;

namespace System.Data.Entity.Internal
{
  internal class WrappedEntityKey
  {
    private readonly IEnumerable<KeyValuePair<string, object>> _keyValuePairs;
    private readonly EntityKey _key;

    public WrappedEntityKey(
      EntitySet entitySet,
      string entitySetName,
      object[] keyValues,
      string keyValuesParamName)
    {
      if (keyValues == null)
        keyValues = new object[1];
      List<string> list = entitySet.ElementType.KeyMembers.Select<EdmMember, string>((Func<EdmMember, string>) (m => m.Name)).ToList<string>();
      if (list.Count != keyValues.Length)
        throw new ArgumentException(Strings.DbSet_WrongNumberOfKeyValuesPassed, keyValuesParamName);
      this._keyValuePairs = list.Zip<string, object, KeyValuePair<string, object>>((IEnumerable<object>) keyValues, (Func<string, object, KeyValuePair<string, object>>) ((name, value) => new KeyValuePair<string, object>(name, value)));
      if (!((IEnumerable<object>) keyValues).All<object>((Func<object, bool>) (v => v != null)))
        return;
      this._key = new EntityKey(entitySetName, this.KeyValuePairs);
    }

    public bool HasNullValues
    {
      get
      {
        return this._key == (EntityKey) null;
      }
    }

    public EntityKey EntityKey
    {
      get
      {
        return this._key;
      }
    }

    public IEnumerable<KeyValuePair<string, object>> KeyValuePairs
    {
      get
      {
        return this._keyValuePairs;
      }
    }
  }
}
