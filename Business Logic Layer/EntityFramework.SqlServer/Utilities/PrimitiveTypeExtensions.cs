// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.SqlServer.Utilities.PrimitiveTypeExtensions
// Assembly: EntityFramework.SqlServer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 0025BC3E-2252-4BA9-A352-D7F62FAA5B3F
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.SqlServer.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.SqlServer.Utilities
{
  internal static class PrimitiveTypeExtensions
  {
    internal static bool IsSpatialType(this PrimitiveType type)
    {
      PrimitiveTypeKind primitiveTypeKind = type.PrimitiveTypeKind;
      if (primitiveTypeKind >= PrimitiveTypeKind.Geometry)
        return primitiveTypeKind <= PrimitiveTypeKind.GeographyCollection;
      return false;
    }
  }
}
