// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DbContextExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace System.Data.Entity.Utilities
{
  internal static class DbContextExtensions
  {
    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    public static XDocument GetModel(this DbContext context)
    {
      return DbContextExtensions.GetModel((Action<XmlWriter>) (w => EdmxWriter.WriteEdmx(context, w)));
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    public static XDocument GetModel(Action<XmlWriter> writeXml)
    {
      using (MemoryStream memoryStream1 = new MemoryStream())
      {
        MemoryStream memoryStream2 = memoryStream1;
        XmlWriterSettings settings = new XmlWriterSettings()
        {
          Indent = true
        };
        using (XmlWriter xmlWriter = XmlWriter.Create((Stream) memoryStream2, settings))
          writeXml(xmlWriter);
        memoryStream1.Position = 0L;
        return XDocument.Load((Stream) memoryStream1);
      }
    }
  }
}
