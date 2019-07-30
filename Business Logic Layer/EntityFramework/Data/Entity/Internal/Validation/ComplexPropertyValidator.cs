// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Validation.ComplexPropertyValidator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace System.Data.Entity.Internal.Validation
{
  internal class ComplexPropertyValidator : PropertyValidator
  {
    private readonly ComplexTypeValidator _complexTypeValidator;

    public ComplexTypeValidator ComplexTypeValidator
    {
      get
      {
        return this._complexTypeValidator;
      }
    }

    public ComplexPropertyValidator(
      string propertyName,
      IEnumerable<IValidator> propertyValidators,
      ComplexTypeValidator complexTypeValidator)
      : base(propertyName, propertyValidators)
    {
      this._complexTypeValidator = complexTypeValidator;
    }

    public override IEnumerable<DbValidationError> Validate(
      EntityValidationContext entityValidationContext,
      InternalMemberEntry property)
    {
      List<DbValidationError> source = new List<DbValidationError>();
      source.AddRange(base.Validate(entityValidationContext, property));
      if (!source.Any<DbValidationError>() && property.CurrentValue != null && this._complexTypeValidator != null)
        source.AddRange(this._complexTypeValidator.Validate(entityValidationContext, (InternalPropertyEntry) property));
      return (IEnumerable<DbValidationError>) source;
    }
  }
}
