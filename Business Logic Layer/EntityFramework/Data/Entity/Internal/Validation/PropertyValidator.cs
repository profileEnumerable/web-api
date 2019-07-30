// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Validation.PropertyValidator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics.CodeAnalysis;

namespace System.Data.Entity.Internal.Validation
{
  internal class PropertyValidator
  {
    private readonly IEnumerable<IValidator> _propertyValidators;
    private readonly string _propertyName;

    public PropertyValidator(string propertyName, IEnumerable<IValidator> propertyValidators)
    {
      this._propertyValidators = propertyValidators;
      this._propertyName = propertyName;
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used by test code.")]
    public IEnumerable<IValidator> PropertyAttributeValidators
    {
      get
      {
        return this._propertyValidators;
      }
    }

    public string PropertyName
    {
      get
      {
        return this._propertyName;
      }
    }

    public virtual IEnumerable<DbValidationError> Validate(
      EntityValidationContext entityValidationContext,
      InternalMemberEntry property)
    {
      List<DbValidationError> dbValidationErrorList = new List<DbValidationError>();
      foreach (IValidator propertyValidator in this._propertyValidators)
        dbValidationErrorList.AddRange(propertyValidator.Validate(entityValidationContext, property));
      return (IEnumerable<DbValidationError>) dbValidationErrorList;
    }
  }
}
