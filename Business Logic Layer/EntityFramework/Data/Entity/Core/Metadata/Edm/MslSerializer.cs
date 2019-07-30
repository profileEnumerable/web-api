// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MslSerializer
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Utilities;
using System.Xml;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal class MslSerializer
  {
    public virtual bool Serialize(DbDatabaseMapping databaseMapping, XmlWriter xmlWriter)
    {
      Check.NotNull<DbDatabaseMapping>(databaseMapping, nameof (databaseMapping));
      Check.NotNull<XmlWriter>(xmlWriter, nameof (xmlWriter));
      new MslXmlSchemaWriter(xmlWriter, databaseMapping.Model.SchemaVersion).WriteSchema(databaseMapping);
      return true;
    }
  }
}
