// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Validation.TypeValidator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace System.Data.Entity.Internal.Validation
{
  internal abstract class TypeValidator
  {
    private readonly IEnumerable<IValidator> _typeLevelValidators;
    private readonly IEnumerable<PropertyValidator> _propertyValidators;

    public TypeValidator(
      IEnumerable<PropertyValidator> propertyValidators,
      IEnumerable<IValidator> typeLevelValidators)
    {
      this._typeLevelValidators = typeLevelValidators;
      this._propertyValidators = propertyValidators;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by test code.")]
    public IEnumerable<IValidator> TypeLevelValidators
    {
      get
      {
        return this._typeLevelValidators;
      }
    }

    public IEnumerable<PropertyValidator> PropertyValidators
    {
      get
      {
        return this._propertyValidators;
      }
    }

    protected IEnumerable<DbValidationError> Validate(
      EntityValidationContext entityValidationContext,
      InternalPropertyEntry property)
    {
      List<DbValidationError> dbValidationErrorList = new List<DbValidationError>();
      this.ValidateProperties(entityValidationContext, property, dbValidationErrorList);
      if (!dbValidationErrorList.Any<DbValidationError>())
      {
        foreach (IValidator typeLevelValidator in this._typeLevelValidators)
          dbValidationErrorList.AddRange(typeLevelValidator.Validate(entityValidationContext, (InternalMemberEntry) property));
      }
      return (IEnumerable<DbValidationError>) dbValidationErrorList;
    }

    protected abstract void ValidateProperties(
      EntityValidationContext entityValidationContext,
      InternalPropertyEntry parentProperty,
      List<DbValidationError> validationErrors);

    public PropertyValidator GetPropertyValidator(string name)
    {
      return this._propertyValidators.SingleOrDefault<PropertyValidator>((Func<PropertyValidator, bool>) (v => v.PropertyName == name));
    }
  }
}
