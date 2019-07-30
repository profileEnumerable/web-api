// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Validation.ValidationAttributeValidator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;
using System.Data.Entity.Validation;
using System.Linq;

namespace System.Data.Entity.Internal.Validation
{
  internal class ValidationAttributeValidator : IValidator
  {
    private readonly DisplayAttribute _displayAttribute;
    private readonly ValidationAttribute _validationAttribute;

    public ValidationAttributeValidator(
      ValidationAttribute validationAttribute,
      DisplayAttribute displayAttribute)
    {
      this._validationAttribute = validationAttribute;
      this._displayAttribute = displayAttribute;
    }

    public virtual IEnumerable<DbValidationError> Validate(
      EntityValidationContext entityValidationContext,
      InternalMemberEntry property)
    {
      ValidationContext validationContext = entityValidationContext.ExternalValidationContext;
      validationContext.SetDisplayName(property, this._displayAttribute);
      object obj = property == null ? entityValidationContext.InternalEntity.Entity : property.CurrentValue;
      ValidationResult validationResult;
      try
      {
        validationResult = this._validationAttribute.GetValidationResult(obj, validationContext);
      }
      catch (Exception ex)
      {
        throw new DbUnexpectedValidationException(Strings.DbUnexpectedValidationException_ValidationAttribute((object) validationContext.DisplayName, (object) this._validationAttribute.GetType()), ex);
      }
      if (validationResult == ValidationResult.Success)
        return Enumerable.Empty<DbValidationError>();
      return DbHelpers.SplitValidationResults(validationContext.MemberName, (IEnumerable<ValidationResult>) new ValidationResult[1]
      {
        validationResult
      });
    }
  }
}
