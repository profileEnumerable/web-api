// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.Util
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Diagnostics;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal static class Util
  {
    internal static void ThrowIfReadOnly(MetadataItem item)
    {
      if (item.IsReadOnly)
        throw new InvalidOperationException(Strings.OperationOnReadOnlyItem);
    }

    [Conditional("DEBUG")]
    internal static void AssertItemHasIdentity(MetadataItem item, string argumentName)
    {
      Check.NotNull<MetadataItem>(item, argumentName);
    }

    internal static ObjectTypeMapping GetObjectMapping(
      EdmType type,
      MetadataWorkspace workspace)
    {
      ItemCollection collection;
      if (workspace.TryGetItemCollection(DataSpace.CSpace, out collection))
        return (ObjectTypeMapping) workspace.GetMap((GlobalItem) type, DataSpace.OCSpace);
      EdmType edmType;
      EdmType cdmType;
      if (type.DataSpace == DataSpace.CSpace)
      {
        edmType = !Helper.IsPrimitiveType(type) ? workspace.GetItem<EdmType>(type.FullName, DataSpace.OSpace) : (EdmType) workspace.GetMappedPrimitiveType(((PrimitiveType) type).PrimitiveTypeKind, DataSpace.OSpace);
        cdmType = type;
      }
      else
      {
        edmType = type;
        cdmType = type;
      }
      if (!Helper.IsPrimitiveType(edmType) && !Helper.IsEntityType(edmType) && !Helper.IsComplexType(edmType))
        throw new NotSupportedException(Strings.Materializer_UnsupportedType);
      return !Helper.IsPrimitiveType(edmType) ? DefaultObjectMappingItemCollection.LoadObjectMapping(cdmType, edmType, (DefaultObjectMappingItemCollection) null) : new ObjectTypeMapping(edmType, cdmType);
    }
  }
}
