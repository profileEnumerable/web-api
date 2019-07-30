// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Internal.EntitySetTypePair
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Entity.Internal
{
  internal class EntitySetTypePair : Tuple<EntitySet, Type>
  {
    public EntitySetTypePair(EntitySet entitySet, Type type)
      : base(entitySet, type)
    {
    }

    public EntitySet EntitySet
    {
      get
      {
        return this.Item1;
      }
    }

    public Type BaseType
    {
      get
      {
        return this.Item2;
      }
    }
  }
}
