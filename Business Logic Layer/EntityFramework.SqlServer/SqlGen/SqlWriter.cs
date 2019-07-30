// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.SqlGen.SqlWriter
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Data.Entity.Migrations.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace System.Data.Entity.SqlServer.SqlGen
{
  internal class SqlWriter : IndentedTextWriter
  {
    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Transferring ownership")]
    public SqlWriter(StringBuilder b)
      : base((TextWriter) new StringWriter(b, (IFormatProvider) IndentedTextWriter.Culture))
    {
    }
  }
}
