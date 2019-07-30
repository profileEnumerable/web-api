// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Core.Common.Internal.Materialization.Coordinator
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 000F5452-2AD1-45BF-987B-3043022F9799
// Assembly location: C:\Users\suckt\source\repos\Epam_Lab_Task\packages\EntityFramework.6.1.3\lib\net45\EntityFramework.dll

namespace System.Data.Entity.Core.Common.Internal.Materialization
{
  internal abstract class Coordinator
  {
    internal readonly CoordinatorFactory CoordinatorFactory;
    internal readonly Coordinator Parent;
    internal readonly Coordinator Next;

    public Coordinator Child { get; protected set; }

    public bool IsEntered { get; protected set; }

    internal bool IsRoot
    {
      get
      {
        return null == this.Parent;
      }
    }

    protected Coordinator(
      CoordinatorFactory coordinatorFactory,
      Coordinator parent,
      Coordinator next)
    {
      this.CoordinatorFactory = coordinatorFactory;
      this.Parent = parent;
      this.Next = next;
    }

    internal void Initialize(Shaper shaper)
    {
      this.ResetCollection(shaper);
      shaper.State[this.CoordinatorFactory.StateSlot] = (object) this;
      if (this.Child != null)
        this.Child.Initialize(shaper);
      if (this.Next == null)
        return;
      this.Next.Initialize(shaper);
    }

    internal int MaxDistanceToLeaf()
    {
      int val1 = 0;
      for (Coordinator coordinator = this.Child; coordinator != null; coordinator = coordinator.Next)
        val1 = Math.Max(val1, coordinator.MaxDistanceToLeaf() + 1);
      return val1;
    }

    internal abstract void ResetCollection(Shaper shaper);

    internal bool HasNextElement(Shaper shaper)
    {
      bool flag = false;
      if (!this.IsEntered || !this.CoordinatorFactory.CheckKeys(shaper))
      {
        int num = this.CoordinatorFactory.SetKeys(shaper) ? 1 : 0;
        this.IsEntered = true;
        flag = true;
      }
      return flag;
    }

    internal abstract void ReadNextElement(Shaper shaper);
  }
}
