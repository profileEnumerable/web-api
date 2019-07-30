// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.InternalTrees.ComplexTypeColumnMap
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.Globalization;

namespace System.Data.Entity.Core.Query.InternalTrees
{
  internal class ComplexTypeColumnMap : TypedColumnMap
  {
    private readonly SimpleColumnMap m_nullSentinel;

    internal ComplexTypeColumnMap(
      TypeUsage type,
      string name,
      ColumnMap[] properties,
      SimpleColumnMap nullSentinel)
      : base(type, name, properties)
    {
      this.m_nullSentinel = nullSentinel;
    }

    internal override SimpleColumnMap NullSentinel
    {
      get
      {
        return this.m_nullSentinel;
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
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "C{0}", (object) base.ToString());
    }
  }
}
