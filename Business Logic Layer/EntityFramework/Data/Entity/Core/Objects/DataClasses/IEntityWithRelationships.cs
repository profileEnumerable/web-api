// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.DataClasses.IEntityWithRelationships
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Objects.DataClasses
{
  /// <summary>
  /// Interface that a data class must implement if exposes relationships
  /// </summary>
  public interface IEntityWithRelationships
  {
    /// <summary>Returns the relationship manager that manages relationships for an instance of an entity type.</summary>
    /// <remarks>
    /// Classes that expose relationships must implement this property
    /// by constructing and setting RelationshipManager in their constructor.
    /// The implementation of this property should use the static method RelationshipManager.Create
    /// to create a new RelationshipManager when needed. Once created, it is expected that this
    /// object will be stored on the entity and will be provided through this property.
    /// </remarks>
    /// <returns>
    /// The <see cref="T:System.Data.Entity.Core.Objects.DataClasses.RelationshipManager" /> for this entity.
    /// </returns>
    RelationshipManager RelationshipManager { get; }
  }
}
