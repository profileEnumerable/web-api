// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Migrations.Edm.ModelCompressor
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace System.Data.Entity.Migrations.Edm
{
  internal class ModelCompressor
  {
    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    public virtual byte[] Compress(XDocument model)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Compress))
          model.Save((Stream) gzipStream);
        return memoryStream.ToArray();
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    public virtual XDocument Decompress(byte[] bytes)
    {
      using (MemoryStream memoryStream = new MemoryStream(bytes))
      {
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Decompress))
          return XDocument.Load((Stream) gzipStream);
      }
    }
  }
}
