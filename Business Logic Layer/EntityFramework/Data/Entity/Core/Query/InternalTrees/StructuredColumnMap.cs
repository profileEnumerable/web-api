// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.StructuredColumnMap
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Text;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal abstract class StructuredColumnMap : ColumnMap
  {
    private readonly ColumnMap[] m_properties;

    internal StructuredColumnMap(TypeUsage type, string name, ColumnMap[] properties)
      : base(type, name)
    {
      this.m_properties = properties;
    }

    internal virtual SimpleColumnMap NullSentinel
    {
      get
      {
        return (SimpleColumnMap) null;
      }
    }

    internal ColumnMap[] Properties
    {
      get
      {
        return this.m_properties;
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = string.Empty;
      stringBuilder.Append("{");
      foreach (ColumnMap property in this.Properties)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) str, (object) property);
        str = ",";
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }
  }
}
