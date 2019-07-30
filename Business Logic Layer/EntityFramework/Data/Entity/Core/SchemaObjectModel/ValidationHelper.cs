// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.SchemaObjectModel.ValidationHelper
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Resources;

namespace System.Data.Entity.Core.SchemaObjectModel
{
  internal static class ValidationHelper
  {
    internal static void ValidateFacets(
      SchemaElement element,
      SchemaType type,
      TypeUsageBuilder typeUsageBuilder)
    {
      if (type != null)
      {
        SchemaEnumType schemaEnumType = type as SchemaEnumType;
        if (schemaEnumType != null)
        {
          typeUsageBuilder.ValidateEnumFacets(schemaEnumType);
        }
        else
        {
          if (type is ScalarType || !typeUsageBuilder.HasUserDefinedFacets)
            return;
          element.AddError(ErrorCode.FacetOnNonScalarType, EdmSchemaErrorSeverity.Error, (object) Strings.FacetsOnNonScalarType((object) type.FQName));
        }
      }
      else
      {
        if (!typeUsageBuilder.HasUserDefinedFacets)
          return;
        element.AddError(ErrorCode.IncorrectlyPlacedFacet, EdmSchemaErrorSeverity.Error, (object) Strings.FacetDeclarationRequiresTypeAttribute);
      }
    }

    internal static void ValidateTypeDeclaration(
      SchemaElement element,
      SchemaType type,
      SchemaElement typeSubElement)
    {
      if (type == null && typeSubElement == null)
        element.AddError(ErrorCode.TypeNotDeclared, EdmSchemaErrorSeverity.Error, (object) Strings.TypeMustBeDeclared);
      if (type == null || typeSubElement == null)
        return;
      element.AddError(ErrorCode.TypeDeclaredAsAttributeAndElement, EdmSchemaErrorSeverity.Error, (object) Strings.TypeDeclaredAsAttributeAndElement);
    }

    internal static void ValidateRefType(SchemaElement element, SchemaType type)
    {
      if (type == null || type is SchemaEntityType)
        return;
      element.AddError(ErrorCode.ReferenceToNonEntityType, EdmSchemaErrorSeverity.Error, (object) Strings.ReferenceToNonEntityType((object) type.FQName));
    }
  }
}
