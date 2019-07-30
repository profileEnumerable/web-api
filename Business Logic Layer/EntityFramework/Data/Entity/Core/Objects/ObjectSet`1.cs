// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Objects.ObjectSet`1
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace System.Data.Entity.Core.Objects
{
  /// <summary>
  /// Represents a typed entity set that is used to perform create, read, update, and delete operations.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public class ObjectSet<TEntity> : ObjectQuery<TEntity>, IObjectSet<TEntity>, IQueryable<TEntity>, IEnumerable<TEntity>, IQueryable, IEnumerable
    where TEntity : class
  {
    private readonly EntitySet _entitySet;

    internal ObjectSet(EntitySet entitySet, ObjectContext context)
      : base((EntitySetBase) entitySet, context, MergeOption.AppendOnly)
    {
      this._entitySet = entitySet;
    }

    /// <summary>
    /// Gets the metadata of the entity set represented by this <see cref="T:System.Data.Entity.Core.Objects.ObjectSet`1" /> instance.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Data.Entity.Core.Metadata.Edm.EntitySet" /> object.
    /// </returns>
    public EntitySet EntitySet
    {
      get
      {
        return this._entitySet;
      }
    }

    /// <summary>Adds an object to the object context in the current entity set. </summary>
    /// <param name="entity">The object to add.</param>
    public void AddObject(TEntity entity)
    {
      this.Context.AddObject(this.FullyQualifiedEntitySetName, (object) entity);
    }

    /// <summary>Attaches an object or object graph to the object context in the current entity set. </summary>
    /// <param name="entity">The object to attach.</param>
    public void Attach(TEntity entity)
    {
      this.Context.AttachTo(this.FullyQualifiedEntitySetName, (object) entity);
    }

    /// <summary>Marks an object for deletion. </summary>
    /// <param name="entity">
    /// An object that represents the entity to delete. The object can be in any state except
    /// <see cref="F:System.Data.Entity.EntityState.Detached" />
    /// .
    /// </param>
    public void DeleteObject(TEntity entity)
    {
      this.Context.DeleteObject((object) entity, this.EntitySet);
    }

    /// <summary>Removes the object from the object context.</summary>
    /// <param name="entity">
    /// Object to be detached. Only the  entity  is removed; if there are any related objects that are being tracked by the same
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateManager" />
    /// , those will not be detached automatically.
    /// </param>
    public void Detach(TEntity entity)
    {
      this.Context.Detach((object) entity, this.EntitySet);
    }

    /// <summary>
    /// Copies the scalar values from the supplied object into the object in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// that has the same key.
    /// </summary>
    /// <returns>The updated object.</returns>
    /// <param name="currentEntity">
    /// The detached object that has property updates to apply to the original object. The entity key of  currentEntity  must match the
    /// <see cref="P:System.Data.Entity.Core.Objects.ObjectStateEntry.EntityKey" />
    /// property of an entry in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </param>
    public TEntity ApplyCurrentValues(TEntity currentEntity)
    {
      return this.Context.ApplyCurrentValues<TEntity>(this.FullyQualifiedEntitySetName, currentEntity);
    }

    /// <summary>
    /// Sets the <see cref="P:System.Data.Entity.Core.Objects.ObjectStateEntry.OriginalValues" /> property of an
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectStateEntry" />
    /// to match the property values of a supplied object.
    /// </summary>
    /// <returns>The updated object.</returns>
    /// <param name="originalEntity">
    /// The detached object that has property updates to apply to the original object. The entity key of  originalEntity  must match the
    /// <see cref="P:System.Data.Entity.Core.Objects.ObjectStateEntry.EntityKey" />
    /// property of an entry in the
    /// <see cref="T:System.Data.Entity.Core.Objects.ObjectContext" />
    /// .
    /// </param>
    public TEntity ApplyOriginalValues(TEntity originalEntity)
    {
      return this.Context.ApplyOriginalValues<TEntity>(this.FullyQualifiedEntitySetName, originalEntity);
    }

    /// <summary>Creates a new entity type object.</summary>
    /// <returns>The new entity type object, or an instance of a proxy type that corresponds to the entity type.</returns>
    public TEntity CreateObject()
    {
      return this.Context.CreateObject<TEntity>();
    }

    /// <summary>Creates an instance of the specified type.</summary>
    /// <returns>An instance of the requested type  T , or an instance of a proxy type that corresponds to the type  T .</returns>
    /// <typeparam name="T">Type of object to be returned.</typeparam>
    public T CreateObject<T>() where T : class, TEntity
    {
      return this.Context.CreateObject<T>();
    }

    private string FullyQualifiedEntitySetName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) this._entitySet.EntityContainer.Name, (object) this._entitySet.Name);
      }
    }
  }
}
