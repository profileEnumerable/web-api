// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Query.PlanCompiler.TypeUtils
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Core.Query.PlanCompiler
{
  internal static class TypeUtils
  {
    internal static bool IsStructuredType(TypeUsage type)
    {
      if (!TypeSemantics.IsReferenceType(type) && !TypeSemantics.IsRowType(type) && (!TypeSemantics.IsEntityType(type) && !TypeSemantics.IsRelationshipType(type)))
        return TypeSemantics.IsComplexType(type);
      return true;
    }

    internal static bool IsCollectionType(TypeUsage type)
    {
      return TypeSemantics.IsCollectionType(type);
    }

    internal static bool IsEnumerationType(TypeUsage type)
    {
      return TypeSemantics.IsEnumerationType(type);
    }

    internal static TypeUsage CreateCollectionType(TypeUsage elementType)
    {
      return TypeHelpers.CreateCollectionTypeUsage(elementType);
    }
  }
}
