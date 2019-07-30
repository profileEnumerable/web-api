// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.ModelHashCalculator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace System.Data.Entity.Internal
{
  internal class ModelHashCalculator
  {
    public virtual string Calculate(DbCompiledModel compiledModel)
    {
      DbProviderInfo providerInfo = compiledModel.ProviderInfo;
      DbModelBuilder dbModelBuilder = compiledModel.CachedModelBuilder.Clone();
      EdmMetadataContext.ConfigureEdmMetadata(dbModelBuilder.ModelConfiguration);
      EdmModel database = dbModelBuilder.Build(providerInfo).DatabaseMapping.Database;
      database.SchemaVersion = 2.0;
      StringBuilder stringBuilder = new StringBuilder();
      StringBuilder output = stringBuilder;
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        Indent = true
      };
      using (XmlWriter xmlWriter = XmlWriter.Create(output, settings))
        new SsdlSerializer().Serialize(database, providerInfo.ProviderInvariantName, providerInfo.ProviderManifestToken, xmlWriter, true);
      return ModelHashCalculator.ComputeSha256Hash(stringBuilder.ToString());
    }

    private static string ComputeSha256Hash(string input)
    {
      byte[] hash = ModelHashCalculator.GetSha256HashAlgorithm().ComputeHash(Encoding.ASCII.GetBytes(input));
      StringBuilder stringBuilder = new StringBuilder(hash.Length * 2);
      foreach (byte num in hash)
        stringBuilder.Append(num.ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture));
      return stringBuilder.ToString();
    }

    private static SHA256 GetSha256HashAlgorithm()
    {
      try
      {
        return (SHA256) new SHA256CryptoServiceProvider();
      }
      catch (PlatformNotSupportedException ex)
      {
        return (SHA256) new SHA256Managed();
      }
    }
  }
}
