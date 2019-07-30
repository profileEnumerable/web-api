// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Validation.ComplexTypeValidator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace System.Data.Entity.Internal.Validation
{
  internal class ComplexTypeValidator : TypeValidator
  {
    public ComplexTypeValidator(
      IEnumerable<PropertyValidator> propertyValidators,
      IEnumerable<IValidator> typeLevelValidators)
      : base(propertyValidators, typeLevelValidators)
    {
    }

    public new IEnumerable<DbValidationError> Validate(
      EntityValidationContext entityValidationContext,
      InternalPropertyEntry property)
    {
      return base.Validate(entityValidationContext, property);
    }

    protected override void ValidateProperties(
      EntityValidationContext entityValidationContext,
      InternalPropertyEntry parentProperty,
      List<DbValidationError> validationErrors)
    {
      foreach (PropertyValidator propertyValidator in this.PropertyValidators)
      {
        InternalPropertyEntry internalPropertyEntry = parentProperty.Property(propertyValidator.PropertyName, (Type) null, false);
        validationErrors.AddRange(propertyValidator.Validate(entityValidationContext, (InternalMemberEntry) internalPropertyEntry));
      }
    }
  }
}
