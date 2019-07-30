// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.DiscriminatedEntityIdentity
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Text;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class DiscriminatedEntityIdentity : EntityIdentity
  {
    private readonly SimpleColumnMap m_entitySetColumn;
    private readonly EntitySet[] m_entitySetMap;

    internal DiscriminatedEntityIdentity(
      SimpleColumnMap entitySetColumn,
      EntitySet[] entitySetMap,
      SimpleColumnMap[] keyColumns)
      : base(keyColumns)
    {
      this.m_entitySetColumn = entitySetColumn;
      this.m_entitySetMap = entitySetMap;
    }

    internal SimpleColumnMap EntitySetColumnMap
    {
      get
      {
        return this.m_entitySetColumn;
      }
    }

    internal EntitySet[] EntitySetMap
    {
      get
      {
        return this.m_entitySetMap;
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = string.Empty;
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "[(Keys={");
      foreach (SimpleColumnMap key in this.Keys)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) str, (object) key);
        str = ",";
      }
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "})]");
      return stringBuilder.ToString();
    }
  }
}
