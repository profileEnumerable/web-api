// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Metadata.Edm.EntitySetBaseCollection
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Resources;
using System.Data.Entity.Utilities;

namespace System.Data.Entity.Core.Metadata.Edm
{
  internal sealed class EntitySetBaseCollection : MetadataCollection<EntitySetBase>
  {
    private readonly EntityContainer _entityContainer;

    internal EntitySetBaseCollection(EntityContainer entityContainer)
      : this(entityContainer, (IEnumerable<EntitySetBase>) null)
    {
    }

    internal EntitySetBaseCollection(
      EntityContainer entityContainer,
      IEnumerable<EntitySetBase> items)
      : base(items)
    {
      Check.NotNull<EntityContainer>(entityContainer, nameof (entityContainer));
      this._entityContainer = entityContainer;
    }

    public override EntitySetBase this[int index]
    {
      get
      {
        return base[index];
      }
      set
      {
        throw new InvalidOperationException(Strings.OperationOnReadOnlyCollection);
      }
    }

    public override EntitySetBase this[string identity]
    {
      get
      {
        return base[identity];
      }
      set
      {
        throw new InvalidOperationException(Strings.OperationOnReadOnlyCollection);
      }
    }

    public override void Add(EntitySetBase item)
    {
      Check.NotNull<EntitySetBase>(item, nameof (item));
      EntitySetBaseCollection.ThrowIfItHasEntityContainer(item, nameof (item));
      base.Add(item);
      item.ChangeEntityContainerWithoutCollectionFixup(this._entityContainer);
    }

    private static void ThrowIfItHasEntityContainer(EntitySetBase entitySet, string argumentName)
    {
      Check.NotNull<EntitySetBase>(entitySet, argumentName);
      if (entitySet.EntityContainer != null)
        throw new ArgumentException(Strings.EntitySetInAnotherContainer, argumentName);
    }
  }
}
