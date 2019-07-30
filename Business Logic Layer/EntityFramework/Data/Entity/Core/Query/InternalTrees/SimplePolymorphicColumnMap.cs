// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.SimplePolymorphicColumnMap
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class SimplePolymorphicColumnMap : TypedColumnMap
  {
    private readonly SimpleColumnMap m_typeDiscriminator;
    private readonly Dictionary<object, TypedColumnMap> m_typedColumnMap;

    internal SimplePolymorphicColumnMap(
      TypeUsage type,
      string name,
      ColumnMap[] baseTypeColumns,
      SimpleColumnMap typeDiscriminator,
      Dictionary<object, TypedColumnMap> typeChoices)
      : base(type, name, baseTypeColumns)
    {
      this.m_typedColumnMap = typeChoices;
      this.m_typeDiscriminator = typeDiscriminator;
    }

    internal SimpleColumnMap TypeDiscriminator
    {
      get
      {
        return this.m_typeDiscriminator;
      }
    }

    internal Dictionary<object, TypedColumnMap> TypeChoices
    {
      get
      {
        return this.m_typedColumnMap;
      }
    }

    [DebuggerNonUserCode]
    internal override void Accept<TArgType>(ColumnMapVisitor<TArgType> visitor, TArgType arg)
    {
      visitor.Visit(this, arg);
    }

    [DebuggerNonUserCode]
    internal override TResultType Accept<TResultType, TArgType>(
      ColumnMapVisitorWithResults<TResultType, TArgType> visitor,
      TArgType arg)
    {
      return visitor.Visit(this, arg);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = string.Empty;
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "P{{TypeId={0}, ", (object) this.TypeDiscriminator);
      foreach (KeyValuePair<object, TypedColumnMap> typeChoice in this.TypeChoices)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}({1},{2})", (object) str, typeChoice.Key, (object) typeChoice.Value);
        str = ",";
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }
  }
}
