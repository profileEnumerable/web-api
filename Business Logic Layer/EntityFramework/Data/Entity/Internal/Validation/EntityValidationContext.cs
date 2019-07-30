// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.Validation.EntityValidationContext
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.ComponentModel.DataAnnotations;

namespace System.Data.Entity.Internal.Validation
{
  internal class EntityValidationContext
  {
    private readonly InternalEntityEntry _entityEntry;

    public EntityValidationContext(
      InternalEntityEntry entityEntry,
      ValidationContext externalValidationContext)
    {
      this._entityEntry = entityEntry;
      this.ExternalValidationContext = externalValidationContext;
    }

    public ValidationContext ExternalValidationContext { get; private set; }

    public InternalEntityEntry InternalEntity
    {
      get
      {
        return this._entityEntry;
      }
    }
  }
}
