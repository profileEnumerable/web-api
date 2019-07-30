// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.SchemaEnumMember
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Globalization;
using System.Xml;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal class SchemaEnumMember : SchemaElement
  {
    private long? _value;

    public SchemaEnumMember(SchemaElement parentElement)
      : base(parentElement, (IDbDependencyResolver) null)
    {
    }

    public long? Value
    {
      get
      {
        return this._value;
      }
      set
      {
        this._value = value;
      }
    }

    protected override bool HandleAttribute(XmlReader reader)
    {
      bool flag = base.HandleAttribute(reader);
      if (!flag && (flag = SchemaElement.CanHandleAttribute(reader, "Value")))
        this.HandleValueAttribute(reader);
      return flag;
    }

    private void HandleValueAttribute(XmlReader reader)
    {
      long result;
      if (!long.TryParse(reader.Value, NumberStyles.AllowLeadingSign, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        return;
      this._value = new long?(result);
    }
  }
}
