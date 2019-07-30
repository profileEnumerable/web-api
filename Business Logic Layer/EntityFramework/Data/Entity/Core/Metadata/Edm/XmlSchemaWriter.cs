// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.XmlSchemaWriter
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Text;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal abstract class XmlSchemaWriter
  {
    protected XmlWriter _xmlWriter;
    protected double _version;

    internal void WriteComment(string comment)
    {
      if (string.IsNullOrEmpty(comment))
        return;
      this._xmlWriter.WriteComment(comment);
    }

    internal virtual void WriteEndElement()
    {
      this._xmlWriter.WriteEndElement();
    }

    protected static string GetQualifiedTypeName(string prefix, string typeName)
    {
      return new StringBuilder().Append(prefix).Append(".").Append(typeName).ToString();
    }

    internal static string GetLowerCaseStringFromBoolValue(bool value)
    {
      return !value ? "false" : "true";
    }
  }
}
