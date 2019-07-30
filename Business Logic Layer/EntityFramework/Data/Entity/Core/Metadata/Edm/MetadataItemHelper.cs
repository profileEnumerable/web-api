// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.MetadataItemHelper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Linq;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal static class MetadataItemHelper
  {
    internal const string SchemaErrorsMetadataPropertyName = "EdmSchemaErrors";
    internal const string SchemaInvalidMetadataPropertyName = "EdmSchemaInvalid";

    public static bool IsInvalid(MetadataItem instance)
    {
      MetadataProperty metadataProperty;
      if (!instance.MetadataProperties.TryGetValue("EdmSchemaInvalid", false, out metadataProperty) || metadataProperty == null)
        return false;
      return (bool) metadataProperty.Value;
    }

    public static bool HasSchemaErrors(MetadataItem instance)
    {
      return instance.MetadataProperties.Contains("EdmSchemaErrors");
    }

    public static IEnumerable<EdmSchemaError> GetSchemaErrors(
      MetadataItem instance)
    {
      MetadataProperty metadataProperty;
      if (!instance.MetadataProperties.TryGetValue("EdmSchemaErrors", false, out metadataProperty) || metadataProperty == null)
        return Enumerable.Empty<EdmSchemaError>();
      return (IEnumerable<EdmSchemaError>) metadataProperty.Value;
    }
  }
}
