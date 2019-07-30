// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Utilities.DbModelExtensions
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Infrastructure;
using System.Xml;
using System.Xml.Linq;

namespace System.Data.Entity.Utilities
{
  internal static class DbModelExtensions
  {
    public static XDocument GetModel(this DbModel model)
    {
      return DbContextExtensions.GetModel((Action<XmlWriter>) (w => EdmxWriter.WriteEdmx(model, w)));
    }
  }
}
