// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Validation.ValidationProvider
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Utilities;

namespace System.Data.Entity.Internal.Validation
{
  internal class ValidationProvider
  {
    private readonly Dictionary<Type, EntityValidator> _entityValidators;
    private readonly EntityValidatorBuilder _entityValidatorBuilder;

    public ValidationProvider(EntityValidatorBuilder builder = null, AttributeProvider attributeProvider = null)
    {
      this._entityValidators = new Dictionary<Type, EntityValidator>();
      this._entityValidatorBuilder = builder ?? new EntityValidatorBuilder(attributeProvider ?? new AttributeProvider());
    }

    public virtual EntityValidator GetEntityValidator(InternalEntityEntry entityEntry)
    {
      Type entityType = entityEntry.EntityType;
      EntityValidator entityValidator1 = (EntityValidator) null;
      if (this._entityValidators.TryGetValue(entityType, out entityValidator1))
        return entityValidator1;
      EntityValidator entityValidator2 = this._entityValidatorBuilder.BuildEntityValidator(entityEntry);
      this._entityValidators[entityType] = entityValidator2;
      return entityValidator2;
    }

    public virtual PropertyValidator GetPropertyValidator(
      InternalEntityEntry owningEntity,
      InternalMemberEntry property)
    {
      EntityValidator entityValidator = this.GetEntityValidator(owningEntity);
      if (entityValidator == null)
        return (PropertyValidator) null;
      return this.GetValidatorForProperty(entityValidator, property);
    }

    protected virtual PropertyValidator GetValidatorForProperty(
      EntityValidator entityValidator,
      InternalMemberEntry memberEntry)
    {
      InternalNestedPropertyEntry nestedPropertyEntry = memberEntry as InternalNestedPropertyEntry;
      if (nestedPropertyEntry == null)
        return entityValidator.GetPropertyValidator(memberEntry.Name);
      ComplexPropertyValidator validatorForProperty = this.GetValidatorForProperty(entityValidator, (InternalMemberEntry) nestedPropertyEntry.ParentPropertyEntry) as ComplexPropertyValidator;
      if (validatorForProperty == null || validatorForProperty.ComplexTypeValidator == null)
        return (PropertyValidator) null;
      return validatorForProperty.ComplexTypeValidator.GetPropertyValidator(memberEntry.Name);
    }

    public virtual EntityValidationContext GetEntityValidationContext(
      InternalEntityEntry entityEntry,
      IDictionary<object, object> items)
    {
      return new EntityValidationContext(entityEntry, new ValidationContext(entityEntry.Entity, (IServiceProvider) null, items));
    }
  }
}
